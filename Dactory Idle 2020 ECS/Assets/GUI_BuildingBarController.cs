using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_BuildingBarController : MonoBehaviour
{
    public BuildingData[] slotBuildings;
    public void GetBuildingInSlot (int i) {
        if (slotBuildings.Length > i) {
            Player_MasterControlCheck.s.buildingController.TryToPlaceItem(slotBuildings[i]);
        } else {
            throw new NullReferenceException("Building slots are not set!");
        }
    }

    public void SellMode () {

    }

    public void BeltMode () {

    }
}
