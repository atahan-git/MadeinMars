using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMaster : MonoBehaviour {
    public static BuildingMaster s;
	public const float buildingUpdatePerSecond = 4;

	public static List<BuildingCraftingController> myBuildings = new List<BuildingCraftingController>();

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

			for (int i = 0; i < myBuildings.Count; i++) {
				if (myBuildings[i] != null) {
					if (myBuildings[i].isActive) {
						myBuildings[i].TakeItemsIn();
						energyUsed += myBuildings[i].UpdateCraftingProcess(efficiency);
						myBuildings[i].PutItemsOut();
					}
				}
			}
			
			yield return new WaitForSeconds(1f / buildingUpdatePerSecond);
		}
	}
}
