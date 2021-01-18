using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Controls the player inventory.
/// Deals with putting items in there, taking them out, and seeing if we have enough items to builds things that we want.
/// </summary>
public class Player_InventoryController : MonoBehaviour, IInventoryController {
    public static Player_InventoryController s;

    // You can assume that this reference is always kept, and you don't need to re-reference the list ever again!
    public List<InventoryItemSlot> mySlots = new List<InventoryItemSlot>();

    public static bool isInventoryLoadingDone = false;
    
    
    public event GenericCallback drawInventoryEvent;
    public event GenericCallback inventoryContentsChangedEvent;

    public bool cheatMode = false;
    private void Awake () {
        if (s != null) {
            Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
        }
        s = this;
        isInventoryLoadingDone = false;
        GameLoader.CallWhenLoaded(GameLoadingComplete);
    }

    // Start is called before the first frame update
    void Start () {
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
    // AKA the start of the game inventory.
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


    /// <summary>
    /// Try to add an item to the inventory, if there is empty space.
    /// I may replace this with infinite inventory later.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
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
        CraftingNode[] ps = DataHolder.s.GetCraftingProcessesOfType(BuildingData.ItemType.Building);
        if (cheatMode)
            return true;
        
        if (ps != null) {
            for (int i = 0; i < ps.Length; i++) {
                if (dat == null) {
                    print(i);
                    print(ps[i].outputs[0]);
                    print(dat);
                    print(dat.uniqueName);
                }
                if (ps[i].outputs[0].itemUniqueName == dat.uniqueName) {
                    for (int m = 0; m < ps[i].inputs.Count; m++) {
                        bool hasEnoughOfThisType = false;
                        for (int k = 0; k < mySlots.Count; k++) {
                            if (mySlots[k].myItem != null && mySlots[k].myItem.uniqueName == ps[i].inputs[m].itemUniqueName) {
                                if (mySlots[k].count >= ps[i].inputs[m].count) {
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
        CraftingNode[] ps = DataHolder.s.GetCraftingProcessesOfType(BuildingData.ItemType.Building);
        if (ps != null) {
            for (int i = 0; i < ps.Length; i++) {
                if (ps[i].outputs[0].itemUniqueName == dat.uniqueName) {
                    for (int m = 0; m < ps[i].inputs.Count; m++) {
                        for (int k = 0; k < mySlots.Count; k++) {
                            if (mySlots[k].myItem != null && mySlots[k].myItem.uniqueName == ps[i].inputs[m].itemUniqueName) {
                                mySlots[k].count -= ps[i].inputs[m].count * multiplier;
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


    /// <summary>
    /// Checks if you have enough resources to craft an item.
    /// Does NOT actually use the resources! 
    /// </summary>
    /// <param name="ps"></param>
    /// <returns></returns>
    public bool CanCraftItem (CraftingNode ps) {
        if (ps != null) {
            for (int m = 0; m < ps.inputs.Count; m++) {
                bool hasEnoughOfThisType = false;
                for (int k = 0; k < mySlots.Count; k++) {
                    if (mySlots[k].myItem != null && mySlots[k].myItem.uniqueName == ps.inputs[m].itemUniqueName) {
                        if (mySlots[k].count >= ps.inputs[m].count) {
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
    /// Also doesn't reward the resulting item. That needs to be done manually using TryAddItem
    /// Use multiplier == -1 for cancelling crafting
    /// </summary>
    public bool UseCraftingResources (CraftingNode ps, int multiplier) {
        if (ps != null) {
            for (int m = 0; m < ps.inputs.Count; m++) {
                for (int k = 0; k < mySlots.Count; k++) {
                    if (mySlots[k].myItem != null && mySlots[k].myItem.uniqueName == ps.inputs[m].itemUniqueName) {
                        mySlots[k].count -= ps.inputs[m].count * multiplier;
                    }
                }
            }
            return true;
        }
        return false;
    }


    /// <summary>
    /// Returns the amount of the item available in the inventory. Returns 0 if not found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int GetItemCount(Item item) {
        for (int i = 0; i < mySlots.Count; i++) {
            if (mySlots[i].myItem != null && mySlots[i].myItem == item) {
                return mySlots[i].count;
            }
        }
        return 0;
    }
    
	private void OnDestroy () {
        GameLoader.RemoveFromCall(GameLoadingComplete);
        DataSaver.saveEvent -= SaveInventory;
	}

}

public delegate void GenericCallback ();

public interface IInventoryController {
    event GenericCallback drawInventoryEvent; // This is for the initial drawing of the inventory. Only needs to be called when slot counts change
    
    event GenericCallback inventoryContentsChangedEvent; // Sign up to this if you want to be updated whenever the inventory contents are changed
}

