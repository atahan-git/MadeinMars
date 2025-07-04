﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using IDroneState = Drone.IDroneState;

public class FactoryDrones {
	public const int DronesSlowFactor = 2;
	public static float DroneUpdatePerSecond = FactoryMaster.SimUpdatePerSecond / DronesSlowFactor;
	public const int DroneSpeedPerTick = 3;
	public static float DroneWorldSpeed = DroneSpeedPerTick * DroneUpdatePerSecond;

	public static List<Building> storageBuildings = new List<Building>();

	public static void RegisterStorageBuilding(Building building) {
		storageBuildings.Add(building);
	}


	public static void RemoveStorageBuilding(Building building) {
		storageBuildings.Remove(building);
	}

	public static void CancelConstructionMidTask(Construction construction) {
		if (construction.IsAssignedToDrone()) {
			for (int i = 0; i < FactoryMaster.s.GetDrones().Count; i++) {
				var myDrone = FactoryMaster.s.GetDrones()[i];
				var thisDroneAssignedToThisConstruction = !myDrone.isBusy && myDrone.currentTask != null && myDrone.currentTask.construction.center == construction.center;
				if (thisDroneAssignedToThisConstruction) {
					myDrone.isLaser = false;
					myDrone.myState = new DroneSearchStorageToEmptyInventory();
					break;
				}
			}
		}

		construction.UnAssignFromDrone();
	}


	public static void UpdateTasks() {

		var assignableToDrones = new List<IAssignableToDrone>();
		assignableToDrones.AddRange(FactoryMaster.s.GetConstructions());
		
		for (int m = 0; m < assignableToDrones.Count; m++) {
			var assignable = assignableToDrones[m];

			if (!assignable.IsAssignedToDrone()) {
				if (GetFreeDrone(out Drone drone)) {
					AssignToDrone(assignable, drone);
				} else {
					// If there are no free drones left, we stop trying to assign tasks
					return;
				}
			}
		}
		
		
		void AssignToDrone(IAssignableToDrone assignable, Drone drone) {
			assignable.AssignToDrone();
			drone.currentTask = new DroneTask(assignable);
			drone.isBusy = true;
			switch (drone.currentTask.myType) {
				case DroneTask.DroneTaskType.build:
					drone.myState = new DroneBeginConstruction();
					break;
				case DroneTask.DroneTaskType.destroy:
					drone.myState = new DroneBeginDeconstruction();
					break;
				case DroneTask.DroneTaskType.transportItem:
					throw new NotImplementedException();
			}
			
		}
	}

	public static bool GetFreeDrone(out Drone drone) {
		for (int i = 0; i < FactoryMaster.s.GetDrones().Count; i++) {
			drone = FactoryMaster.s.GetDrones()[i];
			if (!drone.isBusy) {
				return true;
			}
		}
		drone = null;
		return false;
	}

	public static void DroneStartTravelToLocation(Drone drone, Position target) {
		drone.isTravelling = true;
		drone.targetPosition = target;
	}

	public static void UpdateDrone(Drone drone) {
		drone.myState = drone.myState.ExecuteAndReturnNextState(drone);
		MoveDroneTowardsTarget(drone);
		drone.debugDroneState.Add(drone.myState.GetInfoDisplayText(drone));
	}

	static void MoveDroneTowardsTarget(Drone drone) {
		if (drone.isTravelling) {
			drone.curPosition = Position.MoveTowardsDiagonalAware(drone.curPosition, drone.targetPosition, DroneSpeedPerTick);
			if (drone.curPosition == drone.targetPosition) {
				drone.isTravelling = false;
			}
		}
	}
}


[Serializable]
public class DroneTask {
	public enum DroneTaskType {
		nullTask, build, destroy, transportItem
	}
	
	public DroneTaskType myType = DroneTaskType.nullTask;
	public Construction construction;

	public DroneTask(IAssignableToDrone assignable) {
		if (assignable is Construction cons) {
			construction = cons;
			if (construction.isConstruction) {
				myType = DroneTaskType.build;
			} else {
				myType = DroneTaskType.destroy;
			}
		} else {
			throw new NotImplementedException();
		}
	}

	public DroneTask(DataSaver.DroneSaveData saveData) {
		if (Grid.s.GetTile(saveData.currentTaskPosition).simObject is Construction _construction) {
			construction = _construction;
			if (construction.isConstruction) {
				myType = DroneTaskType.build;
			} else {
				myType = DroneTaskType.destroy;
			}
		} else {
			myType = DroneTaskType.nullTask;
		}
	}


	private static readonly Map< int,DroneTaskType> droneTaskTypeSavingMap = new Map<int ,DroneTaskType >() {
		{0, DroneTaskType.build},
		{1, DroneTaskType.destroy},
		{3, DroneTaskType.transportItem}
	};
	public static int ConvertDroneTaskTypeAndInt(DroneTaskType type) {
		return droneTaskTypeSavingMap.Get(type);
	}
	
	public static DroneTaskType ConvertDroneTaskTypeAndInt(int type) {
		return droneTaskTypeSavingMap.Get(type);
	}
}


class DroneIdleState : IDroneState {
	const int idleDeadZone = 10;
	public IDroneState ExecuteAndReturnNextState(Drone drone) {
		if (drone.myInventory.GetTotalAmountOfItems() > 0){
			return new DroneSearchStorageToEmptyInventory();
		}
		
		if (drone.myInventory.inventoryItemSlots.Count > 0 && drone.myInventory.GetTotalAmountOfItems() == 0) {
			drone.myInventory.SetInventory(new List<InventoryItemSlot>());
		} 

		drone.idleCounter -= 1f / FactoryDrones.DroneUpdatePerSecond;

		if (drone.idleCounter <= 0f) {
			var randomPos = drone.curPosition + new Position(Random.Range(-3, 4), Random.Range(-3, 4));
			randomPos.x = Mathf.Clamp(randomPos.x, idleDeadZone, Grid.s.mapSize-idleDeadZone);
			randomPos.y = Mathf.Clamp(randomPos.y, idleDeadZone, Grid.s.mapSize-idleDeadZone);
			FactoryDrones.DroneStartTravelToLocation(drone, randomPos);
			drone.idleCounter += Random.Range(5f, 10f);
		}
		
		return this;
	}

	public string GetInfoDisplayText(Drone drone) {

		return this.ToString();
	}
}

class DroneBeginConstruction : IDroneState {
	public IDroneState ExecuteAndReturnNextState(Drone drone) {
		return new DroneSearchItemForConstruction();
	}
	
	public string GetInfoDisplayText(Drone drone) {
		return this.ToString();
	}
}

class DroneSearchItemForConstruction : IDroneState {
	public IDroneState ExecuteAndReturnNextState(Drone drone) {
		bool finishedGathering = true;
		var consInvSlots = drone.currentTask.construction.constructionInventory.inventoryItemSlots;
		for (int i = 0; i < consInvSlots.Count; i++) {
			var droneItemCount = drone.myInventory.GetAmountOfItems(consInvSlots[i].myItem);
			if (consInvSlots[i].count + droneItemCount < consInvSlots[i].maxCount) {
				finishedGathering = false;
				break;
			}
		}

		if (finishedGathering) {
			FactoryDrones.DroneStartTravelToLocation(drone, drone.currentTask.construction.center);
			return new DroneTravelToConstruction();
		}

		// Make the drone look for storage buildings
		var buildings = FactoryMaster.s.GetBuildings();
		for (int i = 0; i < buildings.Count; i++) {
			for (int k = 0; k < consInvSlots.Count; k++) {
				var droneInvCount = drone.myInventory.GetAmountOfItems(consInvSlots[k].myItem);
				if (consInvSlots[k].count + droneInvCount < consInvSlots[k].maxCount) {
					if (buildings[i].inv.CheckIfCanTakeItem(consInvSlots[k].myItem, 1, true)) {
						drone.targetStorage = buildings[i];
						FactoryDrones.DroneStartTravelToLocation(drone, drone.targetStorage.myPositions[0]);
						return new DroneTravelToItemStorage();
					}
				}
			}
		}

		// If there are no available items and we don't have anything to put, travel to the construction and wait
		if (drone.myInventory.GetTotalAmountOfItems() == 0) {
			FactoryDrones.DroneStartTravelToLocation(drone, drone.currentTask.construction.center);
			drone.isBusy = false;
			drone.currentTask.construction.UnAssignFromDrone();
			return new DroneIdleState();
		} else {
			FactoryDrones.DroneStartTravelToLocation(drone, drone.currentTask.construction.center);
			return new DroneTravelToConstruction();
		}
	}

	public string GetInfoDisplayText(Drone drone) {

		return this.ToString();
	}
}

class DroneTravelToItemStorage : IDroneState {
	public IDroneState ExecuteAndReturnNextState(Drone drone) {
		if (drone.isTravelling) {
			return this;
		}else{
			return new DroneTakeItemFromStorage();
		}
	}
	
	public string GetInfoDisplayText(Drone drone) {

		return this.ToString();
	}
}

class DroneTakeItemFromStorage : IDroneState {
	public IDroneState ExecuteAndReturnNextState(Drone drone) {
		{
			if (drone.targetStorage == null) {
				return new DroneSearchItemForConstruction();
			}
			
			var k = 0;
			var consInvSlots = drone.currentTask.construction.constructionInventory.inventoryItemSlots;
			while (k < consInvSlots.Count) {
				var droneInvCount = drone.myInventory.GetAmountOfItems(consInvSlots[k].myItem);
				if (consInvSlots[k].count + droneInvCount < consInvSlots[k].maxCount) {
					if (drone.targetStorage.inv.TryAndTakeItem(consInvSlots[k].myItem, 1, true)) {
						drone.myInventory.TryAndAddItem(consInvSlots[k].myItem, 1, false, true);
						return this;
					} else {
						k++;
					}
				} else {
					k++;
				}
			}
		}
		// go back to searching items to check if we have enough items
		return new DroneSearchItemForConstruction();
	}
	
	public string GetInfoDisplayText(Drone drone) {

		return this.ToString();
	}
}


class DroneTravelToConstruction : IDroneState {
	public IDroneState ExecuteAndReturnNextState(Drone drone) {
		if (drone.isTravelling) {
			return this;
		}else{
			drone.isLaser = true;
			return new DroneConstruct();
		}
		return this;
	}
	
	public string GetInfoDisplayText(Drone drone) {

		return this.ToString();
	}
}


class DroneConstruct : IDroneState {
	public IDroneState ExecuteAndReturnNextState(Drone drone) {
		var consInvSlots = drone.currentTask.construction.constructionInventory.inventoryItemSlots;
		for (int k = 0; k < consInvSlots.Count; k++) {
			if (consInvSlots[k].count < consInvSlots[k].maxCount && drone.myInventory.GetAmountOfItems(consInvSlots[k].myItem) > 0) {
				if (drone.myInventory.TryAndTakeItem(consInvSlots[k].myItem, 1, false)) {
					drone.constructionInv.TryAndAddItem(consInvSlots[k].myItem, 1, false, true);
					return this;
				}
			}
		}

		bool finishedGathering = true;
		for (int i = 0; i < consInvSlots.Count; i++) {
			if (consInvSlots[i].count < consInvSlots[i].maxCount) {
				finishedGathering = false;
				break;
			}
		}

		drone.isLaser = false;

		if (finishedGathering) {
			FactoryBuilder.CompleteConstruction(drone.currentTask.construction);
			
			drone.isBusy = false;
			drone.currentTask = null;
			return new DroneIdleState();
		} else {
			return new DroneSearchItemForConstruction();
		}
	}

	public string GetInfoDisplayText(Drone drone) {

		return this.ToString();
	}
}



class DroneBeginDeconstruction : IDroneState {
	public IDroneState ExecuteAndReturnNextState(Drone drone) {
		FactoryDrones.DroneStartTravelToLocation(drone, drone.currentTask.construction.center);
		return new DroneTravelToDeconstruction();
	}
	
	public string GetInfoDisplayText(Drone drone) {

		return this.ToString();
	}
}



class DroneTravelToDeconstruction : IDroneState {
	public IDroneState ExecuteAndReturnNextState(Drone drone) {
		if (drone.isTravelling) {
			return this;
		}else{
			drone.isLaser = true;
			return new DroneDeconstruct();
		}
		return this;
	}
	
	public string GetInfoDisplayText(Drone drone) {

		return this.ToString();
	}
}


class DroneDeconstruct : IDroneState {
	public IDroneState ExecuteAndReturnNextState(Drone drone) {
		{
			if (drone.constructionInv.TryAndTakeNextItem(out var item, true)) {
				drone.myInventory.TryAndAddItem(item, 1, true, true);
				return this;
			}
		} 
		{
			if (drone.constructionInv.TryAndTakeNextItem(out var item, false)) {
				drone.myInventory.TryAndAddItem(item, 1, true, true);
				return this;
			}
		} {
			var afterConstructInv = drone.currentTask.construction.afterConstructionInventory;
			if (afterConstructInv != null) {
				if (afterConstructInv.Count > 0) {
					for (int i = 0; i < afterConstructInv.Count; i++) {
						var curSlot = afterConstructInv[i];
						if (curSlot.count > 0) {
							drone.myInventory.TryAndAddItem(curSlot.myItem, 1, true, true);
							curSlot.count -= 1;
							return this;
						}
					}
				}
			}
		}
		FactoryBuilder.CompleteDeconstruction(drone.currentTask.construction.center);
		drone.isLaser = false;
		drone.currentTask = null;
		return new DroneSearchStorageToEmptyInventory();
	}
	
	public string GetInfoDisplayText(Drone drone) {

		return this.ToString();
	}
}


class DroneSearchStorageToEmptyInventory : IDroneState {
	public IDroneState ExecuteAndReturnNextState(Drone drone) {
		bool finishedEmptying = drone.myInventory.GetTotalAmountOfItems() == 0;

		if (finishedEmptying) {
			drone.isBusy = false;
			return new DroneIdleState();
		}
				
		{
			// Make the drone look for storage buildings
			if (drone.myInventory.CheckIfCanTakeNextItem(out var item)) {
				for (int i = 0; i < FactoryDrones.storageBuildings.Count; i++) {
					if (FactoryDrones.storageBuildings[i].inv.CheckIfCanAddItem(item, 1, true)) {
						drone.targetStorage = FactoryDrones.storageBuildings[i];
						FactoryDrones.DroneStartTravelToLocation(drone,drone.targetStorage.myPositions[0]);
						return new DroneTravelToEmptyDroneInventoryToStorage();
					}
				}
			}
		}
		// If we've reached here that means there are no available storage buildings. In this case we can travel to the construction target
		// And stand there to show to the player how sad we are that we cannot build our building 
		FactoryDrones.DroneStartTravelToLocation(drone, drone.curPosition + new Position(Random.Range(-1, 2), Random.Range(-1, 2)));
		return new DroneUnableToFindEmptyStorage();
	}
	
	public string GetInfoDisplayText(Drone drone) {

		return this.ToString();
	}
}


class DroneTravelToEmptyDroneInventoryToStorage : IDroneState {
	public IDroneState ExecuteAndReturnNextState(Drone drone) {
		if (drone.isTravelling) {
			return this;
		}else{
			return new DroneEmptyInventoryToStorage();
		}
		return this;
	}
	
	public string GetInfoDisplayText(Drone drone) {

		return this.ToString();
	}
}


class DroneEmptyInventoryToStorage : IDroneState {
	public IDroneState ExecuteAndReturnNextState(Drone drone) {
		if (drone.myInventory.TryAndTakeNextItem(out var item)) {
			if (drone.targetStorage == null) {
				return new DroneSearchStorageToEmptyInventory();
			}
			
			if (drone.targetStorage.inv.TryAndAddItem(item, 1, true)) {
				return this;
			}
		}

		if (drone.myInventory.inventoryItemSlots.Count > 0 && drone.myInventory.GetTotalAmountOfItems() == 0) {
			drone.myInventory.SetInventory(new List<InventoryItemSlot>());
		} 

		return new DroneSearchStorageToEmptyInventory();
	}
	
	public string GetInfoDisplayText(Drone drone) {

		return this.ToString();
	}
}




class DroneUnableToFindConstructionMaterial : IDroneState {
	public IDroneState ExecuteAndReturnNextState(Drone drone) {
		drone.currentTask.construction.UnAssignFromDrone();
		drone.isBusy = false;
		return new DroneIdleState();
	}
	
	public string GetInfoDisplayText(Drone drone) {

		return this.ToString();
	}
}


class DroneUnableToFindEmptyStorage : IDroneState {
	public IDroneState ExecuteAndReturnNextState(Drone drone) {
		drone.isBusy = false;
		return new DroneIdleState();
	}
	
	public string GetInfoDisplayText(Drone drone) {

		return this.ToString();
	}
}


class DroneBeginCarryingShipPart : IDroneState {
	public IDroneState ExecuteAndReturnNextState(Drone drone) {
		
		throw new NotImplementedException();
	}
	
	public string GetInfoDisplayText(Drone drone) {

		return this.ToString();
	}
}
