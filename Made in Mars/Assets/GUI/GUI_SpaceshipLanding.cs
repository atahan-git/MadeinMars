using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	    GameMaster.CallWhenNewPlanet(UpdatePlanetResourcesUI);
    }

	
	private void OnDestroy() {
		GameMaster.RemoveFromCall(InitializeSpaceshipLanding);
		GameMaster.RemoveFromCall(UpdatePlanetResourcesUI);
	}

	private bool infoDisplayPrevValue = true;
	public void InitializeSpaceshipLanding(bool isLoadSuccess) {
		CancelInvoke("DelayedStart");
		var currentPlanetSave = DataSaver.s.GetSave().currentPlanet;
		if (currentPlanetSave.isSpaceshipLanded == false) {
			GUI_SwitchController.s.HideAllMenus();
			spaceshipLandingGUI.SetActive(true);
			currentPlanetSave.isSpaceshipLanded = false;
			spaceshipSlot.ChangeBuilding(spaceshipData, true, new List<InventoryItemSlot>()/*ShipDataMaster.s.shipStarterInventory*/, callback: FinishBuilding);
			UpdatePlanetResourcesUI();

			infoDisplayPrevValue = BuildingInfoDisplay.isExtraInfoVisible;
			BuildingInfoDisplay.isExtraInfoVisible = true;
		}
	}

	public void FinishBuilding(bool isSuccess) {
	    if (isSuccess) {
		    if (FactoryMaster.s.GetShip() != null) {
			    spaceshipLandingGUI.SetActive(false);
			    DataSaver.s.GetSave().currentPlanet.isSpaceshipLanded = true;
			    //print(SpriteGraphicsController.SpaceLandingTime);
			    var shipGfx = Grid.s.GetTile(FactoryMaster.s.GetShip().center).visualObject.GetComponentInChildren<SpriteGraphicsController>();
			    shipGfx.DoSpaceLanding(DelayedStart, spaceshipData.spaceLandingXDisp, spaceshipData.gfxSpriteAnimation);
		    }
		    BuildingInfoDisplay.isExtraInfoVisible=infoDisplayPrevValue;
	    }
    }

    void DelayedStart() {
	    NewGameWorldSetup.s.SetUpAfterShipLanding();
    }

    private bool isLookingForNewLandingSite = false;
    public void LookForNewLandingSite() {
	    var currentPlanetSave = DataSaver.s.GetSave().currentPlanet;
	    // Do a quick leave if we haven't landed yet. And if we have landed do a cool leave
	    if (currentPlanetSave.isSpaceshipLanded == false) {
		    TriggerNewLocation();
	    } else {
		    if (!isLeavingPlanet && !isLookingForNewLandingSite) {
			    GUI_SwitchController.s.HideAllMenus();
			    isLookingForNewLandingSite = true;
			    var shipGfx = Grid.s.GetTile(FactoryMaster.s.GetShip().center).visualObject.GetComponentInChildren<SpriteGraphicsController>();
			    shipGfx.DoSpaceLiftoff(TriggerNewLocation, spaceshipData.spaceLandingXDisp, spaceshipData.gfxSpriteAnimation);
		    }
	    }
    }

    void TriggerNewLocation() {
	    isLookingForNewLandingSite = false;
	    GameMaster.s.NewLocationInPlanet();
	    UpdatePlanetResourcesUI();
    }

    public Text planetName;
    public Text planetDesc;
    void UpdatePlanetResourcesUI() {
	    var planet = new Planet(DataSaver.s.mySave.currentPlanet.planetData);
	    planetName.text = $"Location #{planet.planetGenerationInt}";

	    var oreText = "Has ores:\n";

	    for (int i = 0; i < planet.oreDensities.Length; i++) {
		    var oreItem = DataHolder.s.GetItem(planet.oreNames[i]);
		    if (oreItem != null) {
			    oreText += $"{planet.oreDensities[i] + 1} x {oreItem.name} \n";
		    } else {
			    Debug.LogError($"Ore item doesn't exist {planet.oreNames[i]}");
		    }
	    }

	    oreText += "\n" + $"Has cards:\n";
	    
	    for (int i = 0; i < planet.myPresentCardNames.Length; i++) {
		    var shipCard = DataHolder.s.GetShipCard(planet.myPresentCardNames[i]);
		    if (shipCard != null) {
			    oreText += $"{shipCard.GetName()} \n";
		    } else {
			    Debug.LogError($"Unknown ship card {planet.myPresentCardNames}");
		    }
	    }

	    planetDesc.text = oreText;
    }

    private bool isLeavingPlanet = false;

    public void LeavePlanet() {
	    var currentPlanetSave = DataSaver.s.GetSave().currentPlanet;
	    // Do a quick leave if we haven't landed yet. And if we have landed do a cool leave
	    if (currentPlanetSave.isSpaceshipLanded == false) {
		    TriggerLeave();
	    } else {
		    if (!isLeavingPlanet && !isLookingForNewLandingSite) {
			    GUI_SwitchController.s.HideAllMenus();
			    isLeavingPlanet = true;
			    var shipGfx = Grid.s.GetTile(FactoryMaster.s.GetShip().center).visualObject.GetComponentInChildren<SpriteGraphicsController>();
			    shipGfx.DoSpaceLiftoff(TriggerLeave, spaceshipData.spaceLandingXDisp, spaceshipData.gfxSpriteAnimation);
		    }
	    }
    }

    void TriggerLeave() {
	    isLeavingPlanet = false;
	    GameMaster.s.LeavePlanet();
    }
}
