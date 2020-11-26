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


public class Node {
    public Vector3 pos;

    public Node () { }
    
    
}

public class Port {
    public Node source;
    public Port connection; //Leave as null for unset

    public virtual int GiveType() {
        return 0;
    }

    public virtual int GiveConnectableType() {
        return 1;
    }
    
    protected Port() { }

    public Port (Node source, Port connection) {
        this.source = source;
        this.connection = connection;
    }
    public Port (Node source) {
        this.source = source;
        this.connection = null;
    }

    public void RemoveConnection() {
        if (connection != null)
            connection.connection = null;
        connection = null;
    }
    
    public static void ConnectPorts(Port port1, Port port2) {
        port1.RemoveConnection();
        port2.RemoveConnection();
        port1.connection = port2;
        port2.connection = port1;
    }
}



public class CrafterPort : Port{
    public int count;

    public override int GiveType() {
        return 1;
    }

    public override int GiveConnectableType() {
        return 0;
    }
    
    public CrafterPort (int sourceId, int connectedId, int count) {
        this.sourceId = sourceId;
        this.connectedId = connectedId;
        this.count = count;
    }
    public CrafterPort (int sourceId) {
        this.sourceId = sourceId;
        this.connectedId = -1;
        this.count = 0;
    }
}


public class CraftingNode : Node {

    public int tier;
    public float craftingTime;

    public enum CraftingTypes {
        Furnace,
        Processing,
        Melting
    }

    public CraftingTypes myCraftingType;

    private List<CrafterPort> inputs = new List<CrafterPort>();
    private List<CrafterPort> outputs = new List<CrafterPort>();

    public CraftingNode() { }
    
    public CraftingNode(int tier, float craftingTime, CraftingTypes myCraftingType){
        this.tier = tier;
        this.craftingTime = craftingTime;
        this.myCraftingType = myCraftingType;
    }
    
    public void AddPort(bool isInput) {
        
    }

    public void RemovePort(bool isInput, bool index) {
        
    }

    public List<CrafterPort> GetInputs() {
        return inputs;
    }

    public List<CrafterPort> GetOutputs() {
        return outputs;
    }
}



public class ItemNode : Node {

    public Item myItem;

    public List<Port> inputs = new List<Port>();
    public List<Port> outputs = new List<Port>();
    
    public ItemNode(Item item) {
        myItem = item;
    }
    
    public void AddPort(bool isInput) {
        
    }

    public void RemovePort(bool isInput, bool index) {
        
    }
}
//}
