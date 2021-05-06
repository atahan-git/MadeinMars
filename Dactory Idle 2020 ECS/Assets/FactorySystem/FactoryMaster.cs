using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryMaster : MonoBehaviour {
	public static FactoryMaster s;
	public const float SimUpdatePerSecond = 4;
	public const float BeltLength = 1f;
	public const float BeltLengthToMovePerSecond = BeltLength * SimUpdatePerSecond;

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


	void StartBuildingLoops () {
		StartCoroutine(UpdateLoop());
	}


	float energyProduced = 100000;
	float energyUsed = 1;
	float efficiency;

	IEnumerator UpdateLoop () {
		yield return new WaitForSeconds(0.5f);
		while (true) {
			efficiency = energyProduced / energyUsed;
			if (efficiency > 1)
				efficiency = 1;
			energyUsed = 1;
			energyProduced = 100000;

			for (int i = 0; i < FactorySystem.s.buildings.Count; i++) {
				if (FactorySystem.s.buildings[i] != null) {
					if (FactorySystem.s.buildings[i].craftController.isActive) {
						energyUsed += FactorySystem.UpdateBuilding(FactorySystem.s.buildings[i], efficiency);
					}
				}
			}

			for (int i = 0; i < FactorySystem.s.connectors.Count; i++) {
				if (FactorySystem.s.connectors[i] != null) {
					FactorySystem.UpdateConnector(FactorySystem.s.connectors[i]);
				}
			}
			
			FactoryVisuals.s.BeltVisualsUpdate();
			FactoryVisuals.s.ConnectorVisualUpdate();
			
			for (int i = 0; i < FactorySystem.s.belts.Count; i++) {
				if (FactorySystem.s.belts[i] != null) {
					FactorySystem.UpdateBelt(FactorySystem.s.belts[i]);
				}
			}
			

			yield return new WaitForSeconds(1f / SimUpdatePerSecond);
		}
	}
}
