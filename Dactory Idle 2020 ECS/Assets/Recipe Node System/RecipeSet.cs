using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This is the main holder for a crafting tree set.
/// The idea is that a mod will be able to introduce its own recipe set to modify the game
/// This class also holds some helper methods
/// </summary>
[CreateAssetMenu]
public class RecipeSet : ScriptableObject {
    public ItemSet[] myItemSets = new ItemSet[0];
    public BuildingData[] myBuildings = new BuildingData[0];
    [Header ("please note that the higher ups in the lists will override the others during generation")]
    public OreSpawnSettings[] myOres = new OreSpawnSettings[0];

    
    public List<CraftingNode> myCraftingNodes = new List<CraftingNode>();
    public List<ItemNode> myItemNodes = new List<ItemNode>();

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
    
    public BuildingData GetBuilding (string uniqueName) {
        for (int i = 0; i < myBuildings.Length; i++) {
            if (myBuildings[i] != null) {
                if (myBuildings[i].uniqueName == uniqueName) {
                    return myBuildings[i];
                }
            }
        }
        return null;
    }
}

// -----------------------------------
//
//  Everything below here are the helper classes to make the node system work
//
// -----------------------------------


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
    public int timeCost = 5;

    public enum cTypes {
        Miner, Furnace, ProcessorSingle, ProcessorDouble, Press, Coiler, Cutter, Lab, Building
    }

    public cTypes CraftingType;

    public List<CountedItemNode> inputs = new List<CountedItemNode>();
    public List<CountedItemNode> outputs = new List<CountedItemNode>();

    public CraftingNode(int id) : base(id) {
        this.id = id;
    }
    
    public CraftingNode(int id, int tier, int craftingTime, cTypes myCraftingType) : base(id) {
        this.id = id;
        this.tier = tier;
        this.timeCost = craftingTime;
        this.CraftingType = myCraftingType;
    }
}

[Serializable]
public class CountedItemNode {
    //public ItemNode itemNode;
    public int nodeId;
    public string itemUniqueName;
    public int count;

    public CountedItemNode(ItemNode node, int _count) {
        nodeId = node.id;
        itemUniqueName = node.itemUniqueName;
        count = _count;
    }
    
    public CountedItemNode(string _itemUniqueName, int _count) {
        nodeId = -1;
        itemUniqueName = _itemUniqueName;
        count = _count;
    }
}


[Serializable]
public class ItemNode : Node {
    public string itemUniqueName;
    
    public List<int> inputIds = new List<int>();
    public List<int> outputIds = new List<int>();
    
    public ItemNode(int id, Item item) : base(id) {
        this.id = id;
        this.itemUniqueName = item.uniqueName;
    }

    private Item _item;
    public Item GetItem(RecipeTreeMaster master) {
        if (_item == null || _item.uniqueName == "") {
            _item = master.myRecipeSet.GetItem(itemUniqueName);
        }

        if (_item == null) {
            var building = master.myRecipeSet.GetBuilding(itemUniqueName);
            _item = new Item();
            _item.uniqueName = building.uniqueName;
            _item.mySprite = building.gfxSprite;
        }

        return _item;
    }
}
//}
