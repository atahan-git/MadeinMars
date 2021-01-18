using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInventoryController : MonoBehaviour, IInventoryController {

    public enum InventoryType {
        NormalBuilding, Miner, Base
    }

    public InventoryType myType = InventoryType.NormalBuilding;
    public List<InventoryItemSlot> inventory;
    

    public event GenericCallback drawInventoryEvent;
    public event GenericCallback inventoryContentsChangedEvent;

    /// <summary>
    /// Sets up the building inventory with slots made for the possible inputs&outputs craftable by the building.
    /// Must be called after the crafting controller is set up
    /// </summary>
    /// <param name="mydat"></param>
    public void SetUp(BuildingCraftingController myCrafter, BuildingData myData) {
        switch (myData.myType) {
            case BuildingData.ItemType.Base:
                myType = InventoryType.Base;
                break;

            case BuildingData.ItemType.Miner:
                myType = InventoryType.Miner;
                break;
            default:
                myType = InventoryType.NormalBuilding;
                break;
        }

        switch (myType) {
            case InventoryType.NormalBuilding:
                
                for (int i = 0; i < myCrafter.myCraftingProcesses.Length; i++) {
                    var inputs = myCrafter.myCraftingProcesses[i].GetInputItems();
                    for (int m = 0; m < inputs.Length; m++) {
                        AddSlot(inputs[m].Item1, inputs[m].Item2*2, false);
                    }
                    var outputs = myCrafter.myCraftingProcesses[i].GetOutputItems();
                    for (int m = 0; m < outputs.Length; m++) {
                        AddSlot(outputs[m].Item1, outputs[m].Item2*2, true);
                    }
                }
                
                break;
            
            case InventoryType.Miner:

                var ores = DataHolder.s.GetAllOres();

                for (int i = 0; i < ores.Length; i++) {
                    AddSlot(DataHolder.s.GetItem(ores[i].oreUniqueName), -1, false);
                    AddSlot(DataHolder.s.GetItem(ores[i].oreUniqueName), -1, true);
                }
                
                break;
            
            case InventoryType.Base:
                inventory = Player_InventoryController.s.mySlots;
                break;
        }
        
        
        drawInventoryEvent?.Invoke();
        InventoryContentsChanged();
    }

    public void SetUp(List<InventoryItemSlot> _inventory) {
        myType = InventoryType.NormalBuilding;

        inventory = _inventory;

        drawInventoryEvent?.Invoke();
        InventoryContentsChanged();
    }
    
    
    /// <summary>
    /// Adds a slot to the building inventory. The building will not take items from the belts if it doesnt have inventory slot for it.
    /// Also it will stop production if it cannot put more output items into its slots
    /// </summary>
    /// <param name="item"></param>
    /// <param name="maxCount"></param>
    public void AddSlot (Item item, int maxCount, bool isOutput) {
        for (int i = 0; i < inventory.Count; i++) {
            if (inventory[i].myItem == item && inventory[i].isOutputSlot == isOutput) {
                inventory[i].maxCount = Mathf.Max(inventory[i].maxCount, maxCount);
                return;
            }
        }

        inventory.Add(new InventoryItemSlot(item,0,maxCount, isOutput));
    }

    
    /// <summary>
    /// Try to add an item to one of the slots. 
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Returns true or false depending on if there is available space for the particular item</returns>
    public bool TryAddItem(Item item, int amount) {
        for (int i = 0; i < inventory.Count; i++) {
            if (inventory[i].isOutputSlot) {
                if (inventory[i].myItem == item) {
                    if (inventory[i].count < inventory[i].maxCount || inventory[i].maxCount == -1) {
                        inventory[i].count += amount;
                        InventoryContentsChanged();
                        return true;
                    }
                }
            }
        }

        return false;
    }


    /// <summary>
    /// Checks if you can Add an item to the inventory based on storage available
    /// Does NOT actually add the items
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    /// <returns>Returns true or false depending on if there is available space for the particular item</returns>
    public bool CheckAddItem(Item item, int amount) {
        for (int i = 0; i < inventory.Count; i++) {
            if (inventory[i].isOutputSlot) {
                if (inventory[i].myItem == item) {
                    if (inventory[i].count < inventory[i].maxCount || inventory[i].maxCount == -1) {
                        return true;
                    }
                }
            }
        }

        return false;
    }
    
    /// <summary>
    /// Try to take an item from the available slots
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Returns true or false depending on if the item is available</returns>
    public bool TryTakeItem(Item item, int amount) {
        for (int i = 0; i < inventory.Count; i++) {
            if (!inventory[i].isOutputSlot) {
                if (inventory[i].myItem == item) {
                    if (inventory[i].count >= amount) {
                        inventory[i].count -= amount;
                        InventoryContentsChanged();
                        return true;
                    }
                }
            }
        }

        return false;
    }
    
    /// <summary>
    /// Checks if you can Take an item to the inventory based on storage available
    /// Does NOT actually take the items
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    /// <returns>Returns true or false depending on if the item is available</returns>
    public bool CheckTakeItem(Item item, int amount) {
        for (int i = 0; i < inventory.Count; i++) {
            if (!inventory[i].isOutputSlot) {
                if (inventory[i].myItem == item) {
                    if (inventory[i].count >= amount) {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public int GetAmountOfItems(Item item) {
        for (int i = 0; i < inventory.Count; i++) {
            if (inventory[i].myItem == item) {
                return inventory[i].count;
            }
        }

        return 0;
    }


    public void InventoryContentsChanged() {
        inventoryContentsChangedEvent?.Invoke();
    }
}
