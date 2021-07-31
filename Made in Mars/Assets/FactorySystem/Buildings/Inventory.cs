using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[Serializable]
public class Inventory : IInventoryDisplayable, IInventoryWithSlots, IInventorySimulation {
    public List<InventoryItemSlot> inventoryItemSlots = new List<InventoryItemSlot>();
    public event GenericCallback drawInventoryEvent;
    public event GenericCallback inventoryContentsChangedEvent;

    public bool isCheatInventory = false;

    public bool isGrid = false;
    public int columnCount = 10;

    public void SetUp(List<InventoryItemSlot> existingItems) {
        inventoryItemSlots.Clear();

        if (existingItems != null) {
            foreach (var slot in existingItems) {
                RestoreSlot(slot, false);
            }
        }

        drawInventoryEvent?.Invoke();
        InventoryContentsChanged();
    }

    public void ReDrawInventory() {
        drawInventoryEvent?.Invoke();
    }


    public void SetInventory(List<InventoryItemSlot> _inventory) {
        if (_inventory == null)
            inventoryItemSlots.Clear();
        else
            inventoryItemSlots = _inventory;

        drawInventoryEvent?.Invoke();
        InventoryContentsChanged();
    }

    
    void RestoreSlot(InventoryItemSlot slot, bool reDrawInventory = true, bool addSlot = false) {
        bool addedSlot = false;
        for (int i = 0; i < inventoryItemSlots.Count; i++) {
            if (inventoryItemSlots[i].count == 0) {
                if (slot.mySlotType != InventoryItemSlot.SlotType.storage) {
                    if (inventoryItemSlots[i].mySlotType == slot.mySlotType && inventoryItemSlots[i].myItem == slot.myItem ) {
                        inventoryItemSlots[i].count = slot.count;
                        addedSlot = true;
                        break;
                    }
                } else {
                    if (inventoryItemSlots[i].mySlotType == InventoryItemSlot.SlotType.storage) {
                        inventoryItemSlots[i].myItem = slot.myItem;
                        inventoryItemSlots[i].count = slot.count;
                        addedSlot = true;
                        break;
                    }
                }
            }
        }

        if (addSlot) {
            if (!addedSlot) {
                inventoryItemSlots.Add(slot);
            }
        }

        if (reDrawInventory) {
            drawInventoryEvent?.Invoke();
            InventoryContentsChanged();
        }
    }
    
    public InventoryItemSlot AddSlot (Item item, int maxCount, InventoryItemSlot.SlotType slotType, bool reDrawInventory = true) {
        var slotId = -1;
        if (slotType != InventoryItemSlot.SlotType.storage) {
            for (int i = 0; i < inventoryItemSlots.Count; i++) {
                if (inventoryItemSlots[i].mySlotType == slotType && inventoryItemSlots[i].myItem == item) {
                    inventoryItemSlots[i].maxCount = Mathf.Max(inventoryItemSlots[i].maxCount, maxCount);
                    slotId = i;
                    break;
                }
            }
        }

        inventoryItemSlots.Add(new InventoryItemSlot(item,0,maxCount, slotType));
        slotId = inventoryItemSlots.Count - 1;
        if (reDrawInventory) {
            drawInventoryEvent?.Invoke();
            InventoryContentsChanged();
        }

        return inventoryItemSlots[slotId];
    }

    
    public bool TryAndAddItem(Item item, int amount, bool isOutput = false) {
        return TryAndAddItem(item, amount, isOutput, false);
    }
    
    public bool TryAndAddItem(Item item, int amount, bool isOutput, bool isForced) {
        var slotType = isOutput ? InventoryItemSlot.SlotType.output : InventoryItemSlot.SlotType.input;
        for (int i = 0; i < inventoryItemSlots.Count; i++) {
            if (inventoryItemSlots[i].mySlotType == slotType) {
                if (inventoryItemSlots[i].myItem == item) {
                    if (inventoryItemSlots[i].count + amount <= inventoryItemSlots[i].maxCount || inventoryItemSlots[i].maxCount == -1) {
                        inventoryItemSlots[i].count += amount;
                        InventoryContentsChanged();
                        return true;
                    }
                }
                
                
            } else if(inventoryItemSlots[i].mySlotType == InventoryItemSlot.SlotType.storage) {
                if (inventoryItemSlots[i].myItem.isEmpty()) {
                    inventoryItemSlots[i].myItem = item;
                    inventoryItemSlots[i].maxCount = item.inventoryStackCount;
                }
                
                if (inventoryItemSlots[i].myItem == item) {
                    if (inventoryItemSlots[i].count + amount <= inventoryItemSlots[i].maxCount || inventoryItemSlots[i].maxCount == -1) {
                        inventoryItemSlots[i].count += amount;
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
            return TryAndAddItem(item, amount, isOutput, isForced);
        }
    }
    
    public bool CheckIfCanAddItem(Item item, int amount, bool isOutput) {
        var slotType = isOutput ? InventoryItemSlot.SlotType.output : InventoryItemSlot.SlotType.input;
        for (int i = 0; i < inventoryItemSlots.Count; i++) {
            if (inventoryItemSlots[i].mySlotType == slotType) {
                if (inventoryItemSlots[i].myItem == item) {
                    if (inventoryItemSlots[i].count + amount <= inventoryItemSlots[i].maxCount || inventoryItemSlots[i].maxCount == -1) {
                        return true;
                    }
                }
                
                
            }else if(inventoryItemSlots[i].mySlotType == InventoryItemSlot.SlotType.storage) {
                if (inventoryItemSlots[i].myItem.isEmpty()) {
                    inventoryItemSlots[i].maxCount = item.inventoryStackCount;
                }
                
                if (inventoryItemSlots[i].myItem == item || inventoryItemSlots[i].myItem.isEmpty()) {
                    if (inventoryItemSlots[i].count + amount <= inventoryItemSlots[i].maxCount || inventoryItemSlots[i].maxCount == -1) {
                        return true;
                    }
                }
            }
        }

        return false;
    }
    
    public bool TryAndTakeItem(Item item, int amount, bool isOutput) {
        if (isCheatInventory)
            return true;
        
        var slotType = isOutput ? InventoryItemSlot.SlotType.output : InventoryItemSlot.SlotType.input;
        for (int i = 0; i < inventoryItemSlots.Count; i++) {
            if (inventoryItemSlots[i].mySlotType == slotType) {
                if (inventoryItemSlots[i].myItem == item) {
                    if (inventoryItemSlots[i].count >= amount) {
                        inventoryItemSlots[i].count -= amount;
                        InventoryContentsChanged();
                        return true;
                    }
                }
                
                
                
            }else if(inventoryItemSlots[i].mySlotType == InventoryItemSlot.SlotType.storage) {
                if (inventoryItemSlots[i].myItem == item) {
                    if (inventoryItemSlots[i].count >= amount) {
                        inventoryItemSlots[i].count -= amount;
                        
                        if (inventoryItemSlots[i].count == 0) {
                            inventoryItemSlots[i].myItem = Item.GetEmpty();
                        }
                        
                        InventoryContentsChanged();
                        return true;
                    }
                }
            }
        }

        return false;
    }
    
    public bool CheckIfCanTakeItem(Item item, int amount, bool isOutput) {
        if (isCheatInventory)
            return true;
        var slotType = isOutput ? InventoryItemSlot.SlotType.output : InventoryItemSlot.SlotType.input;
        for (int i = 0; i < inventoryItemSlots.Count; i++) {
            if (inventoryItemSlots[i].mySlotType == slotType) {
                if (inventoryItemSlots[i].myItem == item) {
                    if (inventoryItemSlots[i].count >= amount) {
                        return true;
                    }
                }
            }else if(inventoryItemSlots[i].mySlotType == InventoryItemSlot.SlotType.storage) {
                if (inventoryItemSlots[i].myItem == item) {
                    if (inventoryItemSlots[i].count >= amount) {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool TryAndTakeNextItem(out Item item) {
        bool trial = TryAndTakeNextItem(out item, true);
        if (trial) {
            return trial;
        } else {
            trial = TryAndTakeNextItem(out item, false);
        }
        
        if(trial){
            return trial;
        } else {
            return false;
        }
    }
    
    public bool TryAndTakeNextItem(out Item item, bool isOutput) {
        var slotType = isOutput ? InventoryItemSlot.SlotType.output : InventoryItemSlot.SlotType.input;
        for (int i = 0; i < inventoryItemSlots.Count; i++) {
            if (inventoryItemSlots[i].mySlotType == slotType) {
                if (inventoryItemSlots[i].count > 0) {
                    inventoryItemSlots[i].count -= 1;
                    item = inventoryItemSlots[i].myItem;
                    InventoryContentsChanged();
                    return true;
                }
                
            }else if(inventoryItemSlots[i].mySlotType == InventoryItemSlot.SlotType.storage) {
                if (!inventoryItemSlots[i].myItem.isEmpty()) {
                    if (inventoryItemSlots[i].count > 0) {
                        inventoryItemSlots[i].count -= 1;
                        item = inventoryItemSlots[i].myItem;
                        
                        if (inventoryItemSlots[i].count == 0) {
                            inventoryItemSlots[i].myItem = Item.GetEmpty();
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
    
    public bool CheckIfCanTakeNextItem(out Item item) {
        bool trial = CheckIfCanTakeNextItem(out item, true);
        if (trial) {
            return trial;
        } else {
            trial = CheckIfCanTakeNextItem(out item, false);
        }
        
        if(trial){
            return trial;
        } else {
            return false;
        }
    }
    
    
    public bool CheckIfCanTakeNextItem(out Item item, bool isOutput) {
        var slotType = isOutput ? InventoryItemSlot.SlotType.output : InventoryItemSlot.SlotType.input;
        for (int i = 0; i < inventoryItemSlots.Count; i++) {
            if (inventoryItemSlots[i].mySlotType == slotType) {
                if (inventoryItemSlots[i].count > 0) {
                    item = inventoryItemSlots[i].myItem;
                    return true;
                }
                
            }else if(inventoryItemSlots[i].mySlotType == InventoryItemSlot.SlotType.storage) {
                if (!inventoryItemSlots[i].myItem.isEmpty()) {
                    if (inventoryItemSlots[i].count > 0) {
                        item = inventoryItemSlots[i].myItem;
                        return true;
                    }
                }
            }
        }

        item = null;
        return false;
    }
    
    
    public int GetAmountOfItems(Item item) {
        for (int i = 0; i < inventoryItemSlots.Count; i++) {
            if (inventoryItemSlots[i].myItem == item) {
                return inventoryItemSlots[i].count;
            }
        }
        return 0;
    }
    
    public int GetTotalAmountOfItems(bool isOutput) {
        var slotType = isOutput ? InventoryItemSlot.SlotType.output : InventoryItemSlot.SlotType.input;
        int counter = 0;
        for (int i = 0; i < inventoryItemSlots.Count; i++) {
            if (inventoryItemSlots[i].mySlotType == slotType || inventoryItemSlots[i].mySlotType == InventoryItemSlot.SlotType.storage) {
                counter += inventoryItemSlots[i].count;
            }
        }

        return counter;
    }
    
    public int GetTotalAmountOfItems() {
        int counter = 0;
        for (int i = 0; i < inventoryItemSlots.Count; i++) {
            counter += inventoryItemSlots[i].count;
        }

        return counter;
    }


    void InventoryContentsChanged() {
        inventoryContentsChangedEvent?.Invoke();
    }
}
