using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Controls the inventoryItemSlots UI panel
/// </summary>
public class GUI_InventoryController : MonoBehaviour {

    public static bool isExtraInfoVisible = false;

    public Transform BuildingsParent;
    public GameObject BuildingListingPrefab;
    GUI_BuildingBarController bbar;


    void Start() {
        DrawInventory();

        if (PlayerPrefs.GetInt("extrainfo", 0) == 1) {
            ToggleExtraInfo();
        }
    }

    void DrawInventory () {
        print("Drawing inventoryItemSlots");
        BuildingsParent.DeleteAllChildren();
        
        // Draw the buildings
        bbar = GetComponent<GUI_BuildingBarController>();
        foreach (BuildingData dat in DataHolder.s.AllBuildings()) {
            if(dat.playerBuildBarApplicable)
                Instantiate(BuildingListingPrefab, BuildingsParent).GetComponent<MiniGUI_BuildingListing>().SetUp(dat, bbar);
        }
        print(DataHolder.s.AllBuildings().Length.ToString() + " Buildings are put into building list");
        
    }

    public void ToggleExtraInfo () {
		isExtraInfoVisible = !isExtraInfoVisible;
        BuildingInfoDisplay.isExtraInfoVisible = isExtraInfoVisible;
        PlayerPrefs.SetInt("extrainfo", isExtraInfoVisible ? 1 : 0);
    }
}

