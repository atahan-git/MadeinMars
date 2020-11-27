using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemSet : ScriptableObject
{
    public Material myMaterial;
    public Texture myTexture;

    public Item[] items = new Item[10];

    /// <summary>
    /// Returns Item based on uniqueName
    /// Returns null if not found
    /// </summary>
    /// <returns>Item or null</returns>
    public Item GetItem (string uniqueName) {
        for (int i = 0; i < items.Length; i++) {
            if (items[i] != null)
                if (items[i].uniqueName == uniqueName) {
                    return items[i];
                }
        }
        return null;
    }


    public Vector2 GetTextureCoordinates (Item item) {
        float multiplier = 64f / 1024f;
        return new Vector2(item.myTextureOffset.x * multiplier, 1f - (item.myTextureOffset.y+1) * multiplier);
    }

}

[System.Serializable]
public class InventoryItemSlot{
    public Item myItem;
    public int count = 0; // If 0, means the slot is empty.

    public InventoryItemSlot () { }
    public InventoryItemSlot (Item item, int _count) { myItem = item; count = _count; }
}



[System.Serializable]
public class Item {
    public string uniqueName = "";
    public string name = "New Item";
    [TextArea] public string desctiption = "This is a new Item";

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
}