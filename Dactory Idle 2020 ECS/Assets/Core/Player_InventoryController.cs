using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_InventoryController : MonoBehaviour {
    public static Player_InventoryController s;

    // You can assume that this reference is always kept, and you don't need to re-reference the list ever again!
    public List<InventoryItemSlot> mySlots = new List<InventoryItemSlot>();

    public static bool isInventoryLoadingDone = false;
    public delegate void GenericCallback ();
    public static event GenericCallback drawInventoryEvent;
    
    public static event GenericCallback inventoryContentsChangedEvent;

    private void Awake () {
        if (s != null) {
            Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
        }
        s = this;
        isInventoryLoadingDone = false;
    }

    // Start is called before the first frame update
    void Start () {
        GameLoader.CallWhenLoaded(GameLoadingComplete);
        DataSaver.saveEvent += SaveInventory;
    }
    
    void GameLoadingComplete () {
        if (GameLoader.isGameLoadingSuccessfull) {
            print("Loading Inventory");
            LoadInventory();
        } else {
            print("Creating Starter Inventory");
            InitializeStarterInventory();
        }
    }
    void LoadInventory () {
        DataSaver.InventoryData[] myDat = DataSaver.mySave.inventoryData;
        for (int i = 0; i < myDat.Length; i++) {
            if (myDat[i].uniqueName != "") {
                mySlots.Add(new InventoryItemSlot(
                    DataHolder.s.GetItem(myDat[i].uniqueName), myDat[i].count)
                );
            } else {
                mySlots.Add(new InventoryItemSlot());
            }
        }

        isInventoryLoadingDone = true;
        drawInventoryEvent?.Invoke();
    }

    public DataSaver.InventoryData[] startingInventory;
    public int startingInventorySlotCount = 42;

    void InitializeStarterInventory() {
        int totalItemCount = startingInventory.Length;
        for (int i = 0; i < totalItemCount; i++) {
            mySlots.Add(new InventoryItemSlot(
                DataHolder.s.GetItem(startingInventory[i].uniqueName), startingInventory[i].count)
            );
        }

        for (int i = 0; i < startingInventorySlotCount-totalItemCount; i++) {
            mySlots.Add(new InventoryItemSlot());
        }

        isInventoryLoadingDone = true;
        drawInventoryEvent?.Invoke();
    }
    

    void SaveInventory () {
        DataSaver.InventoryDataToBeSaved = new DataSaver.InventoryData[mySlots.Count];
        for (int i = 0; i < mySlots.Count; i++) {
            if (mySlots[i].myItem != null) {
                DataSaver.InventoryDataToBeSaved[i] =
                    new DataSaver.InventoryData(mySlots[i].myItem.uniqueName, mySlots[i].count);
            } else {
                DataSaver.InventoryDataToBeSaved[i] =
                    new DataSaver.InventoryData("", 0);
            }
        }
    }


    public bool TryAddItem (Item item) {
        for (int i = 0; i < mySlots.Count; i++) {
            if (mySlots[i].myItem == item) {
                mySlots[i].count++;
                inventoryContentsChangedEvent?.Invoke();
                return true;
            }
        }

        for (int i = 0; i < mySlots.Count; i++) {
            if (mySlots[i].count == 0) {
                mySlots[i].myItem = item;
                mySlots[i].count++;
                inventoryContentsChangedEvent?.Invoke();
                return true;
            }
        }
        return false;
    }

    // Ugly
    public bool CanPlaceBuilding (BuildingData dat) {
        CraftingProcessNode[] ps = DataHolder.s.GetCraftingProcessesOfType(BuildingData.ItemType.Building);
        if (ps != null) {
            for (int i = 0; i < ps.Length; i++) {
                if (dat == null) {
                    print(i);
                    print(ps[i].outputItemUniqueNames[0]);
                    print(dat);
                    print(dat.uniqueName);
                }
                if (ps[i].outputItemUniqueNames[0] == dat.uniqueName) {
                    for (int m = 0; m < ps[i].inputItemCounts.Count; m++) {
                        bool hasEnoughOfThisType = false;
                        for (int k = 0; k < mySlots.Count; k++) {
                            if (mySlots[k].myItem != null && mySlots[k].myItem.uniqueName == ps[i].inputItemUniqueNames[m]) {
                                if (mySlots[k].count >= ps[i].inputItemCounts[m]) {
                                    hasEnoughOfThisType = true;
                                    break;
                                }
                            }
                        }
                        if (!hasEnoughOfThisType) {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return true;
        } else {
            return true;
        }
    }

    /// <summary>
    /// Please note that this doesn't check if it does have enough resources or not for performance reasons.
    /// Unwanted behaviour will occur if you run this without enough resources
    /// Use multiplier == -1 for when selling a building
    /// </summary>
    public void UseBuildingResources (BuildingData dat, int multiplier) {
        CraftingProcessNode[] ps = DataHolder.s.GetCraftingProcessesOfType(BuildingData.ItemType.Building);
        if (ps != null) {
            for (int i = 0; i < ps.Length; i++) {
                if (ps[i].outputItemUniqueNames[0] == dat.uniqueName) {
                    for (int m = 0; m < ps[i].inputItemUniqueNames.Count; m++) {
                        for (int k = 0; k < mySlots.Count; k++) {
                            if (mySlots[k].myItem != null && mySlots[k].myItem.uniqueName == ps[i].inputItemUniqueNames[m]) {
                                mySlots[k].count -= ps[i].inputItemCounts[m] * multiplier;
                                break;
                            }
                        }
                    }
                    inventoryContentsChangedEvent?.Invoke();
                    return;
                }
            }
        } 
    }


    public bool CanCraftItem (CraftingProcessNode ps) {
        if (ps != null) {
            for (int m = 0; m < ps.inputItemCounts.Count; m++) {
                bool hasEnoughOfThisType = false;
                for (int k = 0; k < mySlots.Count; k++) {
                    if (mySlots[k].myItem != null && mySlots[k].myItem.uniqueName == ps.inputItemUniqueNames[m]) {
                        if (mySlots[k].count >= ps.inputItemCounts[m]) {
                            hasEnoughOfThisType = true;
                            break;
                        }
                    }
                }

                if (!hasEnoughOfThisType) {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Use crafting resources to craft an item. Please note that it doesn't check if you have enough resources or not
    /// Also doesn't reward the resulting item. That needs to be done manually
    /// Use multiplier == -1 for cancelling crafting
    /// </summary>
    public void UseCraftingResources (CraftingProcessNode ps, int multiplier) {
        
    }
    
	private void OnDestroy () {
        GameLoader.RemoveFromCall(GameLoadingComplete);
        DataSaver.saveEvent -= SaveInventory;
	}
}

