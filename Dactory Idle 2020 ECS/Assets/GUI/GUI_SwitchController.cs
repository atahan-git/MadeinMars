using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_SwitchController : MonoBehaviour
{

	public GameObject GUI_Inventory;
	GUI_BuildingBarController bbarcont;

	private void Start () {
		bbarcont = GetComponent<GUI_BuildingBarController>();
	}


	public void ToggleInventory () {
		if (GUI_Inventory.activeSelf) {
			GUI_Inventory.SetActive(false);
			bbarcont.isOnFocus = true;
			Player_MasterControlCheck.s.ToggleMovement(true);
		} else {
			GUI_Inventory.SetActive(true);
			bbarcont.isOnFocus = false;
			Player_MasterControlCheck.s.ToggleMovement(false);
		}
	}

	public void BringBuildingBarToFocus () {
		GUI_Inventory.SetActive(false);
		bbarcont.isOnFocus = true;
	}
}
