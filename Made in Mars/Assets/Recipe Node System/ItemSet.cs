using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


/// <summary>
/// These are sets of items that are used in recipe sets.
/// The idea is that mods will be able to introduce their own itemsets, and use them in recipes.
/// </summary>
[CreateAssetMenu]
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



[System.Serializable]
public class Item : IEquatable<Item> {
    public string uniqueName = "";
    public string name = "New Item";
    [TextArea] public string description = "This is a new Item";

    [HideInInspector]
    //Item sets are assigned by DataHolder
    public ItemSet myItemSet;

    [HideInInspector]
    //Item ids are assigned by DataHolder
    //They are the equivalent of the items array position, counted additively if there are multiple item sets
    public int myId = -1;
    
    public Sprite mySprite;
    
    public int inventoryStackCount = 99;
    
    public Sprite GetSprite() {
        return mySprite;
    }

    public static bool operator == (Item a, Item b) {
        if (ReferenceEquals(a, null))
            return ReferenceEquals(b, null);
        
        if (ReferenceEquals(b, null))
            return false;
        
        return a.uniqueName == b.uniqueName;
    }
	
    public static bool operator != (Item a, Item b) {
        if (ReferenceEquals(a, null))
            return !ReferenceEquals(b, null);
        
        if (ReferenceEquals(b, null))
            return true;
        
        return a.uniqueName != b.uniqueName;
    }
    
    
    public static Item GetEmpty() {
        return null;
    }

    public Item MakeDummyItem(int itemType) {
        uniqueName = itemType.ToString();
        return this;
    }
    
    public Item MakeDummyItem(string itemType) {
        uniqueName = itemType;
        return this;
    }

    public Item MakeBuildingDataDummyItem(string buildingUniqueName, Sprite sprite) {
        uniqueName = buildingUniqueName;
        mySprite =sprite;
        return this;
    }

    public bool Equals(Item other) {
        return this == other;
    }
}