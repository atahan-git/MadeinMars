using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Nova.NodeItemTree {
    [CreateAssetMenu]
    public class RecipeSet : ScriptableObject {
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
                    if (myItem != null)
                        return myItem;
                }
            }
            return null;
        }

        public List<CraftingProcessNode> myCraftingProcessNodes = new List<CraftingProcessNode>();
        public List<ItemNode> myItemNodes = new List<ItemNode>();
    }

    [Serializable]
    public class CraftingProcessNode {

        [HideInInspector]
        public int x, y;

        public int tier;
        public float craftingTime;

        public enum CraftingTypes { Processing, Melting }
        public CraftingTypes myCraftingType;

        public List<(ItemNode, int)> inputs = new List<(ItemNode, int)>();
        public List<(ItemNode, int)> outputs = new List<(ItemNode, int)>();

    }


    [Serializable]
    public class ItemNode {

        [HideInInspector]
        public int x, y;

        public Item myItem;
    }
}
