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
	public static ObjectBuilderMaster s;
	private void Awake () {
		if (s != null) {
			Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
		}
		s = this;
	}

	public GameObject buidingWorldObjectPrefab;
	public static bool CheckPlaceable (Position location) {
		try {
			return Grid.s.myTiles[location.x, location.y].itemPlaceable && !Grid.s.myTiles[location.x, location.y].areThereItem;
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
	public static bool BuildObject (string myUniqueName, Position location) {
		return BuildObject(DataHolder.s.GetBuilding(myUniqueName), location);
	}

	public static bool BuildObject (BuildingData myData, Position location) {

		if (CheckPlaceable(myData, location)) {
			GameObject InstantiatedItem = Instantiate(s.buidingWorldObjectPrefab, location.vector3, Quaternion.identity);

			List<TileBaseScript> coveredTiles = new List<TileBaseScript>();
			for(int y = 0; y < myData.shape.rows.Length; y++) {
				for (int x = 0; x < myData.shape.rows[y].row.Length; x++) {
					if (myData.shape.rows[y].row[x]) {
						TileBaseScript myTile = Grid.s.myTiles[x+location.x - BuildingData.center.x, y+location.y - BuildingData.center.y];
						//print(new Position(x, y) + location - BuildingData.center);
						//myTile.itemPlaceable = false;
						myTile.areThereItem = true;
						myTile.myItem = InstantiatedItem;
						coveredTiles.Add(myTile);
					}
				}
			}

			InstantiatedItem.GetComponent<BuildingWorldObject>().PlaceInWorld(myData, location, coveredTiles);

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
