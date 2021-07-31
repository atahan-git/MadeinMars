using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


/// <summary>
/// These are sets of items that are used in recipe sets.
/// The idea is that mods will be able to introduce their own itemsets, and use them in recipes.
/// </summary>
[CreateAssetMenu(menuName = "RecipeSystem/ItemSet")]
public class ItemSet : ScriptableObject {
    public Item[] items = new Item[10];

    /// <summary>
    /// Returns Item based on uniqueName
    /// Returns null if not found
    /// </summary>
    /// <returns>Item or null</returns>
    public Item GetItem(string uniqueName) {
        for (int i = 0; i < items.Length; i++) {
            if (items[i] != null)
                if (items[i].uniqueName == uniqueName) {
                    return items[i];
                }
        }

        return null;
    }
}


[Serializable]
public class InventoryItemSlot{
    public Item myItem;
    public int count = 0; // If 0, means the slot is empty.
    public int maxCount = -1; // -1 means no limit

    // Make sure to also change datasaver accordingly
    public enum SlotType {
        input, output, storage, house, worker
    }

    public SlotType mySlotType;

    public InventoryItemSlot (Item item, int _count, int _maxcount, SlotType slotType) { myItem = item; count = _count; maxCount = _maxcount; mySlotType = slotType; }
}
