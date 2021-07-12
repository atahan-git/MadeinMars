using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneInfoDisplay : MonoBehaviour
{
    //public static bool isExtraInfoVisible = false; //Controlled by gui inventoryItemSlots controller
    // Uses building info display's static bool instead of its own

    Drone myDrone;

    public GameObject canvas;

    public GameObject InventoryListingPrefab;
    public Transform InventoryParent;
    
    public bool isSetup = false;

    public Text droneInfoDisplay;
    
    
    private void Start() {
        var worldObject = GetComponent<DroneWorldObject>();
        if (worldObject.isInventorySetup) {
            SetUp();
        } else {
            worldObject.buildingInventoryUpdatedCallback += SetUp;
        }
    }
    
    
    public void SetUp() {
        isSetup = true;
        canvas.SetActive(isSetup);
        myDrone = GetComponent<DroneWorldObject>().myDrone;
        myDrone.myInventory.drawInventoryEvent += DrawInventory;
        DrawInventory();
    }

    // Update is called once per frame
    void Update () {
        if (isSetup) {
            canvas.SetActive(BuildingInfoDisplay.isExtraInfoVisible);

        /*if (myDrone.isBusy) {
            var missingMaterialName = "done!";
            int missingMaterialCount = 0;

            if (myDrone.myState == Drone.DroneState.SearchingItem || myDrone.myState == Drone.DroneState.TravellingToItem ||
                myDrone.myState == Drone.DroneState.TakingItem || myDrone.myState == Drone.DroneState.UnableToFindConstructionItem) {
                for (int i = 0; i < myDrone.currentTask.materials.Count; i++) {
                    var difference = myDrone.currentTask.materials[i].maxCount - myDrone.currentTask.materials[i].count;
                    if (difference > 0) {
                        missingMaterialName = myDrone.currentTask.materials[i].myItem.uniqueName;
                        missingMaterialCount = difference;
                        break;
                    }
                }
            }*/


            droneInfoDisplay.text = myDrone.myState.GetInfoDisplayText(myDrone);
            /*switch (myDrone.myState) {
                case Drone.DroneState.idle:
                    droneInfoDisplay.text = "Idle";
                    break;
                
                case Drone.DroneState.BeginBuildingTask:
                    droneInfoDisplay.text = "Starting building";
                    break;
                    
                case Drone.DroneState.SearchingItem:
                    droneInfoDisplay.text = "Searching for building material: " + missingMaterialName + " x" + missingMaterialCount;
                    break;
                
                case Drone.DroneState.TravellingToItem:
                    droneInfoDisplay.text = "Travelling to collect: " + missingMaterialName + " x" + missingMaterialCount;
                    break;
                
                case Drone.DroneState.TakingItem:
                    droneInfoDisplay.text = "Collecting: " + missingMaterialName + " x" + missingMaterialCount;
                    break;
                
                case Drone.DroneState.TravellingToBuild:
                    droneInfoDisplay.text = "Travelling to Build : " + myDrone.currentTask.construction.myData.uniqueName;
                    break;
                
                case Drone.DroneState.Building:
                    droneInfoDisplay.text = "Building : " + myDrone.currentTask.construction.myData.uniqueName;
                    break;
                
                case Drone.DroneState.SearchingToEmptyInventory:
                    droneInfoDisplay.text = "Search to Empty Inventory";
                    break;
                
                case Drone.DroneState.TravellingToEmptyInventory:
                    droneInfoDisplay.text = "Travelling to Empty Inventory";
                    break;
                
                case Drone.DroneState.EmptyingInventory:
                    droneInfoDisplay.text = "Emptying Inventory";
                    break;
                
                case Drone.DroneState.BeingDestructionTask:
                    droneInfoDisplay.text = "Beginning Destruction Task";
                    break;
                
                case Drone.DroneState.TravellingToDestroy:
                    droneInfoDisplay.text = "Travelling to Destroy : " + myDrone.currentTask.construction.myData.uniqueName;
                    break;
                
                case Drone.DroneState.Destroying:
                    droneInfoDisplay.text = "Destroying : " + myDrone.currentTask.construction.myData.uniqueName;
                    break;
                
                case Drone.DroneState.UnableToFindConstructionItem :
                    droneInfoDisplay.text = "Cannot find building material in storage buildings: " + missingMaterialName + " x" + missingMaterialCount;
                    break;
                
                case Drone.DroneState.UnableToFindEmptyStorage :
                    droneInfoDisplay.text = "Cannot find empty space in storage buildings";
                    break;
            }
        } else {
            droneInfoDisplay.text = "Idle";
        }*/
        } //progressBar.value = (float)crafter.curCraftingProgress / (float)crafter.craftingProgressTickReq;
    }

    public void DrawInventory () {
        InventoryParent.DeleteAllChildren();
        
        if (myDrone.currentTask != null) {
            foreach (var itemRequirement in myDrone.myInventory.inventoryItemSlots) {
                Instantiate(InventoryListingPrefab, InventoryParent).GetComponent<MiniGUI_InventoryListing>().SetUp(itemRequirement, myDrone.myInventory, true);
            }
        } 
    }
    
}
