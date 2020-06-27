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
                    items[i].myItemSet = this;
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
public class Item {
    public string uniqueName = "Not Set";
    public string name = "New Item";
    [TextArea]
    public string desctiption = "This is a new Item";

    [HideInInspector]
    public ItemSet myItemSet;
    public Vector2 myTextureOffset = new Vector2(-1,-1);

	private Material myMat = null;

    public Material GetMaterial () {
        if (myMat == null) {
            myMat = new Material(myItemSet.myMaterial);
            myMat.mainTextureOffset = GetTextureCoordinates();
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