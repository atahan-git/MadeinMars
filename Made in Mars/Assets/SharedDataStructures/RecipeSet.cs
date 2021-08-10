using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


/// <summary>
/// This is the main holder for a crafting tree set.
/// The idea is that a mod will be able to introduce its own recipe set to modify the game
/// This class also holds some helper methods
/// </summary>
[CreateAssetMenu(menuName = "RecipeSystem/RecipeSet")]
public class RecipeSet : ScriptableObject {
    public string recipeSetUniqueName = "";
    
    public ItemSet[] myItemSets = new ItemSet[0];
    public BuildingData[] myBuildings = new BuildingData[0];
    public ShipCard[] myCards = new ShipCard[0];
    [Header ("please note that the higher ups in the lists will override the others during generation")]
    public OreSpawnSettings[] myOres = new OreSpawnSettings[0];

    
    [SerializeField] List<CraftingNode> myCraftingNodes = new List<CraftingNode>();
    [SerializeField] List<ItemNode> myItemNodes = new List<ItemNode>();
    [SerializeField] List<ResearchNode> myResearchNodes = new List<ResearchNode>();

    public List<CraftingNode> GetCraftingNodes() {
        return myCraftingNodes;
    }
    public List<ItemNode> GetItemNodes() {
        return myItemNodes;
    }
    
    public List<ResearchNode> getResearchNodes() {
        return myResearchNodes;
    }

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
    
    public ShipCard GetShipCard (string uniqueName) {
        for (int i = 0; i < myCards.Length; i++) {
            if (myCards[i] != null) {
                if (myCards[i].uniqueName == uniqueName) {
                    return myCards[i];
                }
            }
        }
        return null;
    }
    
    public CraftingNode AddCraftingNode(Vector3 pos) {
        var craftNode = new CraftingNode(GetNextId(), recipeSetUniqueName);
        craftNode.pos = pos;
        myCraftingNodes.Add(craftNode);
        return craftNode;
    }

    public ResearchNode AddResearchNode(Vector3 pos) {
        var researchNode = new ResearchNode(GetNextId(), recipeSetUniqueName);
        researchNode.pos = pos;
        myResearchNodes.Add(researchNode);
        return researchNode;
    }

    public ItemNode AddItemNode(Vector3 pos, Item item) {
        var itemNode = new ItemNode(GetNextId(), recipeSetUniqueName, item);
        itemNode.pos = pos;
        myItemNodes.Add(itemNode);
        return itemNode;
    }

    public void RemoveNode(Node node) {
        if (node is CraftingNode) {
            myCraftingNodes.Remove((CraftingNode)node);
        } else if(node is ItemNode) {
            myItemNodes.Remove((ItemNode)node);
        } else {
            myResearchNodes.Remove((ResearchNode)node);
        }
    }

    /// <summary>
    /// Returns whether two adapter groups can be connected
    /// </summary>
    /// <returns></returns>
    public static bool CanConnectAdapters(AdapterGroup group1, AdapterGroup group2) {
        return group1.isLeftAdapter != group2.isLeftAdapter && group1.type == group2.type;
    }

    public static bool ConnectAdapters(AdapterGroup group1, AdapterGroup group2) {
        // Only connect left to right and only connect same types
        if (CanConnectAdapters(group1,group2)) {
            group1.connections.Add(
                new AdapterGroup.AdapterConnection() {count = 1, nodeId = group2.parentNodeId, recipeSetName = group2.recipeSetName}
            );
            group2.connections.Add(
                new AdapterGroup.AdapterConnection() {count = 1, nodeId = group1.parentNodeId, recipeSetName = group1.recipeSetName}
            );
            return true;
        } else {
            return false;
        }
    }

    public void RemoveConnection(AdapterGroup sourceAdapterGroup, AdapterGroup.AdapterConnection sourceConnection) {
        // TODO: Add functionality to disconnect nodes from different recipe sets
        var sourceNode = GetNodeWithId(sourceAdapterGroup.parentNodeId);
        var otherNode = GetNodeWithId(sourceConnection.nodeId);

        AdapterGroup otherAdapterGroup = null; 
        foreach (var adapterGroup in otherNode.myAdapters) {
            if (CanConnectAdapters(sourceAdapterGroup, adapterGroup)) {
                otherAdapterGroup = adapterGroup;
                break;
            }
        }

        AdapterGroup.AdapterConnection otherConnection = null;
        foreach (var connection in otherAdapterGroup.connections) {
            if (connection.nodeId == sourceNode.id) {
                otherConnection = connection;
                break;
            }
        }
        
        sourceAdapterGroup.connections.Remove(sourceConnection);
        otherAdapterGroup.connections.Remove(otherConnection);
    }

    public Node GetNodeWithId(int id) {
        foreach (var node in myCraftingNodes) {
            if (node.id == id)
                return node;
        } 
        foreach (var node in myItemNodes) {
            if (node.id == id)
                return node;
        }

        return null;
    }

    int idCounter = -1;
    int GetNextId() {
        if (idCounter == -1) {
            foreach (var node in myCraftingNodes) {
                idCounter = Mathf.Max(idCounter, node.id);
            } 
            foreach (var node in myItemNodes) {
                idCounter = Mathf.Max(idCounter, node.id);
            }
            foreach (var node in myResearchNodes) {
                idCounter = Mathf.Max(idCounter, node.id);
            }
        }

        idCounter++;
        return idCounter;
    }

    [ContextMenu("FixRecipes")]
    public void FixRecipes() {
        var count = 0;
        foreach (var node in myCraftingNodes) {
            if (node.myAdapters.Count != 2) {
                node.SetupAdapters();
                count += 1;
            } 
        }
        
        foreach (var node in myItemNodes) {
            if (node.myAdapters.Count != 2) {
                node.SetupAdapters();
                count += 1;
            }
        }
        
        foreach (var node in myResearchNodes) {
            if (node.myAdapters.Count != 4) {
                node.SetupAdapters();
                count += 1;
            }
        }
        
        
        List<Node> nodeMegaList = new List<Node>();
        nodeMegaList.AddRange(myCraftingNodes);
        nodeMegaList.AddRange(myItemNodes);
        nodeMegaList.AddRange(myResearchNodes);
        var removedConnectionsCount = 0;


        HashSet<int> allExistingIds = new HashSet<int>();
        foreach (var node in nodeMegaList) {
            allExistingIds.Add(node.id);
        }

        foreach (var node in nodeMegaList) {
            node.recipeSetName = recipeSetUniqueName;
            foreach (var adapterGroup in node.myAdapters) {
                adapterGroup.recipeSetName = recipeSetUniqueName;
                for(int i = adapterGroup.connections.Count-1; i >= 0; i--) {
                    var connection = adapterGroup.connections[i];
                    connection.recipeSetName = recipeSetUniqueName;
                    if (!allExistingIds.Contains(connection.nodeId)) {
                        adapterGroup.connections.RemoveAt(i);
                        removedConnectionsCount++;
                    }
                }
            }
        }
        
        
        Debug.Log($"Fixed {count} nodes and removed {removedConnectionsCount} connections");
    }
}

// -----------------------------------
//
//  Everything below here are the helper classes to make the node system work
//
// -----------------------------------


[Serializable]
public abstract class Node {
    public string recipeSetName;
    public int id;
    public Vector3 pos;

    public List<AdapterGroup> myAdapters = new List<AdapterGroup>();
    
    public Node(int id, string recipeSetName) {
        this.id = id;
        this.recipeSetName = recipeSetName;
    }

    public abstract void SetupAdapters();
    
    public static void AttachAdapters(AdapterGroup incomingAdapter, AdapterGroup targetAdapter) {
        
        // We want to only connect adapters on the opposite sides
        if (incomingAdapter.isLeftAdapter == !targetAdapter.isLeftAdapter) {
            if (incomingAdapter.type == targetAdapter.type) {
                incomingAdapter.connections.Add(new AdapterGroup.AdapterConnection(){count = 1, nodeId = targetAdapter.parentNodeId});
                targetAdapter.connections.Add(new AdapterGroup.AdapterConnection() {count = 1, nodeId = incomingAdapter.parentNodeId});
            }
        }
    }

    public static void DetachAdapters(AdapterGroup incomingAdapter, AdapterGroup targetAdapter) {
        for (int i = 0; i < incomingAdapter.connections.Count; i++) {
            if (incomingAdapter.connections[i].nodeId == targetAdapter.parentNodeId) {
                incomingAdapter.connections.RemoveAt(i);
                break;
            }
        }
        
        for (int i = 0; i < targetAdapter.connections.Count; i++) {
            if (targetAdapter.connections[i].nodeId == incomingAdapter.parentNodeId) {
                targetAdapter.connections.RemoveAt(i);
                break;
            }
        }
    }


    public int GetAdapterConnectionCount(bool isLeft) {
        int count = 0;
        foreach (var adapterGroup in myAdapters) {
            if (adapterGroup.isLeftAdapter == isLeft) {
                count = adapterGroup.connections.Count;
            }
        }

        return count;
    }
}

[Serializable]
public class AdapterGroup {
    public enum AdapterType {
        single, counted
    }

    public bool isLeftAdapter;

    public AdapterType myType;

    public int type = -1; // and adapter only connects to other adapters of the same type

    public List<AdapterConnection> connections = new List<AdapterConnection>();
    public int parentNodeId;
    public string recipeSetName;
    
    [Serializable]
    public class AdapterConnection {
        public int count;
        public string recipeSetName;
        public int nodeId;
    }

    public AdapterGroup(int _parentNodeId, bool _isLeftAdapter, AdapterType _myType, int _type, string _recipeSetName) {
        parentNodeId = _parentNodeId;
        isLeftAdapter = _isLeftAdapter;
        myType = _myType;
        type = _type;
        recipeSetName = _recipeSetName;
    }
}


[Serializable]
public class CraftingNode : Node {

    public int tier;
    public int timeCost = 5;

    // Make sure to also go edit DataHolder's index converted when you add things here!
    public enum cTypes {
        Miner, Furnace, AssemblerSingle, AssemblerDouble, Press, Coiler, Cutter, Lab, Building, Processor
    }

    public cTypes CraftingType;
    
    public CraftingNode(int id, string recipeSetName) : base(id, recipeSetName) {
        this.id = id;
        this.recipeSetName = recipeSetName;
        SetupAdapters();
    }
    
    public CraftingNode(int id, int tier, int craftingTime, cTypes myCraftingType, string recipeSetName) : base(id, recipeSetName) {
        this.id = id;
        this.recipeSetName = recipeSetName;
        this.tier = tier;
        this.timeCost = craftingTime;
        this.CraftingType = myCraftingType;
        SetupAdapters();
    }

    public override void SetupAdapters() {
        myAdapters.Clear();
        myAdapters.Add(new AdapterGroup(id, true, AdapterGroup.AdapterType.counted, 0, recipeSetName));
        myAdapters.Add(new AdapterGroup(id, false, AdapterGroup.AdapterType.counted, 1, recipeSetName));
    }
    
    
}


[Serializable]
public class ItemNode : Node {
    public string itemUniqueName;
    
    public ItemNode(int id, string recipeSetName, Item item) : base(id, recipeSetName) {
        this.id = id;
        this.recipeSetName = recipeSetName;
        this.itemUniqueName = item.uniqueName;
        SetupAdapters();
    }
    public override void SetupAdapters() {
        myAdapters.Clear();
        myAdapters.Add(new AdapterGroup(id, true, AdapterGroup.AdapterType.single, 1, recipeSetName));
        myAdapters.Add(new AdapterGroup(id, false, AdapterGroup.AdapterType.single, 0, recipeSetName));
    }
}

[Serializable]
public class ResearchNode : Node {
    public string researchUniqueName = "rUnnamed";
    public string researchDescription = "no description";

    [Header("Each basic lab unit produces 1 research per 20 seconds")]
    public int researchReq;
    public ResearchNode(int id, string recipeSetName) : base(id, recipeSetName) {
        this.id = id;
        this.recipeSetName = recipeSetName;
        SetupAdapters();
    }
    
    
    public override void SetupAdapters() {
        myAdapters.Clear();
        myAdapters.Add(new AdapterGroup(id, true, AdapterGroup.AdapterType.single, 2, recipeSetName));
        myAdapters.Add(new AdapterGroup(id, false, AdapterGroup.AdapterType.single, 2, recipeSetName));
        myAdapters.Add(new AdapterGroup(id, true, AdapterGroup.AdapterType.counted, 0, recipeSetName));
        myAdapters.Add(new AdapterGroup(id, false, AdapterGroup.AdapterType.counted, 1, recipeSetName));
    }
}
//}
