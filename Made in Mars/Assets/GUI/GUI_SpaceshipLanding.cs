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
	    GameMaster.CallWhenLoaded(InitializeSpaceshipLanding);
    }

	
	private void OnDestroy() {
		GameMaster.CallWhenLoaded(InitializeSpaceshipLanding);
	}

	public void InitializeSpaceshipLanding(bool isLoadSuccess) {
		var currentPlanetSave = DataSaver.s.GetSave().currentPlanet;
		if (currentPlanetSave.isSpaceshipLanded == false) {
			GUI_SwitchController.s.HideAllMenus();
			spaceshipLandingGUI.SetActive(true);
			currentPlanetSave.isSpaceshipLanded = false;
			spaceshipSlot.ChangeBuilding(spaceshipData, true, new List<InventoryItemSlot>(), callback: FinishBuilding);
		}
	}

	public void FinishBuilding(bool isSuccess) {
	    if (isSuccess) {
		    if (FactoryMaster.s.GetShip() != null) {
			    spaceshipLandingGUI.SetActive(false);
			    DataSaver.s.GetSave().currentPlanet.isSpaceshipLanded = true;
			    print(SpriteGraphicsController.SpaceLandingTime);
			    Invoke("DelayedStart", SpriteGraphicsController.SpaceLandingTime);
		    }
	    }
    }

    void DelayedStart() {
	    NewGameWorldSetup.s.SetUpAfterShipLanding();
    }

    public void LookForNewLandingSite() {
	    GameMaster.s.NewLocationInPlanet();
    }
    
    public void LeavePlanet () {
	    GameMaster.s.LeavePlanet();
    }
}
