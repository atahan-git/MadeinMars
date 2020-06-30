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
		StartCoroutine(BeltItemSlotUpdateLoop());
	}

	IEnumerator BeltItemSlotUpdateLoop () {
		while (true) {

			for (int i = 0; i < myBuildings.Count; i++) {
				if (myBuildings[i] != null) {
					if (myBuildings[i].isActive) {
						myBuildings[i].TakeItemsIn();
						myBuildings[i].UpdateCraftingProcess();
						myBuildings[i].PutItemsOut();
					}
				}
			}

			yield return new WaitForSeconds(1f / buildingUpdatePerSecond);
		}
	}
}
