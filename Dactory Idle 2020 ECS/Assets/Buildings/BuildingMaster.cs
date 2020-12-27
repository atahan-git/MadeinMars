using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Keeps track of all the buildings and runs their update loops.
/// </summary>
public class BuildingMaster : MonoBehaviour {
    public static BuildingMaster s;
	public const float buildingUpdatePerSecond = 4;

	public static List<BuildingCraftingController> myBuildingCrafters = new List<BuildingCraftingController>();
	public static List<BuildingInOutController> myBuildingInOuters = new List<BuildingInOutController>();

	private void Awake () {
		if (s != null) {
			Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
		}
		s = this;
	}

	public void StartBuildingSystem () {
		print("Starting Building System");
		StartBuildingLoops();
	}


	public void StartBuildingLoops () {
		StartCoroutine(UpdateLoop());
	}


	float energyProduced = 100000;
	float energyUsed = 1;
	float efficiency;

	IEnumerator UpdateLoop () {
		while (true) {
			efficiency = energyProduced / energyUsed;
			if (efficiency > 1)
				efficiency = 1;
			energyUsed = 1;
			energyProduced = 100000;

			for (int i = 0; i < myBuildingCrafters.Count; i++) {
				if (myBuildingCrafters[i] != null) {
					if (myBuildingCrafters[i].isActive) {
						energyUsed += myBuildingCrafters[i].UpdateCraftingProcess(efficiency);
					}
				}
			}
			
			for (int i = 0; i < myBuildingInOuters.Count; i++) {
				if (myBuildingInOuters[i] != null) {
					if (myBuildingInOuters[i].isActive) {
						myBuildingInOuters[i].TakeItemsIn();
						myBuildingInOuters[i].PutItemsOut();
					}
				}
			}
			
			yield return new WaitForSeconds(1f / buildingUpdatePerSecond);
		}
	}
}
