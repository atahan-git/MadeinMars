using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class DataSaver : MonoBehaviour {

	public static DataSaver s;

	public static SaveFile mySave;
	string saveName = "mySave";

	public static BuildingSaveData[] ItemsToBeSaved = new BuildingSaveData[99];
	public static int n = 0;
	public static BeltData[] BeltsToBeSaved = new BeltData[999];
	public static int b = 0;
	public static string[] BuildingBarDataToBeSaved;

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
	
	public void SaveGame () {
		saveEvent?.Invoke();
		Save();
	}

	void Save () {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/" + saveName + ".banana");

		SaveFile data = new SaveFile(ItemsToBeSaved, BeltsToBeSaved, BuildingBarDataToBeSaved);

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

	[System.Serializable]
	public class SaveFile {
		public BuildingSaveData[] buildingData = new BuildingSaveData[0];
		public BeltData[] beltData = new BeltData[0];
		public string[] buildingBarData = new string[0];

		public SaveFile (BuildingSaveData[] myit, BeltData[] mybel, string[] mybuilbar) {
			buildingData = myit;
			beltData = mybel;
			buildingBarData = mybuilbar;
		}

	}

	[System.Serializable]
	public class BuildingSaveData {
		public string myUniqueName;
		public Position myPos;

		public BuildingSaveData (string _myUniqueName, Position location) {
			myUniqueName = _myUniqueName;
			myPos = location;
		}
	}

	[System.Serializable]
	public class BeltData {
		public bool[] inLocations = new bool[4];
		public bool[] outLocations = new bool[4];
		public Position myPos;
		public bool isBuildingBelt = false;

		public BeltData (bool[] myins, bool[] myouts, Position location, bool amIBuildingBelt) {
			inLocations = myins;
			outLocations = myouts;
			myPos = location;
			isBuildingBelt = amIBuildingBelt;
		}
	}
}
