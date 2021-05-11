using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneSystem : MonoBehaviour {
	/*public static DroneSystem s;
	public const float DroneUpdatePerSecond = 1;
	
	public enum DroneTaskType {
		build, destroy
	}

	[Serializable]
	public class DroneTask {
		public DroneTaskType myType;
		public Position center;
		public InventoryItemSlot[] materials;
	}


	[SerializeField]
	List<DroneTask> droneTasks = new List<DroneTask>();

	public List<BuildingInventoryController> storageBuildings = new List<BuildingInventoryController>();

	public List<DroneController> drones;

	private void Awake() {
		s = this;
	}


	public void StartDroneSystem() {
		StartCoroutine(DroneSystemUpdateLoop());
	}
	
	IEnumerator DroneSystemUpdateLoop() {
		yield return new WaitForSeconds(0.5f);
		while (true) {
			for (int i = 0; i < drones.Count; i++) {
				if (!drones[i].isBusy) {
					if (droneTasks.Count > 0) {
						var curTask = droneTasks[0];
						droneTasks.RemoveAt(0);
						drones[i].currentTask = curTask;
						drones[i].isBusy = true;
						if (curTask.myType == DroneTaskType.build) {
							drones[i].myState = DroneController.DroneState.BeginBuildingTask;
						}else if (curTask.myType == DroneTaskType.destroy) {
							drones[i].myState = DroneController.DroneState.BeingDestructionTask;
						}
					}
				
				} 
				
				drones[i].DroneWorkUpdate();
			}
			
			
			yield return new WaitForSeconds(1f / DroneUpdatePerSecond);
		}
	}

	public void AddDroneDestroyTask(Position center, BuildingData myData) {
		droneTasks.Add(new DroneTask() {myType = DroneTaskType.destroy, center = center, materials = GetRequirements(myData, false)});
	}

	public void AddDroneBuildTask(Position center, BuildingData myData) {
		droneTasks.Add(new DroneTask(){myType = DroneTaskType.build, center = center, materials = GetRequirements(myData, true)});
	}

	public void RemoveDroneTask(Position center) {
		for (int i = 0; i < droneTasks.Count; i++) {
			if (droneTasks[i].center == center) {
				droneTasks.RemoveAt(i);
				break;
			}
		}
	}
	
	InventoryItemSlot[] GetRequirements(BuildingData myDat, bool isInput) {
		CraftingNode[] ps = DataHolder.s.GetCraftingProcessesOfType(BuildingData.ItemType.Building);
		if (ps != null) {
			for (int i = 0; i < ps.Length; i++) {
				if (ps[i].outputs[0].itemUniqueName == myDat.uniqueName) {
					var reqs = new InventoryItemSlot[ps[i].inputs.Count];
					for (int m = 0; m < ps[i].inputs.Count; m++) {
						reqs[m] = new InventoryItemSlot(DataHolder.s.GetItem(ps[i].inputs[m].itemUniqueName),
							0, ps[i].inputs[m].count, 
							isInput? InventoryItemSlot.SlotType.input : InventoryItemSlot.SlotType.output);
					}

					return reqs;
				}
			}
		}

		return new InventoryItemSlot[0];
	}


	public void RegisterStorageBuilding(BuildingInventoryController controller) {
		storageBuildings.Add(controller);
	}
	
	
	public void RemoveStorageBuilding(BuildingInventoryController controller) {
		storageBuildings.Remove(controller);
	}
	*/
	
}


/*
public interface IBuildable {

	BuildingInventoryController GetConstructionInventory();

	void MarkForDeconstruction();
	void UnmarkDestruction();
	void CompleteBuilding();
	void DestroyYourself();
}*/