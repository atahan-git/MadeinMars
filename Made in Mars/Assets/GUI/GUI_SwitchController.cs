using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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
		bbarcont = GetComponent<GUI_BuildingBarController>();
	}

	private void Start () {
		HideAllMenus();
	}

	public void HideAllMenus() {
		if (GUI_Inventory)
			GUI_Inventory.SetActive(false);
		if (GUI_Settings)
			GUI_Settings.SetActive(false);
		if (GUI_Crafting)
			GUI_Crafting.SetActive(false);
		if (GUI_Comms)
			GUI_Comms.SetActive(false);
		if(bbarcont)
			bbarcont.isOnFocus = true;
		if(Player_MasterControlCheck.s)
			Player_MasterControlCheck.s.ToggleMovement(true);
	}

	void ShowOneMenu(GameObject menu) {
		HideAllMenus();
		if (bbarcont)
			bbarcont.isOnFocus = false;
		if (Player_MasterControlCheck.s)
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
		GameQuitter.QuitGame();
		SceneChangeMaster.s.LoadMenu();
		//SceneManager.LoadScene(0);
	}

	public void BringBuildingBarToFocus () {
		HideAllMenus();
	}
}
