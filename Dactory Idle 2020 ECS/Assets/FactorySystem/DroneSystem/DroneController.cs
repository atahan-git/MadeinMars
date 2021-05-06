using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour, IInventoryController {


	public Position curPosition;

	public bool isBusy = false;

	public DroneSystem.DroneTask currentTask;

	public DroneAnimator myAnim;

	
	private void Start() {
		myAnim = GetComponent<DroneAnimator>();
	}

	public enum DroneState {
		SearchingItem,
		TravellingToItem,
		TravellingToBuild,
		Building,
		TravellingToEmptyInventory,
		EmptyingInventory,
		TravellingToDestroy,
		Destroying
	}

	public DroneState myState = DroneState.SearchingItem;

	public BuildingInventoryController targetStorage;
	public void DroneWorkUpdate() {
		// If at any point our building disappears (due to player destroying it), then we stop our current tasks
		if (Grid.s.GetTile(currentTask.location).areThereWorldObject) {
			switch (myState) {
				case DroneState.SearchingItem:
					// Make the drone look for storage buildings
					for (int i = 0; i < DroneSystem.s.storageBuildings.Count; i++) {
						for (int k = 0; k < currentTask.requirements.Length; k++) {
							if (currentTask.requirements[k].count < currentTask.requirements[k].maxCount) {
								if (DroneSystem.s.storageBuildings[i].CheckTakeItem(currentTask.requirements[k].myItem, 1, true)) {
									myState = DroneState.TravellingToItem;
									targetStorage = DroneSystem.s.storageBuildings[i];
									myAnim.FlyToLocation(targetStorage.myLocation);
									myAnim.isInTargetLocation = false;
								}	
							}
						}
					}
					
					bool finishedGathering = true;
					for (int i = 0; i < currentTask.requirements.Length;i++) {
						if (currentTask.requirements[i].count < currentTask.requirements[i].maxCount) {
							finishedGathering = false;
							break;
						}
					}

					if (finishedGathering) {
						myState = DroneState.TravellingToBuild;
						myAnim.FlyToLocation(currentTask.location);
						myAnim.isInTargetLocation = false;
					}
					
					break;
				case DroneState.TravellingToItem:

					if (myAnim.isInTargetLocation) {
						var k = 0;
						while(k < currentTask.requirements.Length){
							if (currentTask.requirements[k].count < currentTask.requirements[k].maxCount) {
								if (targetStorage.TryTakeItem(currentTask.requirements[k].myItem, 1, true)) {
									currentTask.requirements[k].count += 1;
									myAnim.ShowPlusOne();
									InventoryContentsChanged();
									return;
								} else {
									k++;
								}
							} else {
								k++;
							}
						}

						myState = DroneState.SearchingItem;
					}

					break;
				case DroneState.TravellingToBuild:

					if (myAnim.isInTargetLocation) {
						var reqCount = 0;
						for (int i = 0; i < currentTask.requirements.Length; i++) {
							reqCount += currentTask.requirements[i].maxCount;
						}
						
						myAnim.SetMiningLaser(true);
						myState = DroneState.Building;
					}


					break;
				case DroneState.Building:

					for (int k = 0; k < currentTask.requirements.Length; k++) {
						if (currentTask.requirements[k].count > 0) {
							currentTask.requirements[k].count -= 1;
							InventoryContentsChanged();
							return;
						}
					}

					Grid.s.GetTile(currentTask.location).worldObject.GetComponent<IBuildable>().CompleteBuilding();
					myAnim.SetMiningLaser(false);
					isBusy = false;
					
					break;
			}
		} else {
			myAnim.SetMiningLaser(false);
			isBusy = false;
		}
	}



	public Position GetSpiral(int count) {
		// (di, dj) is a vector - direction in which we move right now
		int di = 1;
		int dj = 0;
		// length of current segment
		int segmentLength = 1;

		// current position (i, j) and how much of current segment we passed
		int i = 0;
		int j = 0;
		int segmentPassed = 0;
		for (int k = 0; k < count; ++k) {
			// make a step, add 'direction' vector (di, dj) to current position (i, j)
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
	}

	public void DrawInventory() {
		drawInventoryEvent?.Invoke();
	}
	public void InventoryContentsChanged() {
		inventoryContentsChangedEvent?.Invoke();
	}
	
	public event GenericCallback drawInventoryEvent;
	public event GenericCallback inventoryContentsChangedEvent;
}
