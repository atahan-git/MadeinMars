using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Used just as a stroge object. Should have no functionality of its own
/// </summary>
public class BuildingWorldObject : MonoBehaviour
{

	public BuildingData myData;
	public Position myPos;
	public List<TileData> myTiles;
	public List<BeltBuildingObject> myBelts;
	public BuildingCraftingController myCrafter;

	public SpriteRenderer myRend;

	public void PlaceInWorld (BuildingData _myData, Position _location, List<TileData> _myTiles, List<BeltBuildingObject> _buildingBelts) {
		myData = _myData;
		myPos = _location;
		myTiles = _myTiles;
		myBelts = _buildingBelts;
		myCrafter.myBelts = myBelts;

		myRend.sprite = myData.BuildingSprite;
		DataSaver.saveEvent += SaveYourself;
		transform.position = _location.Vector3(Position.Type.building) + myData.spriteOffset.vector3();
		BuildingMaster.myBuildings.Add(myCrafter);
		myCrafter.SetUpCraftingProcesses(myData);
	}


	void SaveYourself () {
		DataSaver.ItemsToBeSaved[DataSaver.n] = new DataSaver.BuildingSaveData(myData.uniqueName, myPos);
		DataSaver.n++;
	}

	void OnDestroy () {
		DataSaver.saveEvent -= SaveYourself;
	}

	public void DestroyYourself () {
		foreach (TileData myTile in myTiles) {
			if (myTile != null)
				myTile.myBuilding = null;
		}
		foreach (BeltBuildingObject myBelt in myBelts) {
			if (myBelt != null)
				myBelt.DestroyYourself();
		}
		BuildingMaster.myBuildings.Remove(myCrafter);
		Destroy(gameObject);
	}

	
}
