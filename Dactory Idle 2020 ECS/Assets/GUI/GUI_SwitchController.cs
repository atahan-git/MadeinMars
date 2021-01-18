using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;



/// <summary>
/// Helps with the UI menu switching
/// </summary>
public class GUI_SwitchController : MonoBehaviour {
	public static GUI_SwitchController s;

	public GameObject GUI_Inventory;
	public GameObject GUI_Settings;
	public GameObject GUI_Crafting;
	public GameObject GUI_Comms;
	GUI_BuildingBarController bbarcont;

	private void Awake() {
		if (s != null) {
			Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
		}

		s = this;
	}

	private void Start () {
		bbarcont = GetComponent<GUI_BuildingBarController>();
		HideAllMenus();
	}

	public void HideAllMenus() {
		GUI_Inventory.SetActive(false);
		GUI_Settings.SetActive(false);
		GUI_Crafting.SetActive(false);
		GUI_Comms.SetActive(false);
		bbarcont.isOnFocus = true;
		Player_MasterControlCheck.s.ToggleMovement(true);
	}

	void ShowOneMenu(GameObject menu) {
		GUI_Inventory.SetActive(false);
		GUI_Settings.SetActive(false);
		GUI_Crafting.SetActive(false);
		GUI_Comms.SetActive(false);
		bbarcont.isOnFocus = false;
		Player_MasterControlCheck.s.ToggleMovement(false);
		
		menu.SetActive(true);
	}

	void ToggleMenu(GameObject menu) {
		if (menu.activeSelf) {
			HideAllMenus();
		} else {
			ShowOneMenu(menu);
		}
	}

	
	public void ToggleInventory () {ToggleMenu(GUI_Inventory); }
	public void ToggleSettings () { ToggleMenu(GUI_Settings); }
	public void ToggleRecipes () { ToggleMenu(GUI_Crafting); }
	public void ToggleComms () { ToggleMenu(GUI_Comms); }

	
	public void SaveandQuit() {
		GameMaster.StartSavingGameProcess();
		SceneManager.LoadScene(0);
	}

	public void BringBuildingBarToFocus () {
		HideAllMenus();
	}
}
