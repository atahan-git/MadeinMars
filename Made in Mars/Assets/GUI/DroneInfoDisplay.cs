using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneInfoDisplay : MonoBehaviour
{
    //public static bool isExtraInfoVisible = false; //Controlled by gui inventory controller
    // Uses building info display's static bool instead of its own

    DroneController myDrone;

    public GameObject canvas;

    public GameObject InventoryListingPrefab;
    public Transform InventoryParent;
    
    public bool isSetup = false;

    public Text droneInfoDisplay;
    private void Start() {
        isSetup = false;
        canvas.SetActive(isSetup);
        myDrone = GetComponent<DroneController>();
        myDrone.myInventory.drawInventoryEvent += SetUp;
        SetUp();
    }

    // Update is called once per frame
    void Update () {
        if (isSetup) {
            canvas.SetActive(BuildingInfoDisplay.isExtraInfoVisible);
        } //progressBar.value = (float)crafter.curCraftingProgress / (float)crafter.craftingProgressTickReq;

        if (myDrone.isBusy) {
            var missingMaterialName = "done!";
            int missingMaterialCount = 0;

            if (myDrone.myState == DroneController.DroneState.SearchingItem || myDrone.myState == DroneController.DroneState.TravellingToItem ) {
                for (int i = 0; i < myDrone.currentTask.materials.Length; i++) {
                    var difference = myDrone.currentTask.materials[i].maxCount - myDrone.currentTask.materials[i].count;
                    if (difference > 0) {
                        missingMaterialName = myDrone.currentTask.materials[i].myItem.uniqueName;
                        missingMaterialCount = difference;
                        break;
                    }
                }
                    
            }

            switch (myDrone.myState) {
                case DroneController.DroneState.SearchingItem:
                    
                    droneInfoDisplay.text = "Searching for building material: " + missingMaterialName + " x" + missingMaterialCount;
                    break;
                case DroneController.DroneState.TravellingToItem:
                    
                    droneInfoDisplay.text = "Travelling to collect: " + missingMaterialName + " x" + missingMaterialCount;
                    
                    break;
                case DroneController.DroneState.TravellingToBuild:
                    
                    
                    droneInfoDisplay.text = "Travelling to target Building";
                    
                    break;
                case DroneController.DroneState.Building:
                    
                    droneInfoDisplay.text = "Building";
                    break;
            }
        } else {
            droneInfoDisplay.text = "Idle";
        }
    }

    public void SetUp () {
        isSetup = true;
        int childs = InventoryParent.childCount;
        for (int i = childs - 1; i > 0; i--) {
            Destroy(InventoryParent.GetChild(i).gameObject);
        }
        

        if (myDrone.currentTask != null) {
            foreach (var itemRequirement in myDrone.currentTask.materials) {
                Instantiate(InventoryListingPrefab, InventoryParent).GetComponent<MiniGUI_InventoryListing>().SetUp(itemRequirement, myDrone.myInventory, true);
            }
            
            foreach (var itemRequirement in myDrone.myInventory.inventory) {
                Instantiate(InventoryListingPrefab, InventoryParent).GetComponent<MiniGUI_InventoryListing>().SetUp(itemRequirement, myDrone.myInventory, true);
            }
        } 
    }
    
}
