using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//namespace Nova.NodeItemTree {
[CreateAssetMenu]
public class RecipeSet : ScriptableObject {
    public ItemSet[] myItemSets = new ItemSet[1];


    /// <summary>
    /// Returns Item in all item sets based on uniqueName
    /// Returns null if not found
    /// </summary>
    /// <returns>Item or null</returns>
    public Item GetItem(string uniqueName) {
        for (int i = 0; i < myItemSets.Length; i++) {
            if (myItemSets[i] != null) {
                Item myItem = myItemSets[i].GetItem(uniqueName);
                if (myItem != null)
                    return myItem;
            }
        }

        return null;
    }

    public List<CraftingNode> myCraftingNodes = new List<CraftingNode>();
    public List<ItemNode> myItemNodes = new List<ItemNode>();
}

[Serializable]
public class Node {
    public int id;
    public Vector3 pos;

    public Node (int id) {
        this.id = id;
    }
}

[Serializable]
public class CraftingNode : Node {

    public int tier;
    public int craftingTime = 5;

    public enum CraftingTypes {
        Miner, Furnace, ProcessorSingle, ProcessorDouble, Press, Coiler, Cutter, Lab, Building
    }

    public CraftingTypes myCraftingType;

    public List<CountedItemNode> inputs = new List<CountedItemNode>();
    public List<CountedItemNode> outputs = new List<CountedItemNode>();

    public CraftingNode(int id) : base(id) {
        this.id = id;
    }
    
    public CraftingNode(int id, int tier, int craftingTime, CraftingTypes myCraftingType) : base(id) {
        this.id = id;
        this.tier = tier;
        this.craftingTime = craftingTime;
        this.myCraftingType = myCraftingType;
    }
}

[Serializable]
public class CountedItemNode {
    public ItemNode itemNode;
    public int count;

    public CountedItemNode(ItemNode node, int _count) {
        itemNode = node;
        count = _count;
    }
}


[Serializable]
public class ItemNode : Node {
    public Item myItem;
    
    public List<Node> inputs = new List<Node>();
    public List<Node> outputs = new List<Node>();

    public ItemNode(int id, Item item) : base(id) {
        this.id = id;
        myItem = item;
    }
}
//}
