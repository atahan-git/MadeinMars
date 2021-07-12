using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_SpaceshipLanding : MonoBehaviour {

    public BuildingData spaceshipData;

    //public GameObject regularGUI;
    public GameObject spaceshipLandingGUI;
    public MiniGUI_BuildingBarSlot spaceshipSlot;

    private Player_BuildingController _controller;
    private void Awake() {
	    spaceshipLandingGUI.SetActive(false);
	    _controller = transform.parent.GetComponentInChildren<Player_BuildingController>();
    }

    private void Start() {
		GameMaster.CallWhenNewPlanet(InitializeSpaceshipLanding);
	}
	
	private void OnDestroy() {
		GameMaster.RemoveFromCall(InitializeSpaceshipLanding);
	}

    public void InitializeSpaceshipLanding() {
	    GUI_SwitchController.s.HideAllMenus();
	    spaceshipLandingGUI.SetActive(true);
	    DataSaver.s.mySave.isSpaceshipLanded = false;
	    spaceshipSlot.ChangeBuilding(spaceshipData, true, new List<InventoryItemSlot>(), callback:FinishBuilding);
    }


    public void FinishBuilding(bool isSuccess) {
	    if (isSuccess) {
		    spaceshipLandingGUI.SetActive(false);
		    NewGameWorldSetup.s.SetUpNewPlanet();
		    DataSaver.s.mySave.isSpaceshipLanded = true;
	    }
    }

    public void LookForNewLandingSite() {
	    GameMaster.s.NewLocationInPlanet();
    }
    
    public void LeavePlanet () {
	    
    }
}
