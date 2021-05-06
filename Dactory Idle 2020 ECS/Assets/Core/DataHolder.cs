using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine;


/// <summary>
/// Holds all the needed building data, item data, recipe data etc.
/// Assigns UniqueId's to items at the start of the game, and deals with connecting item to id.
/// Mods and stuff will somehow add data here.
/// eg if you add another RecipeSet, congrats! you've modded in extra crafting recipes to the game.
/// </summary>
public class DataHolder : MonoBehaviour {
    public static DataHolder s;
    [SerializeField]
    private BuildingData[] myBuildings;
    [SerializeField]
    private ItemSet[] myItemSets;
    [SerializeField]
    private RecipeSet[] myRecipeSets; 
    CraftingNode[] myCraftingProcesses;
    CraftingNode[][] myCraftingProcessesDivided;

    // Layers - the z coordinates for various objects
    public static int worldLayer = 1;
    public static int beltLayer = 0;
    public static int connectorBaseLayer = 0;
    public static int itemLayer = -1;
    public static int connectorOverlayLayer = -2;
    public static int connectorPullerLayer = -3;
    public static int buildingLayer = -4;
    public static int droneLayer = -6;

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
        return myRecipeSets[0].myOres;
    }

    
    //---------------------------------------------
    // All of the following could probably be converted to hash tables or sth to make them more efficient,
    // but it isn't particularly necessary yet, so the added complexity isn't worth it
    
    public bool UniqueNameToOreId(string name, out int oreId) {
        for (int i = 0; i < GetAllOres().Length; i++) {
            if (GetAllOres()[i].oreUniqueName == name) {
                oreId = i + 1;
                return true;
            }
        }

        oreId = 0;
        return false;
    }

    public bool OreIdtoUniqueName (int id, out string oreType) {
        if (id > 0 && id <= GetAllOres().Length) {
            oreType = GetAllOres()[id - 1].oreUniqueName;
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

    /// <summary>
    /// Should be used for all long term saving purposes
    /// The "UniqueName"s will never change! but the specific objects may change
    /// </summary>
    public Item GetItem (string uniqueName) {
        //int idCounter = 0;
        for (int m = 0; m < myItemSets.Length; m++) {
            for (int i = 0; i < myItemSets[m].items.Length; i++) {
                if (myItemSets[m].items[i] != null) {
                    if (myItemSets[m].items[i].uniqueName == uniqueName) {
                        return myItemSets[m].items[i];
                    }
                }
                //idCounter++;
            }
        }
        throw new NullReferenceException("The item you are requesting " + uniqueName + " does not exist!");
    }

    private Item[] allItems = new Item[0];
    /// <summary>
    /// Automatically generate an array containing all the items currently available
    /// </summary>
    /// <returns>An array of items</returns>
    public Item[] GetAllItems() {
        if (allItems.Length == 0) {

            int length = 0;

            for (int m = 0; m < myItemSets.Length; m++) {
                length += myItemSets[m].items.Length;
            }

            allItems = new Item[length];

            int lastIndex = 0;
            for (int m = 0; m < myItemSets.Length; m++) {
                myItemSets[m].items.CopyTo(allItems, lastIndex);
                lastIndex += myItemSets[m].items.Length;
            }
        }

        return allItems;
    }

    /// <summary>
    /// Only use this while the game is running. Id's are assigned dynamically at start, and may not be the same between sessions
    /// Used by the belt system and the crafting system for efficiency over the unique name system.
    /// </summary>
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
        List<CraftingNode> cp = new List<CraftingNode>();

        for (int i = 0; i < myRecipeSets.Length; i++) {
            for (int m = 0; m < myRecipeSets[i].myCraftingNodes.Count; m++) {
                cp.Add((CraftingNode)myRecipeSets[i].myCraftingNodes[m]);
            }
        }

        myCraftingProcesses = cp.ToArray();
    }

    /// <summary>
    /// We kinda need to convert the rather ugly set of crafting nodes to easily useable categories for the BuildingCraftingController to use
    /// That is done here.
    /// </summary>
    void DivideCraftingProcessesArray () {
        Dictionary<CraftingNode.cTypes, int> cTypetoIndexMatch = new Dictionary<CraftingNode.cTypes, int>();
        cTypetoIndexMatch[CraftingNode.cTypes.Miner]             = 0;
        cTypetoIndexMatch[CraftingNode.cTypes.Furnace]           = 1;
        cTypetoIndexMatch[CraftingNode.cTypes.ProcessorSingle]   = 2;
        cTypetoIndexMatch[CraftingNode.cTypes.ProcessorDouble]   = 3;
        cTypetoIndexMatch[CraftingNode.cTypes.Press]             = 4;
        cTypetoIndexMatch[CraftingNode.cTypes.Coiler]            = 5;
        cTypetoIndexMatch[CraftingNode.cTypes.Cutter]            = 6;
        cTypetoIndexMatch[CraftingNode.cTypes.Lab]               = 7;
        cTypetoIndexMatch[CraftingNode.cTypes.Building]          = 8;
        // Make sure this matches with the switch statement in GetCraftingProcessesOfType function!


        List<List<CraftingNode>> cp = new List<List<CraftingNode>>();

        for (int i = 0; i < cTypetoIndexMatch.Count; i++) {
            cp.Add(new List<CraftingNode>());
        }

        for (int i = 0; i < myCraftingProcesses.Length; i++) {
            cp[cTypetoIndexMatch[myCraftingProcesses[i].CraftingType]].Add(myCraftingProcesses[i]);
        }

        myCraftingProcessesDivided = cp.Select(a => a.ToArray()).ToArray();
    }

    public CraftingNode[] GetCraftingProcessesOfType (BuildingData.ItemType type) {
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
            Debug.Log("Building does not support crafting! >> " + type.ToString());
            return null;
        }

        return myCraftingProcessesDivided[index];
    }

    public CraftingNode[][] GetAllCraftingProcessNodesDivided() {
        return myCraftingProcessesDivided;
    }
}

public delegate void GenericCallback ();

public interface IInventoryController {
    event GenericCallback drawInventoryEvent; // This is for the initial drawing of the inventory. Only needs to be called when slot counts change
    
    event GenericCallback inventoryContentsChangedEvent; // Sign up to this if you want to be updated whenever the inventory contents are changed
}