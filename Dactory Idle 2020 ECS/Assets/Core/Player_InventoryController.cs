using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_InventoryController : MonoBehaviour {
    public static Player_InventoryController s;

    public List<InventoryItemSlot> mySlots = new List<InventoryItemSlot>();
    int itemSlots = 40;

    public static bool isInventoryLoadingDone = false;
    public delegate void GenericCallback ();
    public static event GenericCallback drawInventoryEvent;


    public static event GenericCallback inventoryContentsChangedEvent;

    private void Awake () {
        if (s != null) {
            Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
        }
        s = this;
    }

    // Start is called before the first frame update
    void Start () {
        GameLoader.CallWhenLoaded(LoadInventory);
        DataSaver.saveEvent += SaveInventory;
    }

    void LoadInventory () {
        int totalItemCount = DataHolder.s.TotalItemCount();
        for (int i = 0; i < totalItemCount; i++) {
            mySlots.Add(new InventoryItemSlot(DataHolder.s.GetItem(i), 80));
        }

        for (int i = 0; i < itemSlots-totalItemCount; i++) {
            mySlots.Add(new InventoryItemSlot());
        }

        isInventoryLoadingDone = true;
        drawInventoryEvent?.Invoke();
    }

    void SaveInventory () {
        
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
    /// Use multipler == -1 for when selling a building
    /// </summary>
    /// <param name="dat"></param>
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

	private void OnDestroy () {
        GameLoader.RemoveFromCall(LoadInventory);
        DataSaver.saveEvent -= SaveInventory;
	}
}

