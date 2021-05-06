using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// This controls the inventory in all buildings, even in those who cannot craft
/// </summary>
[Serializable]
public class BuildingInventoryController : IInventoryController {

    public Position myLocation;
    
    public enum InventoryType {
        NormalBuilding, Miner, Base, Storage, Construction
    }

    public InventoryType myType = InventoryType.Construction;
    public List<InventoryItemSlot> inventory;
    

    public event GenericCallback drawInventoryEvent;
    public event GenericCallback inventoryContentsChangedEvent;

    public bool isCheatInventory = false;


    /// <summary>
    /// Sets up the inventory for construction
    /// </summary>
    /// <param name="location"></param>
    public void SetUpConstruction(Position location) {
        inventory = new List<InventoryItemSlot>();
        myLocation = location;
        
        myType = InventoryType.Construction;
        drawInventoryEvent?.Invoke();
        InventoryContentsChanged();
    }

    /// <summary>
    /// Sets up the building inventory with slots made for the possible inputs&outputs craftable by the building.
    /// Must be called after the crafting controller is set up
    /// </summary>
    /// <param name="mydat"></param>
    public void SetUp(Position location, BuildingCraftingController myCrafter, BuildingData myData) {
        if(myType != InventoryType.Construction)
            Debug.LogError("trying to setup the inventory after being setup once. This is not allowed!");
        myLocation = location;
        
        switch (myData.myType) {
            case BuildingData.ItemType.Base:
                myType = InventoryType.Base;
                break;

            case BuildingData.ItemType.Miner:
                //myType = InventoryType.Miner;
                myType = InventoryType.NormalBuilding;
                break;
            case BuildingData.ItemType.Storage:
                myType = InventoryType.Storage;
                break;
            default:
                myType = InventoryType.NormalBuilding;
                break;
        }

        // If this is not a storage type, we need to remove the extra slots leftover from construction
        if (myType != InventoryType.Storage) {
            for (int i = inventory.Count-1; i >=0 ; i--) {
                if (inventory[i].mySlotType == InventoryItemSlot.SlotType.storage) {
                    inventory.RemoveAt(i);
                }
            }
        }

        switch (myType) {
            case InventoryType.NormalBuilding:

                for (int i = 0; i < myCrafter.myCraftingProcesses.Length; i++) {
                    var inputs = myCrafter.myCraftingProcesses[i].GetInputItems();
                    for (int m = 0; m < inputs.Length; m++) {
                        AddSlot(inputs[m].Item1, inputs[m].Item2 * 2, InventoryItemSlot.SlotType.input);
                    }

                    var outputs = myCrafter.myCraftingProcesses[i].GetOutputItems();
                    for (int m = 0; m < outputs.Length; m++) {
                        AddSlot(outputs[m].Item1, outputs[m].Item2 * 2, InventoryItemSlot.SlotType.output);
                    }
                }

                break;

            case InventoryType.Miner:

                var ores = DataHolder.s.GetAllOres();

                for (int i = 0; i < ores.Length; i++) {
                    AddSlot(DataHolder.s.GetItem(ores[i].oreUniqueName), -1, InventoryItemSlot.SlotType.input);
                    AddSlot(DataHolder.s.GetItem(ores[i].oreUniqueName), -1, InventoryItemSlot.SlotType.output);
                }

                break;

            case InventoryType.Storage:

                // only fill in as much as we need. some slots may be leftover from construction.
                for (int i = inventory.Count; i < myData.buildingGrade; i++) {
                    AddSlot(Item.GetEmpty(), 99, InventoryItemSlot.SlotType.storage);
                }
                
                DroneSystem.s.RegisterStorageBuilding(this);
                
                break;
        }


        drawInventoryEvent?.Invoke();
        InventoryContentsChanged();
    }

    void OnDestroy () {
        DroneSystem.s.RemoveStorageBuilding(this);
    }


    public void SetInventory(List<InventoryItemSlot> _inventory) {
        inventory = _inventory;

        drawInventoryEvent?.Invoke();
        InventoryContentsChanged();
    }
    
    
    /// <summary>
    /// Adds a slot to the building inventory. The building will not take items from the belts if it doesnt have inventory slot for it.
    /// Also it will stop production if it cannot put more output items into its slots
    /// </summary>
    /// <param name="itemReference"></param>
    /// <param name="maxCount"></param>
    public void AddSlot (Item item, int maxCount, InventoryItemSlot.SlotType slotType) {
        if (slotType != InventoryItemSlot.SlotType.storage) {
            for (int i = 0; i < inventory.Count; i++) {
                if (inventory[i].myItem == item && inventory[i].mySlotType == slotType) {
                    inventory[i].maxCount = Mathf.Max(inventory[i].maxCount, maxCount);
                    return;
                }
            }
        }

        inventory.Add(new InventoryItemSlot(item,0,maxCount, slotType));
    }

    
    /// <summary>
    /// Try to add an item to one of the slots. 
    /// </summary>
    /// <param name="itemReference"></param>
    /// <returns>Returns true or false depending on if there is available space for the particular item</returns>
    public bool TryAddItem(Item item, int amount, bool isOutput) {
        return ForceAddItem(item, amount, isOutput, false);
    }
    
    /// <summary>
    /// Try to add an item to one of the slots. 
    /// </summary>
    /// <param name="itemReference"></param>
    /// <returns>Returns true or false depending on if there is available space for the particular item</returns>
    public bool ForceAddItem(Item item, int amount, bool isOutput, bool isForced) {
        var slotType = isOutput ? InventoryItemSlot.SlotType.output : InventoryItemSlot.SlotType.input;
        for (int i = 0; i < inventory.Count; i++) {
            if (inventory[i].mySlotType == slotType) {
                if (inventory[i].myItem == item) {
                    if (inventory[i].count + amount <= inventory[i].maxCount || inventory[i].maxCount == -1) {
                        inventory[i].count += amount;
                        InventoryContentsChanged();
                        return true;
                    }
                }
                
                
            } else if(inventory[i].mySlotType == InventoryItemSlot.SlotType.storage) {
                if (inventory[i].myItem.isEmpty()) {
                    inventory[i].myItem = item;
                    inventory[i].maxCount = item.inventoryStackCount;
                }
                
                if (inventory[i].myItem == item) {
                    if (inventory[i].count + amount <= inventory[i].maxCount || inventory[i].maxCount == -1) {
                        inventory[i].count += amount;
                        InventoryContentsChanged();
                        return true;
                    }
                }
            }
        }

        if (!isForced) {
            return false;
        } else {
            AddSlot(item,99, InventoryItemSlot.SlotType.storage);
            return ForceAddItem(item, amount, isOutput, isForced);
        }
    }


    /// <summary>
    /// Checks if you can Add an item to the inventory based on storage available
    /// Does NOT actually add the items
    /// </summary>
    /// <param name="itemReference"></param>
    /// <param name="amount"></param>
    /// <returns>Returns true or false depending on if there is available space for the particular item</returns>
    public bool CheckAddItem(Item item, int amount, bool isOutput) {
        var slotType = isOutput ? InventoryItemSlot.SlotType.output : InventoryItemSlot.SlotType.input;
        for (int i = 0; i < inventory.Count; i++) {
            if (inventory[i].mySlotType == slotType) {
                if (inventory[i].myItem == item) {
                    if (inventory[i].count + amount <= inventory[i].maxCount || inventory[i].maxCount == -1) {
                        return true;
                    }
                }
                
                
            }else if(inventory[i].mySlotType == InventoryItemSlot.SlotType.storage) {
                if (inventory[i].myItem.isEmpty()) {
                    inventory[i].maxCount = item.inventoryStackCount;
                }
                
                if (inventory[i].myItem == item || inventory[i].myItem.isEmpty()) {
                    if (inventory[i].count + amount <= inventory[i].maxCount || inventory[i].maxCount == -1) {
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
    /// <param name="itemReference"></param>
    /// <returns>Returns true or false depending on if the item is available</returns>
    public bool TryTakeItem(Item item, int amount, bool isOutput) {
        if (isCheatInventory)
            return true;
        
        var slotType = isOutput ? InventoryItemSlot.SlotType.output : InventoryItemSlot.SlotType.input;
        for (int i = 0; i < inventory.Count; i++) {
            if (inventory[i].mySlotType == slotType) {
                if (inventory[i].myItem == item) {
                    if (inventory[i].count >= amount) {
                        inventory[i].count -= amount;
                        InventoryContentsChanged();
                        return true;
                    }
                }
                
                
                
            }else if(inventory[i].mySlotType == InventoryItemSlot.SlotType.storage) {
                if (inventory[i].myItem == item) {
                    if (inventory[i].count >= amount) {
                        inventory[i].count -= amount;
                        
                        if (inventory[i].count == 0) {
                            inventory[i].myItem = Item.GetEmpty();
                        }
                        
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
    /// <param name="itemReference"></param>
    /// <param name="amount"></param>
    /// <returns>Returns true or false depending on if the item is available</returns>
    public bool CheckTakeItem(Item item, int amount, bool isOutput) {
        if (isCheatInventory)
            return true;
        var slotType = isOutput ? InventoryItemSlot.SlotType.output : InventoryItemSlot.SlotType.input;
        for (int i = 0; i < inventory.Count; i++) {
            if (inventory[i].mySlotType == slotType) {
                if (inventory[i].myItem == item) {
                    if (inventory[i].count >= amount) {
                        return true;
                    }
                }
            }else if(inventory[i].mySlotType == InventoryItemSlot.SlotType.storage) {
                if (inventory[i].myItem == item) {
                    if (inventory[i].count >= amount) {
                        return true;
                    }
                }
            }
        }

        return false;
    }


    /// <summary>
    /// isOutput agnostic version of try take next item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool TryTakeNextItem(out Item item) {
        bool trial = TryTakeNextItem(out item, true);
        if (trial) {
            return trial;
        } else {
            trial = TryTakeNextItem(out item, false);
        }
        
        if(trial){
            return trial;
        } else {
            return false;
        }
    }
    
    /// <summary>
    /// Will try to take the next available item from any of the output slots
    /// </summary>
    /// <returns></returns>
    public bool TryTakeNextItem(out Item item, bool isOutput) {
        var slotType = isOutput ? InventoryItemSlot.SlotType.output : InventoryItemSlot.SlotType.input;
        for (int i = 0; i < inventory.Count; i++) {
            if (inventory[i].mySlotType == slotType) {
                if (inventory[i].count > 0) {
                    inventory[i].count -= 1;
                    item = inventory[i].myItem;
                    InventoryContentsChanged();
                    return true;
                }
                
            }else if(inventory[i].mySlotType == InventoryItemSlot.SlotType.storage) {
                if (!inventory[i].myItem.isEmpty()) {
                    if (inventory[i].count > 0) {
                        inventory[i].count -= 1;
                        item = inventory[i].myItem;
                        
                        if (inventory[i].count == 0) {
                            inventory[i].myItem = Item.GetEmpty();
                        }
                        
                        InventoryContentsChanged();
                        return true;
                    }
                }
            }
        }

        item = null;
        return false;
    }
    
    /// <summary>
    /// isOutput agnostic version of check take next item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool CheckTakeNextItem(out Item item) {
        bool trial = CheckTakeNextItem(out item, true);
        if (trial) {
            return trial;
        } else {
            trial = CheckTakeNextItem(out item, false);
        }
        
        if(trial){
            return trial;
        } else {
            return false;
        }
    }
    
    /// <summary>
    /// Checks which item would be removed from the building next without actually removing it
    /// </summary>
    /// <returns></returns>
    public bool CheckTakeNextItem(out Item item, bool isOutput) {
        var slotType = isOutput ? InventoryItemSlot.SlotType.output : InventoryItemSlot.SlotType.input;
        for (int i = 0; i < inventory.Count; i++) {
            if (inventory[i].mySlotType == slotType) {
                if (inventory[i].count > 0) {
                    item = inventory[i].myItem;
                    return true;
                }
                
            }else if(inventory[i].mySlotType == InventoryItemSlot.SlotType.storage) {
                if (!inventory[i].myItem.isEmpty()) {
                    if (inventory[i].count > 0) {
                        item = inventory[i].myItem;
                        return true;
                    }
                }
            }
        }

        item = null;
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
    
    public int GetTotalAmountOfItems(bool isOutput) {
        var slotType = isOutput ? InventoryItemSlot.SlotType.output : InventoryItemSlot.SlotType.input;
        int counter = 0;
        for (int i = 0; i < inventory.Count; i++) {
            if (inventory[i].mySlotType == slotType || inventory[i].mySlotType == InventoryItemSlot.SlotType.storage) {
                counter += inventory[i].count;
            }
        }

        return counter;
    }
    
    public int GetTotalAmountOfItems() {
        int counter = 0;
        for (int i = 0; i < inventory.Count; i++) {
            counter += inventory[i].count;
        }

        return counter;
    }


    public void InventoryContentsChanged() {
        inventoryContentsChangedEvent?.Invoke();
    }
}
