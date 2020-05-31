using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class DataSaver : MonoBehaviour {

	public delegate void InstantiationUpdate ();
	public static event InstantiationUpdate beltEvent;

	public static DataSaver s;

	SaveFile mySave;
	string saveName = "mySave";

	public static ItemData[] ItemsToBeSaved = new ItemData[99];
	public static int n = 0;
	public static BeltData[] BeltsToBeSaved = new BeltData[999];
	public static int b = 0;


	public GameObject[] itemPrefabs;
	public GameObject beltPrefab;

	public delegate void SaveStuff ();
	public static event SaveStuff saveEvent;

	// Use this for initialization
	void Awake () {
		s = this;

	}
	void Start () {
		print(Application.persistentDataPath);
		n = 0;
		if (Load()) {
			InstantiateItems();
			InstantiateBelts();
		}
	}

	//--------------------------------------------------------------------------------------------------------------------------------
	int itemID = 0;
	int beltID = 0;
	void InstantiateItems () {
		foreach (ItemData myItem in mySave.itemData) {
			if (myItem != null) {
				if (myItem.type != -1) {
					ItemBaseScript reelItem = ((GameObject)Instantiate(itemPrefabs[myItem.type], transform.position, transform.rotation)).GetComponent<ItemBaseScript>();
					reelItem.PlaceSelf(myItem.x, myItem.y);
					reelItem.gameObject.name = reelItem.gameObject.name + " " + itemID;
					itemID++;
				}
			}
		}
	}

	void InstantiateBelts () {
		foreach (BeltData myBelt in mySave.beltData) {
			if (myBelt != null) {
				if (myBelt.x != -1) {
					BeltScript reelBelt = ((GameObject)Instantiate(beltPrefab, transform.position, transform.rotation)).GetComponent<BeltScript>();
					reelBelt.PlaceSelf(myBelt.x, myBelt.y, myBelt.inLocations, myBelt.outLocations);
					reelBelt.gameObject.name = reelBelt.gameObject.name + " " + beltID;
					beltID++;
				}
			}
		}

		if (beltEvent != null)
			beltEvent();
	}


	//--------------------------------------------------------------------------------------------------------------------------------

	void OnApplicationQuit () {
		if (saveEvent != null)
			saveEvent();
		Save();
	}

	public void Save () {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/" + saveName + ".banana");

		SaveFile data = new SaveFile(ItemsToBeSaved, BeltsToBeSaved);

		bf.Serialize(file, data);
		file.Close();
		print("Data Saved");
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
		public ItemData[] itemData;
		public BeltData[] beltData;

		public SaveFile (ItemData[] myit, BeltData[] mybel) {
			itemData = myit;
			beltData = mybel;
		}

	}

	[System.Serializable]
	public class ItemData {
		public int type = -1;
		public int x = -1;
		public int y = -1;

		public ItemData (int myType, int myX, int myY) {
			type = myType;
			x = myX;
			y = myY;
		}
	}

	[System.Serializable]
	public class BeltData {
		public int x = -1;
		public int y = -1;
		public bool[] inLocations = new bool[4];
		public bool[] outLocations = new bool[4];

		public BeltData (int myX, int myY, bool[] myins, bool[] myouts) {
			x = myX;
			y = myY;
			inLocations = myins;
			outLocations = myouts;

		}
	}
}
