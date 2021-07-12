using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Any placement of an object to the world should happen through this singleton
/// Also handles placement checks
/// See Player_BuildingController for seeing how the player puts down the buildings. This component just deals with the busywork of the placement.
/// </summary>
public class FactoryPlayerConnector : MonoBehaviour
{
    public static FactoryPlayerConnector s;
	public GameObject buidingWorldObjectPrefab;
	public GameObject beltWorldObjectPrefab;
	public GameObject connectorWorldObjectPrefab;


	private void Awake () {
		if (s != null) {
			Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
		}
		s = this;
		
		//GameLoader.CallWhenLoadedEarly(LoadFromSave);
	}

	/*void LoadFromSave () {
		if (DataSaver.s.mySave != null) {
			foreach (DataSaver.s.BeltSaveData belt in DataSaver.s.mySave.beltData) {
				if (belt != null)
					BuildBeltFromSave(belt.center, belt.cardinalDirection, belt.isBuilt, belt.myInvConverted());
			}

			foreach (DataSaver.s.BuildingSaveData building in DataSaver.s.mySave.buildingData) {
				if (building != null)
					BuildObjectFromSave(building.myUniqueName, building.center, building.isBuilt, building.myInvConverted());
			}
			
			
			foreach (DataSaver.s.ConnectorSaveData connector in DataSaver.s.mySave.connectorData) {
				if (connector != null)
					BuildConnectorFromSave(connector.center, connector.myDir, connector.isBuilt, connector.myInvConverted());
			}
		}
	}*/

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
	/*bool BuildObjectFromSave (string myUniqueName, Position center, bool isBuilt, List<InventoryItemSlot> inventoryItemSlots) {
		BuildingData dat = DataHolder.s.GetBuilding(myUniqueName);
		if (dat != null)
			return BuildObject(DataHolder.s.GetBuilding(myUniqueName), center, false, false, inventoryItemSlots, isBuilt);
		else
			return false;
	}*/

	public bool BuildConnector (Position location, int direction, bool isBuilt) {
		return _BuildConnector(location, direction, false, isBuilt);
	}
	
	/*
	bool BuildConnectorFromSave (Position center, int direction, bool isBuilt, List<InventoryItemSlot> inventoryItemSlots) {
		if (direction > 2)
			direction = 0;
		return _BuildConnector(Grid.s.GetTile(center), direction, false, isBuilt, inventoryItemSlots);
	}*/
	
	bool _BuildConnector (Position location, int direction, bool forced, bool isBuilt) {
		if (CheckPlaceable(location) || forced) {
			
			if (!isBuilt) {
				FactoryBuilder.StartConstruction(FactoryMaster.s.connectorBuildingData, direction, location);
			} else {
				FactoryBuilder.CreateConnector(location, direction);
			}

			return true;
		} else {
			return false;
		}
	}
	
	public bool BuildBelt (Position location, int direction, bool isBuilt, List<InventoryItemSlot> inventory = null) {
		return _BuildBelt(location, direction, false, isBuilt, inventory);
	}
	
	/*bool BuildBeltFromSave (Position center, int direction, bool isBuilt, List<InventoryItemSlot> inventoryItemSlots) {
		return _BuildBelt(Grid.s.GetTile(center), direction, false, isBuilt, inventoryItemSlots);
	}*/
	
	
	 
	bool _BuildBelt (Position location, int direction, bool forced, bool isBuilt, List<InventoryItemSlot> inventory) {
		if (CheckPlaceable(location) || forced) {
			
			if (!isBuilt) {
				FactoryBuilder.StartConstruction(FactoryMaster.s.beltBuildingData, direction, location, inventory);
			} else {
				FactoryBuilder.CreateBelt(location, direction, inventory);
			}
			
			return true;
		} else {
			return false;
		}
	}

	public bool BuildObject(BuildingData myData, Position location, bool forced, bool rocketBuild, bool isBuilt, List<InventoryItemSlot> inventory = null) {
		return _BuildObject(myData, location, forced, rocketBuild, isBuilt, inventory);
	}

	bool _BuildObject (BuildingData myData, Position location, bool forced, bool rocketBuild, bool isBuilt, List<InventoryItemSlot> inventory) {

		if (rocketBuild) {
			isBuilt = true;
		}
		
		if (CheckPlaceable(myData, location) || forced) {
			
			if (!isBuilt) {
				FactoryBuilder.StartConstruction(myData, 0, location, inventory);
			} else {
				FactoryBuilder.CreateBuilding(myData, location,  inventory);
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
}
