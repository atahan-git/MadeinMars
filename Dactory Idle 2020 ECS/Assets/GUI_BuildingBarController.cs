using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_BuildingBarController : MonoBehaviour
{
    public GameObject beltBuildingOverlay;
    public GameObject sellModeOverlay;

    public BuildingData[] slotBuildings;
    public void GetBuildingInSlot (int i) {
        beltBuildingOverlay.SetActive(false);
        sellModeOverlay.SetActive(false);
        if (slotBuildings.Length > i) {
            Player_MasterControlCheck.s.buildingController.TryToPlaceItem(slotBuildings[i]);
        } else {
            throw new NullReferenceException("Building slots are not set!");
        }
    }

    public void SellMode () {
        beltBuildingOverlay.SetActive(false);
        sellModeOverlay.SetActive(true);
        Player_MasterControlCheck.s.buildingController.ActivateSellMode();
    }

    public void BeltMode () {
        beltBuildingOverlay.SetActive(true);
        sellModeOverlay.SetActive(false);
        Player_MasterControlCheck.s.buildingController.ActivateBeltMode();
    }

    public void Deselect () {
        beltBuildingOverlay.SetActive(false);
        sellModeOverlay.SetActive(false);
        Player_MasterControlCheck.s.buildingController.Deselect();
    }
}
