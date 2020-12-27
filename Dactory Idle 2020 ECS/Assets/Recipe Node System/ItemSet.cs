using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// These are sets of items that are used in recipe sets.
/// The idea is that mods will be able to introduce their own itemsets, and use them in recipes.
/// </summary>
[CreateAssetMenu]
public class ItemSet : ScriptableObject {
    public Material myMaterial;
    public Texture myTexture;

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


    public Vector2 GetTextureCoordinates(Item item) {
        float multiplier = 64f / 1024f;
        return new Vector2(item.myTextureOffset.x * multiplier, 1f - (item.myTextureOffset.y + 1) * multiplier);
    }
}


[System.Serializable]
public class InventoryItemSlot{
    public Item myItem;
    public int count = 0; // If 0, means the slot is empty.
    public int maxCount = -1; // -1 means no limit

    public bool isOutputSlot;

    public InventoryItemSlot () { }
    public InventoryItemSlot (Item item, int _count) { myItem = item; count = _count; }
    public InventoryItemSlot (Item item, int _count, int _maxcount) { myItem = item; count = _count; maxCount = _maxcount; }
    public InventoryItemSlot (Item item, int _count, int _maxcount, bool _isOutputSlot) { myItem = item; count = _count; maxCount = _maxcount; isOutputSlot = _isOutputSlot; }
}



[System.Serializable]
public class Item {
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

    public Vector2 myTextureOffset = new Vector2(-1, -1);
    public Sprite mySprite;

    private Material myMat = null;
    
    
    public float weight = 1;

    [Header("Buy/Sell")]

    public int butsellamount = 1;
    [Tooltip("Note that 1 cost = $1 M.")]
    public float buyCost = 10;
    [Tooltip("Note that 1 cost = $1 M.")]
    public float sellCost = 1;

    public Sprite GetSprite() {
        return mySprite;
    }

    public Material GetMaterial () {
        if (myMat == null) {
            myMat = new Material(myItemSet.myMaterial);
            myMat.mainTextureOffset = GetTextureCoordinates();
            myMat.name = uniqueName + " Material";
        }
        return myMat;
    }

    public Vector2 GetTextureCoordinates () {
        return myItemSet.GetTextureCoordinates(this);
    }

    public Vector2 GetScale () {
        float scale = 64f / 1024f;
        return new Vector2(scale, scale);
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
}