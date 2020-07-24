using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_InventoryController : MonoBehaviour {

    public static bool isExtraInfoVisible = true;

    public Transform BuildingsParent;
    public GameObject BuildingListingPrefab;
    GUI_BuildingBarController bbar;

    public Transform InventoryParent;
    public GameObject InventoryListingPrefab;
    Player_InventoryController pcont;

    // Start is called before the first frame update
    void Start () {
        if (Player_InventoryController.isInventoryLoadingDone)
            DrawInventory();
        else
            Player_InventoryController.drawInventoryEvent += DrawInventory;
    }

    void DrawInventory () {
        bbar = GetComponent<GUI_BuildingBarController>();
        foreach (BuildingData dat in DataHolder.s.AllBuildings()) {
            Instantiate(BuildingListingPrefab, BuildingsParent).GetComponent<MiniGUI_BuildingListing>().SetUp(dat, bbar);
        }

        pcont = transform.parent.GetComponentInChildren<Player_InventoryController>();
        foreach (InventoryItemSlot it in pcont.mySlots) {
            Instantiate(InventoryListingPrefab, InventoryParent).GetComponent<MiniGUI_InventoryListing>().SetUp(it, this);
        }
    }

    void OnDestroy () {
        Player_InventoryController.drawInventoryEvent -= DrawInventory;
    }

    public void ToggleExtraInfo () {
		isExtraInfoVisible = !isExtraInfoVisible;
        BuildingInfoDisplay.isExtraInfoVisible = isExtraInfoVisible;
    }
}

