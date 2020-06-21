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

	private void Awake () {
		if (s != null) {
			Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
		}
		s = this;
	}

	public static bool CheckPlaceable (Position location) {
		try {
			return Grid.s.GetTile(location).itemPlaceable && Grid.s.GetTile(location).isEmpty;
		} catch (IndexOutOfRangeException e) {
			return false;
		}
	}

	public static bool CheckPlaceable (BuildingData myData, Position location) {
		for (int y = 0; y < myData.shape.rows.Length; y++) {
			for (int x = 0; x < myData.shape.rows[y].row.Length; x++) {
				if (myData.shape.rows[y].row[x]) {
					if (!CheckPlaceable(new Position(x,y) + location - BuildingData.center)) {
						return false;
					}
				}
			}
		}
		return true;
	}
	public static bool BuildObjectFromSave (string myUniqueName, Position location) {
		return BuildObject(DataHolder.s.GetBuilding(myUniqueName), location, true);
	}

	static BeltObject BuildBelt (TileBaseScript tileS, bool isBuildingBelt) {
		GameObject prefab = isBuildingBelt ? s.buildingBeltPrefab : s.beltPrefab;

		BeltObject myBelt = ((GameObject)Instantiate(prefab, tileS.position.Vector3(Position.Type.belt) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity)).GetComponent<BeltObject>();
		myBelt.gameObject.name = tileS.position.ToString() + " - " + myBelt.gameObject.name;
		myBelt.pos = tileS.position;
		tileS.myBelt = myBelt.gameObject;
		myBelt.tileCovered = tileS;

		DataSaver.saveEvent += myBelt.SaveYourself;
		return myBelt;
	}

	public static BeltObject BuildBelt (TileBaseScript tileS) {
		return BuildBelt(tileS, false);
	}

	public static bool BuildBeltFromSave (bool[] beltInData, bool[] beltOutData, Position location, bool isBuildingBelt) {
		if (CheckPlaceable(location)) {
			BeltObject myBelt = BuildBelt(Grid.s.GetTile(location), isBuildingBelt);
			myBelt.beltInputs = beltInData;
			myBelt.beltOutputs = beltOutData;

			return true;
		} else {
			return false;
		}
	}

	public static bool BuildObject (BuildingData myData, Position location) {
		return BuildObject(myData, location, false);
	}

	public static bool BuildObject (BuildingData myData, Position location, bool forced) {

		if (CheckPlaceable(myData, location) || forced) {
			GameObject InstantiatedItem = Instantiate(s.buidingWorldObjectPrefab, location.Vector3(Position.Type.building), Quaternion.identity);

			List<TileBaseScript> coveredTiles = new List<TileBaseScript>();
			List<BeltBuildingObject> buildingBelts = new List<BeltBuildingObject>();
			for (int y = 0; y < myData.shape.rows.Length; y++) {
				for (int x = 0; x < myData.shape.rows[y].row.Length; x++) {
					if (myData.shape.rows[y].row[x]) {
						TileBaseScript myTile = Grid.s.GetTile(new Position(x, y) + location - BuildingData.center);
						//print(new Position(x, y) + location - BuildingData.center);
						//myTile.itemPlaceable = false;
						myTile.myBuilding = InstantiatedItem;
						coveredTiles.Add(myTile);

						GameObject InstantiatedBuildingBelt = BuildBelt(myTile, s.buildingBeltPrefab).gameObject;

						buildingBelts.Add(InstantiatedBuildingBelt.GetComponent<BeltBuildingObject>());
					}
				}
			}

			InstantiatedItem.GetComponent<BuildingWorldObject>().PlaceInWorld(myData, location, coveredTiles, buildingBelts);

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
