using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine;
using XNode;


/// <summary>
/// Holds all the building data, item data, recipe data etc. needed
/// Mods and stuff will somehow add data here.
/// </summary>
public class DataHolder : MonoBehaviour {
    public static DataHolder s;
    [SerializeField]
    private BuildingData[] myBuildings;
    [SerializeField]
    private ItemSet[] myItemSets;
    [SerializeField]
    private RecipeSet[] myRecipeSets; 
    CraftingProcessNode[] myCraftingProcesses;
    CraftingProcessNode[][] myCraftingProcessesDivided;

    [Header ("please note that the higher ups in the lists will override the others during generation")]
    [SerializeField]
	private OreSpawnSettings[] myOres;

    //Layers
    public static int worldLayer = 1;
    public static int beltLayer = 0;
    public static int itemLayer = -1;
    public static int buildingLayer = -2;

    private void Awake () {
        if (s != null) {
            Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
        }
        s = this;
        GenerateCraftingProcessesArray();
        DivideCraftingProcessesArray();
        AssignItemIds();
    }

    public OreSpawnSettings[] GetAllOres () {
        return myOres;
    }

    public bool OreIdtoUniqueName (int id, out string oreType) {
        if (id > 0 && id <= myOres.Length) {
            oreType = myOres[id - 1].oreUniqueName;
            return true;
        } else {
            oreType = null;
            return false;
        }
    }

    public BuildingData GetBuilding (string uniqueName) {
        // This should be converted to use a dictionary at some point
        for (int i = 0; i < myBuildings.Length; i++) {
            if (myBuildings[i] != null) {
                if (myBuildings[i].uniqueName == uniqueName)
                    return myBuildings[i];
            }
        }
        Debug.LogError("Illegal building request: " + uniqueName);
        return null;
    }

    public BuildingData[] AllBuildings () {
        return myBuildings;
    }

    public int GetItemIDFromName (string uniqueName) {
        return GetItem(uniqueName).myId;
    }

    public Item GetItem (string uniqueName) {
        int idCounter = 0;
        for (int m = 0; m < myItemSets.Length; m++) {
            for (int i = 0; i < myItemSets[m].items.Length; i++) {
                if (myItemSets[m].items[i] != null) {
                    if (myItemSets[m].items[i].uniqueName == uniqueName) {
                        return myItemSets[m].items[i];
                    }
                }
                idCounter++;
            }
        }
        throw new NullReferenceException("The item you are requesting " + uniqueName + " does not exist!");
    }


    public Item GetItem (int itemId) {
        int itemSet = 0;
        while (itemId > myItemSets[itemSet].items.Length) {
            itemId -= myItemSets[itemSet].items.Length;
            itemSet++;
            if (itemSet >= myItemSets.Length) {
                throw new NullReferenceException("The item you are requesting with id " + itemId + " does not exist!");
            }
        }
        return myItemSets[itemSet].items[itemId];
    }

    public int TotalItemCount () {
        int total = 0;
        for (int m = 0; m < myItemSets.Length; m++) {
            total += myItemSets[m].items.Length;
        }
        return total;
    }

    void AssignItemIds () {
        int idCounter = 0;
        for (int m = 0; m < myItemSets.Length; m++) {
            for (int i = 0; i < myItemSets[m].items.Length; i++) {
                if (myItemSets[m].items[i] != null) {
                    myItemSets[m].items[i].myItemSet = myItemSets[m];
                    myItemSets[m].items[i].myId = idCounter;
                }
                idCounter++;
            }
        }
    }

    void GenerateCraftingProcessesArray () {
        List<CraftingProcessNode> cp = new List<CraftingProcessNode>();

        for (int i = 0; i < myRecipeSets.Length; i++) {
            for (int m = 0; m < myRecipeSets[i].nodes.Count; m++) {
                if (myRecipeSets[i].nodes[m] is CraftingProcessNode) {
                    cp.Add((CraftingProcessNode)myRecipeSets[i].nodes[m]);
                }
            }
        }

        myCraftingProcesses = cp.ToArray();
    }

    void DivideCraftingProcessesArray () {
        Dictionary<CraftingProcessNode.cTypes, int> cTypetoIndexMatch = new Dictionary<CraftingProcessNode.cTypes, int>();
        cTypetoIndexMatch[CraftingProcessNode.cTypes.Miner]             = 0;
        cTypetoIndexMatch[CraftingProcessNode.cTypes.Furnace]           = 1;
        cTypetoIndexMatch[CraftingProcessNode.cTypes.ProcessorSingle]   = 2;
        cTypetoIndexMatch[CraftingProcessNode.cTypes.ProcessorDouble]   = 3;
        cTypetoIndexMatch[CraftingProcessNode.cTypes.Press]             = 4;
        cTypetoIndexMatch[CraftingProcessNode.cTypes.Coiler]            = 5;
        cTypetoIndexMatch[CraftingProcessNode.cTypes.Cutter]            = 6;
        cTypetoIndexMatch[CraftingProcessNode.cTypes.Lab]               = 7;
        cTypetoIndexMatch[CraftingProcessNode.cTypes.Building]          = 8;
        // Make sure this matches with the switch statement in GetCraftingProcessesOfType function!


        List<List<CraftingProcessNode>> cp = new List<List<CraftingProcessNode>>();

        for (int i = 0; i < cTypetoIndexMatch.Count; i++) {
            cp.Add(new List<CraftingProcessNode>());
        }

        for (int i = 0; i < myCraftingProcesses.Length; i++) {
            cp[cTypetoIndexMatch[myCraftingProcesses[i].CraftingType]].Add(myCraftingProcesses[i]);
        }

        myCraftingProcessesDivided = cp.Select(a => a.ToArray()).ToArray();
    }

    public CraftingProcessNode[] GetCraftingProcessesOfType (BuildingData.ItemType type) {
        int index = -1;
        switch (type) {
        case BuildingData.ItemType.Miner:           index = 0; break;
        case BuildingData.ItemType.Furnace:         index = 1; break;
        case BuildingData.ItemType.ProcessorSingle: index = 2; break;
        case BuildingData.ItemType.ProcessorDouble: index = 3; break;
        case BuildingData.ItemType.Press:           index = 4; break;
        case BuildingData.ItemType.Coiler:          index = 5; break;
        case BuildingData.ItemType.Cutter:          index = 6; break;
        case BuildingData.ItemType.Lab:             index = 7; break;
        case BuildingData.ItemType.Building:        index = 8; break;
        }
        if (index == -1) {
            Debug.LogError("Building does not support crafting! >> " + type.ToString());
            return null;
        }

        return myCraftingProcessesDivided[index];
    }
}
