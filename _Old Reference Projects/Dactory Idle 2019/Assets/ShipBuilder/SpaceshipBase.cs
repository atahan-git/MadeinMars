using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SpaceshipBase : MonoBehaviour {

	public bool isEditor = false;
	public string shipName = "default";
	public int maxX = 3;
	public int maxY = 3;
	public int blockSize = 1;
	public Vector3 topLeftCorner = new Vector3 (-1, 1, 0);
	public static SpaceshipBase sBase;
	[HideInInspector]
	public Spaceship myShip = new Spaceship();
	GameObject[] curShip;

	// Use this for initialization
	void Awake () {
		if(isEditor) sBase = this;
		int maxSlot = maxX * maxY;
		curShip = new GameObject[maxSlot];
		if (!Load ()) {
			myShip.shipBlocks = new AllBlocks.BlockTypes[maxSlot];
		}

		DrawShip ();
	
	}
	
	public void DrawShip(){
		Vector3 currentPos = topLeftCorner;
		int index = 1;
		foreach (AllBlocks.BlockTypes block in myShip.shipBlocks) {
			//destroy previous block
			if(curShip[index-1] != null)Destroy(curShip[index-1]);
			switch (block){
			case AllBlocks.BlockTypes.none:
				//do nothing! there is no block to draw
				break;
			case AllBlocks.BlockTypes.hull:
				//print (curShip[index-1]);
				curShip[index-1] = (GameObject)Instantiate(AllBlocks.blocks.hull, transform.TransformPoint(currentPos), transform.rotation);
				break;
			case AllBlocks.BlockTypes.gun:
				curShip[index-1] = (GameObject)Instantiate(AllBlocks.blocks.gun, transform.TransformPoint(currentPos), transform.rotation);
				break;
			}
			if(curShip[index-1] != null)curShip[index-1].transform.parent = transform;

			if(index%maxX != 0){
				currentPos.x += blockSize;
			}else{
				currentPos.x = topLeftCorner.x;
				currentPos.y -= blockSize;
			}
			index ++;
		}
	}

	public bool Load(){
		if (File.Exists (Application.persistentDataPath + "/" + shipName + ".banana")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/" + shipName + ".banana", FileMode.Open);
			Spaceship data = (Spaceship)bf.Deserialize (file);
			file.Close ();
			
			myShip = data;
			return true;
		} else {
			return false;
		}
	}
}


[Serializable]
public class Spaceship {
	public AllBlocks.BlockTypes[] shipBlocks;
}