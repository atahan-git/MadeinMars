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

	public static SaveFile mySave;
	public const string saveName = "mySave";

	public static List<BuildingSaveData> ItemsToBeSaved = new List<BuildingSaveData>();
	public static List<BeltData> BeltsToBeSaved = new List<BeltData>();
	public static List<ConnectorData> ConnectorsToBeSaved = new List<ConnectorData>();
	public static string[] BuildingBarDataToBeSaved;
	public static TileData[,] TileDataToBeSaved;
	public static InventoryData[] InventoryDataToBeSaved;

	public delegate void SaveYourself ();
	public static event SaveYourself saveEvent;

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
		FileStream file = File.Create(Application.persistentDataPath + "/" + saveName + ".banana");

		SaveFile data = new SaveFile(ItemsToBeSaved.ToArray(), BeltsToBeSaved.ToArray(),ConnectorsToBeSaved.ToArray(), BuildingBarDataToBeSaved, TileDataToBeSaved, InventoryDataToBeSaved);

		bf.Serialize(file, data);
		file.Close();
		print("Data Saved to " + Application.persistentDataPath + "/");
	}

	public bool Load () {
		try {
			if (File.Exists(Application.persistentDataPath + "/" + saveName + ".banana")) {
				BinaryFormatter bf = new BinaryFormatter();
				FileStream file = File.Open(Application.persistentDataPath + "/" + saveName + ".banana", FileMode.Open);
				SaveFile data = (SaveFile)bf.Deserialize(file);
				file.Close();

				mySave = data;
				print("Data Loaded");
				return true;
			} else {
				print("No Data Found");
				return false;
			}
		} catch {
			File.Delete(Application.persistentDataPath + "/" + saveName + ".banana");
			print("Corrupt Data Deleted");
			return false;
		}
	}

	public void DeleteSave () {
		DeleteSave(saveName);
	}

	public static void DeleteSave(string filename) {
		File.Delete(Application.persistentDataPath + "/" + saveName + ".banana");
	}

	[System.Serializable]
	public class SaveFile {
		public BuildingSaveData[] buildingData = new BuildingSaveData[0];
		public BeltData[] beltData = new BeltData[0];
		public ConnectorData[] connectorData = new ConnectorData[0];
		public string[] buildingBarData = new string[0];
		public TileData[,] tileData = new TileData[0,0];
		public InventoryData[] inventoryData = new InventoryData[0];

		public SaveFile (BuildingSaveData[] myit, BeltData[] mybel, ConnectorData[] mycon, string[] mybuilbar, TileData[,] myTiledata, InventoryData[] myInventoryData) {
			buildingData = myit;
			beltData = mybel;
			connectorData = mycon;
			buildingBarData = mybuilbar;
			tileData = myTiledata;
			inventoryData = myInventoryData;
		}

	}

	[System.Serializable]
	public class BuildingSaveData {
		public string myUniqueName;
		public Position myPos;
		public bool isBuilt;

		public BuildingSaveData (string _myUniqueName, Position location, bool _isBuilt) {
			myUniqueName = _myUniqueName;
			myPos = location;
			isBuilt = _isBuilt;
		}
	}

	[System.Serializable]
	public class BeltData {
		public Position myPos;
		public int cardinalDirection;
		public bool isBuilt;
		
		public BeltData (Position location, int myDirection, bool _isBuilt) {
			myPos = location;
			cardinalDirection = myDirection;
			isBuilt = _isBuilt;
		}
	}
	
	[System.Serializable]
	public class ConnectorData {
		public Position myPos;
		public int myDir;
		public bool isBuilt;
		public ConnectorData (Position location, int direction, bool _isBuilt) {
			myPos = location;
			myDir = direction;
			isBuilt = _isBuilt;
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

		public InventoryData (string _uniqueName, int _count) {
			uniqueName = _uniqueName;
			count = _count;
		}
	}
}
