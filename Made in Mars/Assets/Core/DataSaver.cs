using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using UnityEngine.UI;


[Serializable]
public class DataSaver {

	public static DataSaver s;

	public SaveFile mySave;
	public const string saveName = "mySave.data";

	public string saveFilePathAndFileName {
		get { return Application.persistentDataPath + "/" + saveName; }
	}
	public delegate void SaveYourself ();
	public static event SaveYourself saveEvent;
	
	public SaveFile GetSave() {
		return mySave;
	}

	public void ClearSave() {
		mySave = new SaveFile();
		OnNewSave();
	}

	public ShipCard[] newSaveCards = new ShipCard[0];
	public void OnNewSave() {
		for (int i = 0; i < newSaveCards.Length; i++) {
			var card = newSaveCards[i];
			if (card != null) {
				mySave.availableShipCards.Add(new ShipCardData(card));
			}
		}

		mySave.isRealSaveFile = true;
	}

	//--------------------------------------------------------------------------------------------------------------------------------

	public bool dontSave = false;
	public void SaveGame () {
		if (!dontSave) {
			saveEvent?.Invoke();
			Save();
		}
	}

	void Save () {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(saveFilePathAndFileName);
		
		mySave.isRealSaveFile = true;
		SaveFile data = mySave;

		bf.Serialize(file, data);
		file.Close();
		Debug.Log("Data Saved to " + saveFilePathAndFileName);
	}

	public bool Load () {
		if (mySave.isRealSaveFile) {
			return true;
		}
		try {
			if (File.Exists(saveFilePathAndFileName)) {
				BinaryFormatter bf = new BinaryFormatter();
				FileStream file = File.Open(saveFilePathAndFileName, FileMode.Open);
				try {
					SaveFile data = (SaveFile)bf.Deserialize(file); 
					file.Close();

					mySave = data;
					Debug.Log("Data Loaded");
				} catch (Exception e) {
					file.Close();
					throw;
				}
				return true;
			} else {
				Debug.Log("No Data Found");
				mySave = new SaveFile();
				OnNewSave();
				return false;
			}
		} catch {
			File.Delete(saveFilePathAndFileName );
			Debug.Log("Corrupt Data Deleted");
			mySave = new SaveFile();
			OnNewSave();
			
			return false;
		}
	}

	public void DeleteSave () {
		File.Delete(saveFilePathAndFileName);
		mySave = new SaveFile();
		OnNewSave();
	}

	[System.Serializable]
	public class SaveFile {
		public bool isRealSaveFile = false;
		public LocalPlanetData currentPlanet;

		public List<ShipCardData> activeShipCards = new List<ShipCardData>();
		public List<ShipCardData> availableShipCards = new List<ShipCardData>();
		public List<ShipCardData> unlockedShipCards = new List<ShipCardData>();

		public List<DataHolder.CountedItem> itemsMadeData = new List<DataHolder.CountedItem>();

	}
	
	[Serializable]
	public class LocalPlanetData {
		public List<BuildingSaveData> buildingData = new List<BuildingSaveData>();
		public List<BeltSaveData> beltData = new List<BeltSaveData>();
		public List<ConnectorSaveData> connectorData = new List<ConnectorSaveData>();
		public List<ConstructionSaveData> constructionData = new List<ConstructionSaveData>();
		public List<DroneSaveData> droneData = new List<DroneSaveData>();
		public string[] buildingBarData = new string[0];
		public PlanetData planetData;
		public TileData[,] tileData = new TileData[0,0];
		public bool isSpaceshipLanded = false;
		public bool newPlanet = true;


		public LocalPlanetData(PlanetData planetData) {
			this.planetData = planetData;
		}
	}
	
	[Serializable]
	public class PlanetData {
		public string planetSchematicUniqueName;
		public int[] oreDensities = new int[0];
		public string[] oreUniqueNames = new string[0];

		public PlanetData(string planetSchematicUniqueName, int[] oreDensities, string[] oreUniqueNames) {
			this.planetSchematicUniqueName = planetSchematicUniqueName;
			this.oreDensities = oreDensities;
			this.oreUniqueNames = oreUniqueNames;
		}
	}

	[System.Serializable]
	public class  ShipCardData {
		public string myUniqueName;

		public ShipCardData(ShipCard card) {
			myUniqueName = card.uniqueName;
		}

		public ShipCard GetShipCard() {
			return DataHolder.s.GetShipCard(myUniqueName);
		}
	}
	
	[System.Serializable]
	public class ConstructionSaveData {
		public string myUniqueName;
		public Position center;
		public int direction;
		public bool isConstruction;
		public bool isAssignedDrone;
		public InventoryData[] myInv;
		public InventoryData[] afterConstructionInventory;

		public ConstructionSaveData (string _myUniqueName, Position location, int _direction,
			bool _isConstruction, bool _isAssignedDrone,
			List<InventoryItemSlot> _myInv, List<InventoryItemSlot> _afterConstructionInventory) {
			myUniqueName = _myUniqueName;
			center = location;
			direction = _direction;
			isConstruction = _isConstruction;
			isAssignedDrone = _isAssignedDrone;
			myInv = InventoryData.ConvertToSaveData(_myInv);
			afterConstructionInventory = InventoryData.ConvertToSaveData(_afterConstructionInventory);
		}
		
		
		public List<InventoryItemSlot> myInvConverted() {
			return InventoryData.ConvertToRegularData(myInv);
		}
	}

	[System.Serializable]
	public class BuildingSaveData {
		public string myUniqueName;
		public Position center;
		public InventoryData[] myInv;
		
		//crafting info
		public int lastCheckid = 0;
		public float[] curCraftingProgress;

		public BuildingSaveData (string _myUniqueName, Position _center, List<InventoryItemSlot> slots,
			int _lastCheckid, float[] _curCraftingProgress) {
			
			myUniqueName = _myUniqueName;
			center = _center;
			myInv = InventoryData.ConvertToSaveData(slots);

			lastCheckid = _lastCheckid;
			curCraftingProgress = _curCraftingProgress;
		}
		
		
		public List<InventoryItemSlot> myInvConverted() {
			return InventoryData.ConvertToRegularData(myInv);
		}
	}

	[System.Serializable]
	public class BeltSaveData {
		public Position start;
		public Position end;
		public int direction;
		public InventoryData[] myInv;
		
		public BeltSaveData (Position _start, Position _end, int _direction, List<InventoryItemSlot> slots) {
			start = _start;
			end = _end;
			direction = _direction;
			myInv = InventoryData.ConvertToSaveData(slots);
		}

		public List<InventoryItemSlot> myInvConverted() {
			return InventoryData.ConvertToRegularData(myInv);
		}
	}
	
	[System.Serializable]
	public class ConnectorSaveData {
		public Position start;
		public Position end;
		public int direction;
		//public InventoryData[] myInv;
		public ConnectorSaveData (Position _start, Position _end, int _direction /*List<InventoryItemSlot> slots*/) {
			start = _start;
			end = _end;
			direction = _direction;
			//myInv = InventoryData.ConvertToSaveData(slots);
		}
		
		/*public List<InventoryItemSlot> myInvConverted() {
			return InventoryData.ConvertToRegularData(myInv);
		}*/
	}
	
	[System.Serializable]
	public class DroneSaveData {
		public Position curPosition;
		public Position targetPosition;
		
		public bool isTravelling;
		public bool isBusy;
		public bool isLaser;
		
		public Position currentTaskPosition;
		public InventoryData[] currentTaskMaterials;
		
		public InventoryData[] myInv;

		public int droneState;
		
		public DroneSaveData (Position _curPosition, Position _targePosition, 
			bool _isTravelling, bool _isBusy, bool _isLaser,
			Position _currentTaskPosition, List<InventoryItemSlot> _currentTaskMaterials,
			List<InventoryItemSlot> _myInv,
			int _droneState) {

			curPosition = _curPosition;
			targetPosition = _targePosition;

			isTravelling = _isTravelling;
			isBusy = _isBusy;
			isLaser = _isLaser;

			currentTaskPosition = _currentTaskPosition;
			currentTaskMaterials = InventoryData.ConvertToSaveData(_currentTaskMaterials);
			
			myInv = InventoryData.ConvertToSaveData(_myInv);

			droneState = _droneState;
		}
	}
	
	


	[System.Serializable]
	public class TileData {
		public int height = 0;
		public int material = 0;
		public int oreType = 0;
		public int oreAmount = 0;

		public TileData (int _height, int _material, int _oreType, int _oreAmount) {
			height = _height;
			material = _material;
			oreType = _oreType;
			oreAmount = _oreAmount;
		}
	}
	
	[System.Serializable]
	public class InventoryData {
		public string uniqueName = "";
		public int count = 0;
		public int maxCount = 0;
		public int type = 0;

		public InventoryData (InventoryItemSlot slot) {
			if (!slot.myItem.isEmpty())
				uniqueName = slot.myItem.uniqueName;
			else
				uniqueName = "";
			count = slot.count;
			maxCount = slot.maxCount;
			switch (slot.mySlotType) {
				case InventoryItemSlot.SlotType.input:
					type = 0;
					break;
				case InventoryItemSlot.SlotType.output:
					type = 1;
					break;
				case InventoryItemSlot.SlotType.storage:
					type = 2;
					break;
				case InventoryItemSlot.SlotType.house:
					type = 3;
					break;
				case InventoryItemSlot.SlotType.worker:
					type = 4;
					break;
				default:
					Debug.LogError("Unknown item slot type detected!");
					type = -1;
					break;
			}
		}
		
		public static InventoryData[] ConvertToSaveData (List<InventoryItemSlot> slots) {
			if (slots != null) {
				var invdata = new InventoryData[slots.Count];
				for (int i = 0; i < slots.Count; i++) {
					invdata[i] = new InventoryData(slots[i]);
				}

				return invdata;
			} else {
				return new InventoryData[0];
			}
		}

		public InventoryItemSlot ConvertToInvSlot() {
			var slot = InventoryItemSlot.SlotType.storage;
			switch (type) {
				case 0:
					slot = InventoryItemSlot.SlotType.input;
					break;
				case 1:
					slot = InventoryItemSlot.SlotType.output;
					break;
				case 2:
					slot = InventoryItemSlot.SlotType.storage;
					break;
				case 3:
					slot = InventoryItemSlot.SlotType.house;
					break;
				case 4:
					slot = InventoryItemSlot.SlotType.worker;
					break;
				default:
					Debug.LogError("Unknown item slot type detected!");
					return null;
					break;
			}

			return new InventoryItemSlot(DataHolder.s.GetItem(uniqueName), count, maxCount, slot);
		}
		
		public  static List<InventoryItemSlot> ConvertToRegularData (InventoryData[] slots) {
			var invdata = new List<InventoryItemSlot>();
			if (slots != null) {
				for (int i = 0; i < slots.Length; i++) {
					var slot = slots[i].ConvertToInvSlot();
					if(slot != null)
						invdata.Add(slot);
				}
			}

			return invdata;
		}
		
	}
}
