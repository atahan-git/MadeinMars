using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DroneController : MonoBehaviour {
	
	/*
	public Position curPosition;

	public bool isBusy = false;

	public DroneSystem.DroneTask currentTask;

	public BuildingInventoryController myInventory = new BuildingInventoryController();

	public DroneAnimator myAnim;

	
	private void Start() {
		myAnim = GetComponent<DroneAnimator>();
	}

	public enum DroneState {
		idle,
		BeginBuildingTask,
		SearchingItem,
		TravellingToItem,
		TakingItem,
		TravellingToBuild,
		Building,
		SearchingToEmptyInventory,
		TravellingToEmptyInventory,
		EmptyingInventory,
		SearchingToDestroy,
		BeingDestructionTask,
		TravellingToDestroy,
		Destroying
	}

	public DroneState myState = DroneState.SearchingItem;

	BuildingInventoryController targetStorage;
	BuildingInventoryController constructionInv;
	public float idleCounter = 0;
	public void DroneWorkUpdate() {
		// If at any point our building disappears (due to player destroying it), then we will try to empty our inventory to get idle
		if (isBusy && myState != DroneState.TravellingToEmptyInventory && myState != DroneState.EmptyingInventory && myState != DroneState.SearchingToEmptyInventory) {
			if (!Grid.s.GetTile(currentTask.center).areThereWorldObject) {
				myState = DroneState.SearchingToEmptyInventory;
				myAnim.SetMiningLaser(false);
			}
		}

		switch (myState) {
			case DroneState.idle:

				if (myInventory.inventory.Count > 0) {
					myInventory.SetInventory(new List<InventoryItemSlot>());
				}

				idleCounter -= 1f / DroneSystem.DroneUpdatePerSecond;

				if (idleCounter <= 0f) {
					myAnim.FlyToLocation(curPosition + new Position(Random.Range(-3, 3), Random.Range(-3, 3)));
					idleCounter += Random.Range(5f, 10f);
				}

				break;
			
			// When starting the task first time we need to check if the building inventory already includes some of the building materials
			// This could be leftover from a cancelled destruction order
			case DroneState.BeginBuildingTask:
				
				// Tally both our inventory and the building's construction inventory for the available items
				constructionInv = Grid.s.GetTile(currentTask.center).worldObject.GetComponent<IBuildable>().GetConstructionInventory();
				for (int k = 0; k < currentTask.materials.Length; k++) {
					currentTask.materials[k].count = constructionInv.GetAmountOfItems(currentTask.materials[k].myItem);
					constructionInv.AddSlot(currentTask.materials[k].myItem, currentTask.materials[k].maxCount, InventoryItemSlot.SlotType.storage);
				}
				
				for (int k = 0; k < currentTask.materials.Length; k++) {
					currentTask.materials[k].count += myInventory.GetAmountOfItems(currentTask.materials[k].myItem);
				}

				myState = DroneState.SearchingItem;

				break;
			case DroneState.SearchingItem:
				
				bool finishedGathering = true;
				for (int i = 0; i < currentTask.materials.Length; i++) {
					if (currentTask.materials[i].count < currentTask.materials[i].maxCount) {
						finishedGathering = false;
						break;
					}
				}

				if (finishedGathering) {
					myState = DroneState.TravellingToBuild;
					myAnim.FlyToLocation(currentTask.center);
				}
				
				// Make the drone look for storage buildings
				for (int i = 0; i < DroneSystem.s.storageBuildings.Count; i++) {
					for (int k = 0; k < currentTask.materials.Length; k++) {
						if (currentTask.materials[k].count < currentTask.materials[k].maxCount) {
							if (DroneSystem.s.storageBuildings[i].CheckTakeItem(currentTask.materials[k].myItem, 1, true)) {
								myState = DroneState.TravellingToItem;
								targetStorage = DroneSystem.s.storageBuildings[i];
								myAnim.FlyToLocation(targetStorage.myLocation);
							}
						}
					}
				}

				break;
			case DroneState.TravellingToItem:

				if (myAnim.isInTargetLocation) {
					myState = DroneState.TakingItem;
				}
				break;

			case DroneState.TakingItem: 
				{
					var k = 0;
					while (k < currentTask.materials.Length) {
						if (currentTask.materials[k].count < currentTask.materials[k].maxCount) {
							if (targetStorage.TryTakeItem(currentTask.materials[k].myItem, 1, true)) {
								currentTask.materials[k].count += 1;
								myInventory.ForceAddItem(currentTask.materials[k].myItem, 1, true, true);
								myAnim.ShowPlusOne();
								return;
							} else {
								k++;
							}
						} else {
							k++;
						}
					}
				}

				myState = DroneState.SearchingItem;

				break;
			case DroneState.TravellingToBuild:

				if (myAnim.isInTargetLocation) {
					myAnim.SetMiningLaser(true);
					myState = DroneState.Building;
				}


				break;
			case DroneState.Building:

				for (int k = 0; k < currentTask.materials.Length; k++) {
					if (currentTask.materials[k].count > 0) {
						currentTask.materials[k].count -= 1;
						if (myInventory.TryTakeItem(currentTask.materials[k].myItem, 1, true)) {
							constructionInv.ForceAddItem(currentTask.materials[k].myItem, 1, true, true);
						}
						return;
					}
				}

				Grid.s.GetTile(currentTask.center).worldObject.GetComponent<IBuildable>().CompleteBuilding();
				myAnim.SetMiningLaser(false);
				isBusy = false;
				myState = DroneState.idle;

				break;


			// ----------------------------------------------- De-construction steps
			
			
			
			case DroneState.BeingDestructionTask:
				
				// Add building materials to the destruction inventory
				constructionInv = Grid.s.GetTile(currentTask.center).worldObject.GetComponent<IBuildable>().GetConstructionInventory();
				for (int k = 0; k < currentTask.materials.Length; k++) {
					constructionInv.ForceAddItem(currentTask.materials[k].myItem, currentTask.materials[k].maxCount, true, true);
				}

				myState = DroneState.SearchingToDestroy;
				
				break;
			
			case DroneState.SearchingToDestroy:
				
				myState = DroneState.TravellingToDestroy;
				myAnim.FlyToLocation(currentTask.center);

				break;
			
			
			case DroneState.TravellingToDestroy:
				if (myAnim.isInTargetLocation){
					myAnim.SetMiningLaser(true);
					myState = DroneState.Destroying;
				}

				break;

			case DroneState.Destroying: 
			{
				if (constructionInv.TryTakeNextItem(out var item, true)) {
					myInventory.ForceAddItem(item, 1, true, true);
					return;
				}
			} 
			{
				if (constructionInv.TryTakeNextItem(out var item, false)) {
					myInventory.ForceAddItem(item, 1, true, true);
					return;
				}
			}
				Grid.s.GetTile(currentTask.center).worldObject.GetComponent<IBuildable>().DestroyYourself();
				myAnim.SetMiningLaser(false);
				myState = DroneState.SearchingToEmptyInventory;

				break;

			case DroneState.SearchingToEmptyInventory: 
				
				bool finishedEmptying = myInventory.GetTotalAmountOfItems() == 0;

				if (finishedEmptying) {
					myState = DroneState.idle;
					isBusy = false;
				}
				
			{
				// Make the drone look for storage buildings
				if (myInventory.CheckTakeNextItem(out var item)) {
					for (int i = 0; i < DroneSystem.s.storageBuildings.Count; i++) {
						if (DroneSystem.s.storageBuildings[i].CheckAddItem(item, 1, true)) {
							myState = DroneState.TravellingToEmptyInventory;
							targetStorage = DroneSystem.s.storageBuildings[i];
							myAnim.FlyToLocation(targetStorage.myLocation);
						}
					}
				}
			}
				break;

			case DroneState.TravellingToEmptyInventory:
				
				if (myAnim.isInTargetLocation) {
					myState = DroneState.EmptyingInventory;
				}
				break;

			case DroneState.EmptyingInventory: 
			{
				// Make the drone look for storage buildings
				if (myInventory.TryTakeNextItem(out var item)) {
					if (targetStorage.TryAddItem(item, 1, true)) {
						return;
					}

				}
			}

				myState = DroneState.SearchingToEmptyInventory;
				break;
		}
	}



	public Position GetSpiral(int count) {
		// (di, dj) is a vector - direction in which we move right now
		int di = 1;
		int dj = 0;
		// length of current segment
		int segmentLength = 1;

		// current center (i, j) and how much of current segment we passed
		int i = 0;
		int j = 0;
		int segmentPassed = 0;
		for (int k = 0; k < count; ++k) {
			// make a step, add 'direction' vector (di, dj) to current center (i, j)
			i += di;
			j += dj;
			++segmentPassed;

			if (segmentPassed == segmentLength) {
				// done with current segment
				segmentPassed = 0;

				// 'rotate' directions
				int buffer = di;
				di = -dj;
				dj = buffer;

				// increase segment length if necessary
				if (dj == 0) {
					++segmentLength;
				}
			}
		}

		return new Position(i, j);
	}*/
}
