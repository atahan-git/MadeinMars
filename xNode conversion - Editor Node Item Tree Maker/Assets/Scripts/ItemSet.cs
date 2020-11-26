using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//namespace Nova.NodeItemTree {
    [CreateAssetMenu]
    public class ItemSet : ScriptableObject {
        public Item[] items = new Item[10];

        /// <summary>
        /// Returns Item based on uniqueName
        /// Returns null if not found
        /// </summary>
        /// <returns>Item or null</returns>
        public Item GetItem (string uniqueName) {
            for (int i = 0; i < items.Length; i++) {
                if (items[i] != null)
                    if (items[i].uniqueName == uniqueName)
                        return items[i];
            }
            return null;
        }
    }


    [System.Serializable]
    public class Item {
        public string uniqueName = "Not Set";
        public string name = "New Item";
        [TextArea]
        public string description = "This is a new Item";

        public Material myMat;
        public Sprite mySprite;
    }
//}