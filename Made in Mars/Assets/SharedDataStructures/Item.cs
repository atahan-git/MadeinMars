using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
[CreateAssetMenu(menuName = "RecipeSystem/Item")]
public class Item : ScriptableObject, IEquatable<Item> {
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