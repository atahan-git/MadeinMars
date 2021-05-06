using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores important variables for cross player_ script access
/// </summary>
public class Player_MasterControlCheck : MonoBehaviour
{
    public bool isMovementEnabled = true;
	public bool isPlacingItem = false;
	public static Player_MasterControlCheck s;
	public Player_BuildingController buildingController;

	private void Awake () {
		if (s != null) {
			Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
		}
		s = this;
		buildingController = GetComponent<Player_BuildingController>();
	}

	public void ToggleMovement (bool state) {
		isMovementEnabled = state;
	}

	public void TogglePlacingItem (bool state) {
		isPlacingItem = state;
	}
}
