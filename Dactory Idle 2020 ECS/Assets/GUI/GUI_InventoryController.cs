using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_InventoryController : MonoBehaviour {

    public Transform BuildingsParent;
    public GameObject buildingListingPrefab;
    GUI_BuildingBarController bbar;

    // Start is called before the first frame update
    void Start () {
        bbar = GetComponent<GUI_BuildingBarController>();
        foreach (BuildingData dat in DataHolder.s.AllBuildings()) {
            Instantiate(buildingListingPrefab, BuildingsParent).GetComponent<MiniGUI_BuildingListing>().SetUp(dat, bbar);
        }
    }
}
