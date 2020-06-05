using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class SpaceshipBuilder : MonoBehaviour {

	public string editShipName = "default";
	public AllBlocks.BlockTypes selectedType;
	public Image[] myButtons;

	void Start (){
		ChangeBlockType (0);
	}

	public void SetBlock(int slot){

		SpaceshipBase.sBase.myShip.shipBlocks [slot] = selectedType;
		Save ();
		//redraw the ship
		SpaceshipBase.sBase.DrawShip ();


	}

	public void ChangeBlockType(int type){
		switch (type) {
		case 0:
			selectedType = AllBlocks.BlockTypes.none;
			break;
		case 1:
			selectedType = AllBlocks.BlockTypes.hull;
			break;
		case 2:
			selectedType = AllBlocks.BlockTypes.gun;
			break;
		}

		//make all buttons white
		foreach(Image image in myButtons){
			image.color = Color.white;
		}
		myButtons [type].color = Color.green;//make selected button green
	}

	public void Save (){
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/" + editShipName + ".banana");

		Spaceship data = SpaceshipBase.sBase.myShip;

		bf.Serialize (file, data);
		file.Close ();
	}

	public bool Load(){
		if (File.Exists (Application.persistentDataPath + "/" + editShipName + ".banana")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/" + editShipName + ".banana", FileMode.Open);
			Spaceship data = (Spaceship)bf.Deserialize (file);
			file.Close ();

			SpaceshipBase.sBase.myShip = data;
			return true;
		} else {
			return false;
		}
	}
}