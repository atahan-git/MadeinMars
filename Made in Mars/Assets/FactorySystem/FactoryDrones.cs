using System;
using System.Collections.Generic;
using UnityEngine;
using DroneState = Drone.DroneState;
using Random = UnityEngine.Random;

public class FactoryDrones {
    public const int DronesSlowFactor = 2;
    public static float DroneUpdatePerSecond = FactoryMaster.SimUpdatePerSecond / DronesSlowFactor;
    public const int DroneSpeedPerTick = 3;
    public static float DroneWorldSpeed = DroneSpeedPerTick*DroneUpdatePerSecond;
    
    public static List<BuildingInventoryController> storageBuildings = new List<BuildingInventoryController>();
    
    public static void RegisterStorageBuilding(BuildingInventoryController controller) {
	    storageBuildings.Add(controller);
    }
	
	
    public static void RemoveStorageBuilding(BuildingInventoryController controller) {
	    storageBuildings.Remove(controller);
    }

    public static void UpdateConstructionTaskModeMidTask(Construction construction) {
	    if (construction.isAssignedToDrone) {
		    for (int i = 0; i < FactoryMaster.s.GetDrones().Count; i++) {
			    var myDrone = FactoryMaster.s.GetDrones()[i];
			    if (!myDrone.isBusy) {
				    if (myDrone.currentTask != null) {
					    if (myDrone.currentTask.location == construction.center) {
						    myDrone.isLaser = false;
						    myDrone.myState = DroneState.SearchingToEmptyInventory;
					    }
				    }
			    } 
		    }
	    }
    }


    public static void UpdateTasks() {
	    for (int i = 0; i < FactoryMaster.s.GetDrones().Count; i++) {
		    var myDrone = FactoryMaster.s.GetDrones()[i];
		    for (int m = 0; m < FactoryMaster.s.GetConstructions().Count; m++) {
			    if (!myDrone.isBusy) {
				    var myCons = FactoryMaster.s.GetConstructions()[m];
				    if (!myCons.isAssignedToDrone) {
					    myCons.isAssignedToDrone = true;
					    myDrone.currentTask = new DroneTask(myCons);
					    myDrone.isBusy = true;
					    if (myDrone.currentTask.myType == DroneTask.DroneTaskType.build) {
						    myDrone.myState = DroneState.BeginBuildingTask;
					    } else {
						    myDrone.myState = DroneState.BeingDestructionTask;
					    }
					    break;
				    }
			    }

		    }
	    }
    }

    public static void DroneStartTravelToLocation(Drone drone, Position target) {
	    drone.isTravelling = true;
	    drone.targetPosition = target;
    }

    public static void UpdateDrone(Drone drone) {
        // If at any point our building disappears (due to player destroying it), then we will try to empty our inventory to get idle
		if (drone.isBusy 
		    && drone.myState != DroneState.TravellingToEmptyInventory 
		    && drone.myState != DroneState.EmptyingInventory 
		    && drone.myState != DroneState.SearchingToEmptyInventory) {

			if (drone.currentTask != null) {
				if (!drone.currentTask.location.isValid() || !Grid.s.GetTile(drone.currentTask.location).areThereConstruction) {
					drone.myState = DroneState.SearchingToEmptyInventory;
					drone.isLaser = false;
				}
			} else {
				drone.myState = DroneState.SearchingToEmptyInventory;
				drone.isLaser = false;
			}
		}

		if (drone.isTravelling) {
			drone.curPosition = Position.MoveTowardsDiagonalAware(drone.curPosition, drone.targetPosition, DroneSpeedPerTick);
			if (drone.curPosition == drone.targetPosition) {
				drone.isTravelling = false;
			}
		}

		switch (drone.myState) {
			case DroneState.idle:

				if (drone.myInventory.inventory.Count > 0) {
					drone.myInventory.SetInventory(new List<InventoryItemSlot>());
				}

				drone.idleCounter -= 1f / FactoryDrones.DroneUpdatePerSecond;

				if (drone.idleCounter <= 0f) {
					DroneStartTravelToLocation(drone, drone.curPosition + new Position(Random.Range(-3, 4), Random.Range(-3, 4)));
					drone.idleCounter += Random.Range(5f, 10f);
				}

				break;
			
			// When starting the task first time we need to check if the building inventory already includes some of the building materials
			// This could be leftover from a cancelled destruction order
			case DroneState.BeginBuildingTask:
				
				// Tally both our inventory and the building's construction inventory for the available items
				drone.constructionInv = Grid.s.GetTile(drone.currentTask.location).myConstruction.constructionInventory;
				for (int k = 0; k < drone.currentTask.materials.Count; k++) {
					drone.currentTask.materials[k].count = drone.constructionInv.GetAmountOfItems(drone.currentTask.materials[k].myItem);
				}
				
				for (int k = 0; k < drone.currentTask.materials.Count; k++) {
					drone.currentTask.materials[k].count += drone.myInventory.GetAmountOfItems(drone.currentTask.materials[k].myItem);
				}

				drone.myState = DroneState.SearchingItem;

				break;
			case DroneState.SearchingItem:
				
				bool finishedGathering = true;
				for (int i = 0; i < drone.currentTask.materials.Count; i++) {
					if (drone.currentTask.materials[i].count < drone.currentTask.materials[i].maxCount) {
						finishedGathering = false;
						break;
					}
				}

				if (finishedGathering) {
					drone.myState = DroneState.TravellingToBuild;
					DroneStartTravelToLocation(drone, drone.currentTask.location);
					return;
				}
				
				// Make the drone look for storage buildings
				for (int i = 0; i < storageBuildings.Count; i++) {
					for (int k = 0; k < drone.currentTask.materials.Count; k++) {
						if (drone.currentTask.materials[k].count < drone.currentTask.materials[k].maxCount) {
							if (storageBuildings[i].CheckTakeItem(drone.currentTask.materials[k].myItem, 1, true)) {
								drone.myState = DroneState.TravellingToItem;
								drone.targetStorage = FactoryDrones.storageBuildings[i];
								DroneStartTravelToLocation(drone, drone.targetStorage.myLocation);
								return;
							}
						}
					}
				}
				
				// If we've reached here that means there are no available storage buildings. In this case we can travel to the construction target
				// And stand there to show to the player how sad we are that we cannot build our building 
				DroneStartTravelToLocation(drone, drone.currentTask.location);
				drone.myState = DroneState.UnableToFindConstructionItem;

				break;
			case DroneState.UnableToFindConstructionItem:

				drone.myState = DroneState.idle;
				drone.currentTask.construction.isAssignedToDrone = false;
				drone.isBusy = false;
				
				break;
			case DroneState.TravellingToItem:

				if (!drone.isTravelling) {
					drone.myState = DroneState.TakingItem;
				}
				break;

			case DroneState.TakingItem: 
				{
					var k = 0;
					while (k < drone.currentTask.materials.Count) {
						if (drone.currentTask.materials[k].count < drone.currentTask.materials[k].maxCount) {
							if (drone.targetStorage.TryTakeItem(drone.currentTask.materials[k].myItem, 1, true)) {
								drone.currentTask.materials[k].count += 1;
								drone.myInventory.ForceAddItem(drone.currentTask.materials[k].myItem, 1, true, true);
								return;
							} else {
								k++;
							}
						} else {
							k++;
						}
					}
				}

				drone.myState = DroneState.SearchingItem;

				break;
			case DroneState.TravellingToBuild:

				if (!drone.isTravelling) {
					drone.isLaser = true;
					drone.myState = DroneState.Building;
				}


				break;
			case DroneState.Building:

				for (int k = 0; k < drone.currentTask.materials.Count; k++) {
					if (drone.currentTask.materials[k].count > 0) {
						drone.currentTask.materials[k].count -= 1;
						if (drone.myInventory.TryTakeItem(drone.currentTask.materials[k].myItem, 1, true)) {
							drone.constructionInv.ForceAddItem(drone.currentTask.materials[k].myItem, 1, true, true);
						}
						return;
					}
				}

				FactoryBuilder.CompleteConstruction(drone.currentTask.construction);
				
				drone.isLaser = false;
				drone.isBusy = false;
				drone.currentTask = null;
				drone.myState = DroneState.idle;

				break;


			// ----------------------------------------------- De-construction steps
			
			
			
			case DroneState.BeingDestructionTask:
				
				drone.constructionInv = drone.currentTask.construction.constructionInventory;
				drone.myState = DroneState.SearchingToDestroy;
				
				break;
			
			case DroneState.SearchingToDestroy:
				
				drone.myState = DroneState.TravellingToDestroy;
				DroneStartTravelToLocation(drone, drone.currentTask.location);

				break;
			
			
			case DroneState.TravellingToDestroy:
				if (!drone.isTravelling) {
					drone.isLaser = true;
					drone.myState = DroneState.Destroying;
				}

				break;

			case DroneState.Destroying: 
			{
				if (drone.constructionInv.TryTakeNextItem(out var item, true)) {
					drone.myInventory.ForceAddItem(item, 1, true, true);
					return;
				}
			} 
			{
				if (drone.constructionInv.TryTakeNextItem(out var item, false)) {
					drone.myInventory.ForceAddItem(item, 1, true, true);
					return;
				}
			} {
				var afterConstructInv = drone.currentTask.construction.afterConstructionInventory;
				if (afterConstructInv != null) {
					if (afterConstructInv.Count > 0) {
						for (int i = 0; i < afterConstructInv.Count; i++) {
							var curSlot = afterConstructInv[i];
							if (curSlot.count > 0) {
								drone.myInventory.ForceAddItem(curSlot.myItem, 1, true, true);
								curSlot.count -= 1;
								return;
							}
						}
					}
				}
			}
				FactoryBuilder.CompleteDeconstruction(drone.currentTask.location);
				drone.isLaser = false;
				drone.currentTask = null;
				drone.myState = DroneState.SearchingToEmptyInventory;

				break;

			case DroneState.SearchingToEmptyInventory: 
				
				bool finishedEmptying = drone.myInventory.GetTotalAmountOfItems() == 0;

				if (finishedEmptying) {
					drone.myState = DroneState.idle;
					drone.isBusy = false;
					return;
				}
				
			{
				// Make the drone look for storage buildings
				if (drone.myInventory.CheckTakeNextItem(out var item)) {
					for (int i = 0; i < FactoryDrones.storageBuildings.Count; i++) {
						if (FactoryDrones.storageBuildings[i].CheckAddItem(item, 1, true)) {
							drone.myState = DroneState.TravellingToEmptyInventory;
							drone.targetStorage = FactoryDrones.storageBuildings[i];
							DroneStartTravelToLocation(drone,drone.targetStorage.myLocation);
							return;
						}
					}
				}
			}
				// If we've reached here that means there are no available storage buildings. In this case we can travel to the construction target
				// And stand there to show to the player how sad we are that we cannot build our building 
				DroneStartTravelToLocation(drone, drone.curPosition + new Position(Random.Range(-1, 2), Random.Range(-1, 2)));
				drone.myState = DroneState.UnableToFindEmptyStorage;

				break;
			case DroneState.UnableToFindEmptyStorage:

				drone.myState = DroneState.idle;
				drone.isBusy = false;
				
				break;

			case DroneState.TravellingToEmptyInventory:
				
				if (!drone.isTravelling) {
					drone.myState = DroneState.EmptyingInventory;
				}
				break;

			case DroneState.EmptyingInventory: 
			{
				// Make the drone look for storage buildings
				if (drone.myInventory.TryTakeNextItem(out var item)) {
					if (drone.targetStorage.TryAddItem(item, 1, true)) {
						return;
					}

				}
			}

				drone.myState = DroneState.SearchingToEmptyInventory;
				break;
		}
    }
}


[Serializable]
public class DroneTask {
	public enum DroneTaskType {
		build, destroy
	}
	
	public DroneTaskType myType;
	public Position location = Position.InvalidPosition();
	public List<InventoryItemSlot> materials = new List<InventoryItemSlot>();
	public Construction construction;

	public DroneTask(Construction cons) {
		construction = cons;
		if (construction.isConstruction) {
			myType = DroneTaskType.build;
		} else {
			myType = DroneTaskType.destroy;
		}

		location = construction.center;
		var inv = cons.constructionInventory.inventory;
		materials.Clear();
		for (int i = 0; i < inv.Count; i++) {
			materials.Add(new InventoryItemSlot(inv[i].myItem, 0, inv[i].maxCount, InventoryItemSlot.SlotType.storage));
		}
	}

	public DroneTask(DataSaver.DroneSaveData saveData) {
		construction = Grid.s.GetTile(saveData.currentTaskPosition).myConstruction;
		if (construction != null) {
			if (construction.isConstruction) {
				myType = DroneTaskType.build;
			} else {
				myType = DroneTaskType.destroy;
			}

			location = construction.center;
			materials = DataSaver.InventoryData.ConvertToRegularData(saveData.currentTaskMaterials);
		} else {
			location = Position.InvalidPosition();
		}
	}


	public static int DroneTaskTypeToInt(DroneTaskType type) {
		switch (type) {
			case DroneTaskType.build:
				return 0;
			case DroneTaskType.destroy:
				return 1;
		}

		return -1;
	}
	
	public static DroneTaskType IntToDroneTaskType(int type) {
		switch (type) {
			case 0:
				return DroneTaskType.build;
			case 1:
				return DroneTaskType.destroy;
		}

		return DroneTaskType.build;
	}
}


