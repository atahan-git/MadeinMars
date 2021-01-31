using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class RecipeSet : ScriptableObject {
    public ItemSet[] myItemSets = new ItemSet[1];
    public List<Node> myNodes = new List<Node>();


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

}

public class CountedPort : Port {
    public int count;
}

public class RegularPort : Port {
}


public class CraftingNode : Node {
    
    public int tier;
    public int craftingTime = 5;

    public enum CraftingTypes {
        Miner, Furnace, ProcessorSingle, ProcessorDouble, Press, Coiler, Cutter, Lab, Building
    }

    public CraftingTypes myCraftingType;
    
    public CraftingNode(int id) : base(id) { }
    
    public override List<Port> GetRightPorts() {
        throw new NotImplementedException();
    }

    public override List<Port> GetLeftPorts() {
        throw new NotImplementedException();
    }
}

public class ItemNode : Node {
    public ItemNode(int id) : base(id) { }
    
    public override List<Port> GetRightPorts() {
        throw new NotImplementedException();
    }

    public override List<Port> GetLeftPorts() {
        throw new NotImplementedException();
    }
}