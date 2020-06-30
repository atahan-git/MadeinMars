using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu]
public class RecipeSet : NodeGraph {
    // Start is called before the first frame update
    public ItemSet[] myItemSets = new ItemSet[1];


    /// <summary>
    /// Returns Item in all item sets based on uniqueName
    /// Returns null if not found
    /// </summary>
    /// <returns>Item or null</returns>
    public Item GetItem (string uniqueName) {
        for (int i = 0; i < myItemSets.Length; i++) {
            if (myItemSets[i] != null) {
                Item myItem = myItemSets[i].GetItem(uniqueName);
                if (myItem != null) {
                    myItem.myItemSet = myItemSets[i];
                    return myItem;
                }
            }
        }
        return null;
    }
}


[System.Serializable]
public class ItemInput { }


[System.Serializable]
public class ItemOutput { }

