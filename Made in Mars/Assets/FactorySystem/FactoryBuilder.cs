using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Any placement of an object to the world should happen through this singleton
/// Also handles placement checks
/// See Player_BuildingController for seeing how the player puts down the buildings. This component just deals with the busywork of the placement.
/// </summary>
public class FactoryBuilder : MonoBehaviour
{
    public static FactoryBuilder s;
	public GameObject buidingWorldObjectPrefab;
	public GameObject beltWorldObjectPrefab;
	public GameObject connectorWorldObjectPrefab;

	public BuildingData beltBuildingData;
	public BuildingData connectorBuildingData;

	private void Awake () {
		if (s != null) {
			Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
		}
		s = this;
		
		GameLoader.CallWhenLoaded(LoadFromSave);
	}

	void LoadFromSave () {
		if (DataSaver.mySave != null) {
			foreach (DataSaver.BeltData belt in DataSaver.mySave.beltData) {
				if (belt != null)
					BuildBeltFromSave(belt.myPos, belt.cardinalDirection, belt.isBuilt, belt.myInvConverted());
			}

			foreach (DataSaver.BuildingSaveData building in DataSaver.mySave.buildingData) {
				if (building != null)
					BuildObjectFromSave(building.myUniqueName, building.myPos, building.isBuilt, building.myInvConverted());
			}
			
			
			foreach (DataSaver.ConnectorData connector in DataSaver.mySave.connectorData) {
				if (connector != null)
					BuildConnectorFromSave(connector.myPos, connector.myDir, connector.isBuilt, connector.myInvConverted());
			}
		}
	}

	public bool CheckPlaceable (Position location) {
		try {
			return Grid.s.GetTile(location).buildingPlaceable && Grid.s.GetTile(location).isEmpty;
		} catch (IndexOutOfRangeException e) {
			return false;
		}
	}

	public bool CheckPlaceable (BuildingData myData, Position location) {
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
	bool BuildObjectFromSave (string myUniqueName, Position location, bool isBuilt, List<InventoryItemSlot> inventory) {
		BuildingData dat = DataHolder.s.GetBuilding(myUniqueName);
		if (dat != null)
			return BuildObject(DataHolder.s.GetBuilding(myUniqueName), location, false, false, inventory, isBuilt);
		else
			return false;
	}

	public bool BuildConnector (TileData tileS, int direction, bool isBuilt, List<InventoryItemSlot> inventory) {
		return _BuildConnector(tileS, direction, false, isBuilt, inventory);
	}
	
	bool BuildConnectorFromSave (Position location, int direction, bool isBuilt, List<InventoryItemSlot> inventory) {
		if (direction > 2)
			direction = 0;
		return _BuildConnector(Grid.s.GetTile(location), direction, false, isBuilt, inventory);
	}
	
	bool _BuildConnector (TileData tileS, int direction, bool forced, bool isBuilt, List<InventoryItemSlot> inventory) {
		if (CheckPlaceable(tileS.position) || forced) {
			GameObject prefab = s.connectorWorldObjectPrefab;

			var instantiatedItem = Instantiate(prefab);

			instantiatedItem.GetComponent<ConnectorWorldObject>().PlaceInWorld(direction, tileS.position, tileS, false, isBuilt, inventory);
			
			if (!isBuilt) {
				DroneSystem.s.AddDroneBuildTask(tileS.position, connectorBuildingData);
			}

			return true;
		} else {
			return false;
		}
	}
	public bool BuildBelt (TileData tileS, int direction, bool isBuilt, List<InventoryItemSlot> inventory) {
		return _BuildBelt(tileS, direction, false, isBuilt, inventory);
	}
	
	bool BuildBeltFromSave (Position location, int direction, bool isBuilt, List<InventoryItemSlot> inventory) {
		return _BuildBelt(Grid.s.GetTile(location), direction, false, isBuilt, inventory);
	}
	
	
	 
	bool _BuildBelt (TileData tileS, int direction, bool forced, bool isBuilt, List<InventoryItemSlot> inventory) {
		if (CheckPlaceable(tileS.position) || forced) {
			GameObject prefab = s.beltWorldObjectPrefab;

			var instantiatedItem = Instantiate(prefab);

			instantiatedItem.GetComponent<BeltWorldObject>().PlaceInWorld(direction, tileS.position, tileS, false, isBuilt,  inventory);

			if (!isBuilt) {
				DroneSystem.s.AddDroneBuildTask(tileS.position, beltBuildingData);
			}
			
			return true;
		} else {
			return false;
		}
	}

	public bool BuildObject(BuildingData myData, Position location, bool forced, bool isBuilt) {
		return BuildObject(myData, location, forced, false, null, isBuilt);
	}
	
	public bool BuildObject(BuildingData myData, Position location, bool forced, bool spaceLandingBuild, List<InventoryItemSlot> inventory, bool isBuilt) {
		return _BuildObject(myData, location, forced, spaceLandingBuild, inventory, isBuilt);
	}

	bool _BuildObject (BuildingData myData, Position location, bool forced, bool spaceLandingBuild, List<InventoryItemSlot> inventory, bool isBuilt) {

		if (CheckPlaceable(myData, location) || forced) {
			
			List<Position> coveredPositions = new List<Position>();
			for (int y = 0; y < myData.shape.column.Length; y++) {
				for (int x = 0; x < myData.shape.column[y].row.Length; x++) {
					if (myData.shape.column[y].row[x]) {
						var pos = new Position(x, y) + location - BuildingData.center;
						//print(new Position(x, y) + location - BuildingData.center);
						//myTile.itemPlaceable = false;
						coveredPositions.Add(pos);
					}
				}
			}
			
			GameObject instantiatedItem = Instantiate(s.buidingWorldObjectPrefab);
			instantiatedItem.GetComponent<BuildingWorldObject>().PlaceInWorld(myData,location, coveredPositions, spaceLandingBuild, inventory, isBuilt);
			instantiatedItem.gameObject.name = Grid.s.GetTile(location).position.ToString() + " - " + instantiatedItem.gameObject.name;
			
			if (!isBuilt) {
				DroneSystem.s.AddDroneBuildTask(coveredPositions[0], myData);
			}

			return true;
		} else {
			Debug.LogError(string.Format("A building of type {0} was tried to be built on {1}, " +
			                             "but this was not possible. This should have been caught by the player_building controller, " +
			                             "or shouldn't be able to saved like this!", 
				myData.myType, location.ToString()));
			return false;
		}
	}

	public void OnDestroy() {
		GameLoader.RemoveFromCall(LoadFromSave);
	}
}
