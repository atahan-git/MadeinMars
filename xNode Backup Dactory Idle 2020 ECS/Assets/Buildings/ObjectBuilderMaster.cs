using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Any placement of an object to the world should happen through this singleton
/// Also handles placement checks
/// </summary>
public class ObjectBuilderMaster : MonoBehaviour
{
	static ObjectBuilderMaster s;
	public GameObject buidingWorldObjectPrefab;
	public GameObject beltPrefab;
	public GameObject buildingBeltPrefab;

	public BuildingData _beltBuildingData;
	public static BuildingData beltBuildingData;

	private void Awake () {
		if (s != null) {
			Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
		}
		s = this;
		beltBuildingData = _beltBuildingData;
		
		GameLoader.CallWhenLoaded(LoadFromSave);
	}

	void LoadFromSave () {
		if (DataSaver.mySave != null) {
			foreach (DataSaver.BeltData belt in DataSaver.mySave.beltData) {
				if (belt != null)
					BuildBeltFromSave(belt.inLocations, belt.outLocations, belt.myPos, belt.isBuildingBelt);
			}

			foreach (DataSaver.BuildingSaveData building in DataSaver.mySave.buildingData) {
				if (building != null)
					BuildObjectFromSave(building.myUniqueName, building.myPos);
			}
		}
	}

	public static bool CheckPlaceable (Position location) {
		try {
			return Grid.s.GetTile(location).buildingPlaceable && Grid.s.GetTile(location).isEmpty;
		} catch (IndexOutOfRangeException e) {
			return false;
		}
	}

	public static bool CheckPlaceable (BuildingData myData, Position location) {
		for (int y = 0; y < myData.shape.column.Length; y++) {
			for (int x = 0; x < myData.shape.column[y].row.Length; x++) {
				if (myData.shape.column[y].row[x]) {
					if (!CheckPlaceable(new Position(x,y) + location - BuildingData.center)) {
						return false;
					}
				}
			}
		}
		return true;
	}
	static bool BuildObjectFromSave (string myUniqueName, Position location) {
		BuildingData dat = DataHolder.s.GetBuilding(myUniqueName);
		if (dat != null)
			return BuildObject(DataHolder.s.GetBuilding(myUniqueName), location, true);
		else
			return false;
	}

	

	public static BeltObject BuildBelt (TileData tileS) {
		return _BuildBelt(tileS, false);
	}
	
	public static BeltObject BuildBelt (TileData tileS, bool isBuildingBelt) {
		return _BuildBelt(tileS, isBuildingBelt);
	}

	static bool BuildBeltFromSave (bool[] beltInData, bool[] beltOutData, Position location, bool isBuildingBelt) {
		if (CheckPlaceable(location)) {
			BeltObject myBelt = BuildBelt(Grid.s.GetTile(location), isBuildingBelt);
			myBelt.beltInputs = beltInData;
			myBelt.beltOutputs = beltOutData;
			BeltMaster.s.AddOneBeltFromSave(myBelt);

			return true;
		} else {
			return false;
		}
	}
	
	static BeltObject _BuildBelt (TileData tileS, bool isBuildingBelt) {
		GameObject prefab = isBuildingBelt ? s.buildingBeltPrefab : s.beltPrefab;

		BeltObject myBelt = Instantiate(prefab, tileS.position.Vector3(Position.Type.belt) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity).GetComponent<BeltObject>();
		myBelt.gameObject.name = tileS.position.ToString() + " - " + myBelt.gameObject.name;
		myBelt.pos = tileS.position;
		tileS.myBelt = myBelt.gameObject;
		myBelt.tileCovered = tileS;
		myBelt.myData = beltBuildingData;

		DataSaver.saveEvent += myBelt.SaveYourself;
		return myBelt;
	}

	public static bool BuildObject (BuildingData myData, Position location) {
		return BuildObject(myData, location, false);
	}

	public static bool BuildObject(BuildingData myData, Position location, bool forced) {
		return BuildObject(myData, location, forced, false);
	}
	
	public static bool BuildObject(BuildingData myData, Position location, bool forced, bool spaceLandingBuild) {
		return _BuildObject(myData, location, forced, spaceLandingBuild);
	}

	static bool _BuildObject (BuildingData myData, Position location, bool forced, bool spaceLandingBuild) {

		if (CheckPlaceable(myData, location) || forced) {
			GameObject InstantiatedItem = Instantiate(s.buidingWorldObjectPrefab, location.Vector3(Position.Type.building), Quaternion.identity);

			List<TileData> coveredTiles = new List<TileData>();
			List<BeltBuildingObject> buildingBelts = new List<BeltBuildingObject>();
			for (int y = 0; y < myData.shape.column.Length; y++) {
				for (int x = 0; x < myData.shape.column[y].row.Length; x++) {
					if (myData.shape.column[y].row[x]) {
						TileData myTile = Grid.s.GetTile(new Position(x, y) + location - BuildingData.center);
						//print(new Position(x, y) + location - BuildingData.center);
						//myTile.itemPlaceable = false;
						myTile.myBuilding = InstantiatedItem;
						coveredTiles.Add(myTile);


						GameObject InstantiatedBuildingBelt;
						if (myTile.areThereBelt) {
							if (myTile.myBelt.GetComponent<BeltBuildingObject>()) {
								InstantiatedBuildingBelt = myTile.myBelt.gameObject;
							} else {
								myTile.myBelt.GetComponent<BeltObject>().DestroyYourself();
								InstantiatedBuildingBelt = BuildBelt(myTile, s.buildingBeltPrefab).gameObject;
							}
						} else {
							InstantiatedBuildingBelt = BuildBelt(myTile, s.buildingBeltPrefab).gameObject;
						}

						buildingBelts.Add(InstantiatedBuildingBelt.GetComponent<BeltBuildingObject>());
					}
				}
			}

			InstantiatedItem.GetComponent<BuildingWorldObject>().PlaceInWorld(myData, location, coveredTiles, buildingBelts);
			InstantiatedItem.gameObject.name = Grid.s.GetTile(location).position.ToString() + " - " + InstantiatedItem.gameObject.name; 

			return true;
		} else {
			Debug.LogError(string.Format("A building of type {0} was tried to be built on {1}, " +
			                             "but this was not possible. This should have been caught by the player_building controller, " +
			                             "or shouldn't be able to saved like this!", 
				myData.myType, location.ToString()));
			return false;
		}
	}
}
