using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

/// <summary>
/// Deals with all the saving processes.
/// If a data is saved, it should be here.
/// </summary>
public class DataSaver : MonoBehaviour {

	public static DataSaver s;

	public SaveFile mySave;
	public const string saveName = "mySave.data";

	public List<BuildingSaveData> BuildingsToBeSaved = new List<BuildingSaveData>();
	public List<BeltSaveData> BeltsToBeSaved = new List<BeltSaveData>();
	public List<ConnectorSaveData> ConnectorsToBeSaved = new List<ConnectorSaveData>();
	public List<ConstructionSaveData> ConstructionsToBeSaved = new List<ConstructionSaveData>();
	public List<DroneSaveData> DronesToBeSaved = new List<DroneSaveData>();
	public string[] BuildingBarDataToBeSaved;
	public TileData[,] TileDataToBeSaved;

	public delegate void SaveYourself ();
	public event SaveYourself saveEvent;

	// Use this for initialization
	void Awake () {
		s = this;
	}

	void Start () {
		print("Save Location:" + Application.persistentDataPath);
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
		FileStream file = File.Create(Application.persistentDataPath + "/" + saveName);

		SaveFile data = new SaveFile(
			BuildingsToBeSaved.ToArray(),
			BeltsToBeSaved.ToArray(),
			ConnectorsToBeSaved.ToArray(),
			ConstructionsToBeSaved.ToArray(),
			DronesToBeSaved.ToArray(),
			BuildingBarDataToBeSaved, TileDataToBeSaved);

		bf.Serialize(file, data);
		file.Close();
		print("Data Saved to " + Application.persistentDataPath + "/");
	}

	public bool Load () {
		try {
			if (File.Exists(Application.persistentDataPath + "/" + saveName)) {
				BinaryFormatter bf = new BinaryFormatter();
				FileStream file = File.Open(Application.persistentDataPath + "/" + saveName, FileMode.Open);
				try {
					SaveFile data = (SaveFile)bf.Deserialize(file); 
					file.Close();

					mySave = data;
					print("Data Loaded");
				} catch (Exception e) {
					file.Close();
					throw;
				}
				return true;
			} else {
				print("No Data Found");
				return false;
			}
		} catch {
			File.Delete(Application.persistentDataPath + "/" + saveName );
			print("Corrupt Data Deleted");
			return false;
		}
	}

	public void DeleteSave () {
		DeleteSave(saveName);
	}

	public void DeleteSave(string filename) {
		File.Delete(Application.persistentDataPath + "/" + saveName);
	}

	[System.Serializable]
	public class SaveFile {
		public BuildingSaveData[] buildingData = new BuildingSaveData[0];
		public BeltSaveData[] beltData = new BeltSaveData[0];
		public ConnectorSaveData[] connectorData = new ConnectorSaveData[0];
		public ConstructionSaveData[] constructionData = new ConstructionSaveData[0];
		public DroneSaveData[] droneData = new DroneSaveData[0];
		public string[] buildingBarData = new string[0];
		public TileData[,] tileData = new TileData[0,0];

		public float[] newGameData = new float[0];

		public SaveFile (BuildingSaveData[] myit, BeltSaveData[] mybel, ConnectorSaveData[] mycon, ConstructionSaveData[] mycons, DroneSaveData[] mydron,  string[] mybuilbar, TileData[,] myTiledata) {
			buildingData = myit;
			beltData = mybel;
			connectorData = mycon;
			constructionData = mycons;
			droneData = mydron;
			buildingBarData = mybuilbar;
			tileData = myTiledata;
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

		public Position targetStorage;
		public Position constructionInventory;
		
		public DroneSaveData (Position _curPosition, Position _targePosition, 
			bool _isTravelling, bool _isBusy, bool _isLaser,
			Position _currentTaskPosition, List<InventoryItemSlot> _currentTaskMaterials,
			List<InventoryItemSlot> _myInv,
			int _droneState,
			Position _targetStorage,
			Position _constructionInventory) {

			curPosition = _curPosition;
			targetPosition = _targePosition;

			isTravelling = _isTravelling;
			isBusy = _isBusy;
			isLaser = _isLaser;

			currentTaskPosition = _currentTaskPosition;
			currentTaskMaterials = InventoryData.ConvertToSaveData(_currentTaskMaterials);
			
			myInv = InventoryData.ConvertToSaveData(_myInv);

			droneState = _droneState;

			targetStorage = _targetStorage;
			constructionInventory = _constructionInventory;
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
