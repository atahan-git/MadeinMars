using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_BuildingBarController : MonoBehaviour
{
    public GameObject beltBuildingOverlay;

    public BuildingData[] slotBuildings;
    public void GetBuildingInSlot (int i) {
        beltBuildingOverlay.SetActive(false);
        if (slotBuildings.Length > i) {
            Player_MasterControlCheck.s.buildingController.TryToPlaceItem(slotBuildings[i]);
        } else {
            throw new NullReferenceException("Building slots are not set!");
        }
    }

    public void SellMode () {
        beltBuildingOverlay.SetActive(false);

    }

    public void BeltMode () {
        beltBuildingOverlay.SetActive(true);
        Player_MasterControlCheck.s.buildingController.ActivateBeltMode();
    }

    public void Deselect () {
        beltBuildingOverlay.SetActive(false);
        Player_MasterControlCheck.s.buildingController.Deselect();
    }
}
