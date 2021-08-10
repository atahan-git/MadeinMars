using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


/// <summary>
/// Holds all the needed building data, item data, recipe data etc.
/// Assigns UniqueId's to items at the start of the game, and deals with connecting item to id.
/// Mods and stuff will somehow add data here.
/// eg if you add another RecipeSet, congrats! you've modded in extra crafting recipes to the game.
/// </summary>
[Serializable]
public class DataHolder  {
    public static DataHolder s;
    public TileSet[] allTileSets;
    public PlanetSchematic[] allPlanetSchematics;
    public OreDensityLevelsHolder[] allOreDensities;
    
    [Space]
    
    public BuildingData[] allBuildings;
    public ItemSet[] allItemSets;
    public RecipeSet[] allRecipeSets;
    public ShipCard[] allShipCards;
    CraftingNode[] myCraftingProcesses;
    CraftingNode[][] myCraftingProcessesDivided;

    [Space] 
    public BuildingData cardBuildingData;

    // Layers - the z coordinates for various objects
    public static readonly int worldLayer = 1;
    public static readonly int beltLayer = 0;
    public static readonly int connectorBaseLayer = 0;
    public static readonly int itemLayer = -1;
    public static readonly int connectorOverlayLayer = -2;
    public static readonly int connectorPullerLayer = -3;
    public static readonly int buildingLayer = -4;
    public static readonly int droneLayer = -6;
    public static readonly int ShipCardLayer = -7;
    public static readonly int itemPlacementLayer = -5;

    public void Setup() {
        GenerateCraftingProcessesArray();
        DivideCraftingProcessesArray();
        AssignItemIds();
    }

    public (int, string) OreSpawnSettingsToSaveInfo(OreSpawnSettings ore) {
        for (int i = 0; i < allOreDensities.Length; i++) {
            if (allOreDensities[i].GetUniqueName() == ore.oreUniqueName) {
                var oreLevels = allOreDensities[i].oreLevels;
                for (int j = 0; j < oreLevels.Length; j++) {
                    if (oreLevels[j] == ore) {
                        return (j, ore.oreUniqueName);
                    }
                }
            }
        }

        Debug.LogError("Ore not found in the list of available ores and density levels: " + ore.oreUniqueName);
        return (0, "unknown");
    }

    public OreSpawnSettings GetOreSpawnSettings(int density, string uniqueName) {
        Assert.IsTrue(density >= 0);
        for (int i = 0; i < allOreDensities.Length; i++) {
            if (allOreDensities[i].GetUniqueName() == uniqueName) {
                if (density > allOreDensities[i].oreLevels.Length) {
                    Debug.LogError($"Density level {density} is too high, defaulting to next biggest value {allOreDensities[i].oreLevels.Length-1}");
                    density = allOreDensities[i].oreLevels.Length-1;
                }
                return allOreDensities[i].oreLevels[density];
            }
        }
        
        Debug.LogError("Ore not found: " + uniqueName + " - " + density);
        return null;
    }

    public ShipCard GetShipCard(string uniqueName) {
        for (int i = 0; i < allShipCards.Length; i++) {
            if (allShipCards[i].uniqueName == uniqueName) {
                return allShipCards[i];
            }
        }
        
        Debug.LogError("Ship Card not found: " + uniqueName);
        return null;
    }

    public ShipCard BuildingDataToShipCard(BuildingData data) {
        for (int i = 0; i < allShipCards.Length; i++) {
            if (allShipCards[i].uniqueName == data.uniqueName) {
                return allShipCards[i];
            }
        }

        Debug.LogError("Ship Card not found: " + data.uniqueName);
        return null;
    }

    public BuildingData ShipCardToBuildingData(ShipCard card) {
        var dat = Object.Instantiate(cardBuildingData);;
        dat.uniqueName = card.uniqueName;

        return dat;
    }

    public OreSpawnSettings[] GetAllOres () {
        return allRecipeSets[0].myOres;
    }
    
    public TileSet IdToTileSet(int id) {
        if (id >= 0 && id < allTileSets.Length) {
            return allTileSets[id];
        } else {
            Debug.LogError($"Unknown tile id: {id} - Reverting to default");
            return allTileSets[0];
        }
    }
    
    public PlanetSchematic GetPlanetSchematic(string uniqueName) {
        for (int i = 0; i < allPlanetSchematics.Length; i++) {
            if (allPlanetSchematics[i].uniqueName == uniqueName) {
                return allPlanetSchematics[i];
            }
        }
        
        Debug.LogError("Planet Schematic not found: " + uniqueName);
        return null;
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

        oreId = -1;
        return false;
    }

    /// <summary>
    /// Ore Ids start from 1
    /// </summary>
    public Item OreIdToItem(int id) {
        if (OreIdToUniqueName(id, out string oreType)) {
            return GetItem(oreType);
        } else {
            return Item.GetEmpty();
        }
    }
    
    /// <summary>
    /// Ore Ids start from 1
    /// </summary>
    public bool OreIdToUniqueName (int id, out string oreType) {
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
        for (int i = 0; i < allBuildings.Length; i++) {
            if (allBuildings[i] != null) {
                if (allBuildings[i].uniqueName == uniqueName)
                    return allBuildings[i];
            }
        }
        
        for (int i = 0; i < allShipCards.Length; i++) {
            if (allShipCards[i] != null) {
                if (allShipCards[i].uniqueName == uniqueName)
                    return ShipCardToBuildingData(allShipCards[i]);
            }
        }
        
        Debug.LogError("Illegal building request: " + uniqueName);
        return null;
    }

    public BuildingData[] AllBuildings () {
        return allBuildings;
    }

    public int GetItemIDFromName (string uniqueName) {
        return GetItem(uniqueName).myId;
    }

    /// <summary>
    /// Should be used for all long term saving purposes
    /// The "UniqueName"s will never change! but the specific objects may change
    /// </summary>
    public Item GetItem (string uniqueName) {
        if (uniqueName.Length == 0)
            return Item.GetEmpty();
        
        //int idCounter = 0;
        for (int m = 0; m < allItemSets.Length; m++) {
            for (int i = 0; i < allItemSets[m].items.Length; i++) {
                if (allItemSets[m].items[i] != null) {
                    if (allItemSets[m].items[i].uniqueName == uniqueName) {
                        return allItemSets[m].items[i];
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

            for (int m = 0; m < allItemSets.Length; m++) {
                length += allItemSets[m].items.Length;
            }

            allItems = new Item[length];

            int lastIndex = 0;
            for (int m = 0; m < allItemSets.Length; m++) {
                allItemSets[m].items.CopyTo(allItems, lastIndex);
                lastIndex += allItemSets[m].items.Length;
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
        while (itemId > allItemSets[itemSet].items.Length) {
            itemId -= allItemSets[itemSet].items.Length;
            itemSet++;
            if (itemSet >= allItemSets.Length) {
                throw new NullReferenceException("The item you are requesting with id " + itemId + " does not exist!");
            }
        }
        return allItemSets[itemSet].items[itemId];
    }

    public int TotalItemCount () {
        int total = 0;
        for (int m = 0; m < allItemSets.Length; m++) {
            total += allItemSets[m].items.Length;
        }
        return total;
    }

    void AssignItemIds () {
        int idCounter = 0;
        for (int m = 0; m < allItemSets.Length; m++) {
            for (int i = 0; i < allItemSets[m].items.Length; i++) {
                if (allItemSets[m].items[i] != null) {
                    allItemSets[m].items[i].myItemSet = allItemSets[m];
                    allItemSets[m].items[i].myId = idCounter;
                }
                idCounter++;
            }
        }
    }
    
    
    [Serializable]
    public class CountedItem {
        public string itemUniqueName;
        public int count;

        public CountedItem(Item item, int count) {
            itemUniqueName = item.uniqueName;
            this.count = count;
        }
        
        public CountedItem(string uniqueName, int count) {
            itemUniqueName = uniqueName;
            this.count = count;
        }
    }

    
    /// <summary>
    /// Get Adapter connections as a list of counted items
    /// use "isLeft == true" for inputs and vice versa for outputs
    /// </summary>
    /// <param name="craftingNode"></param>
    /// <param name="isLeft"></param>
    /// <returns></returns>
    public List<CountedItem> GetConnections(CraftingNode craftingNode, bool isLeft) {
        var countedItems = new List<CountedItem>();
        for (int i = 0; i < craftingNode.myAdapters.Count; i++) {
            var myAdapter = craftingNode.myAdapters[i];
            if (myAdapter.isLeftAdapter == isLeft) {
                for (int n = 0; n < myAdapter.connections.Count; n++) {
                    var myConnection = myAdapter.connections[n];
                    countedItems.Add(new CountedItem(GetItemNode(myConnection.recipeSetName, myConnection.nodeId).itemUniqueName,  myConnection.count));
                }
            }
        }

        if (countedItems.Count == 0) {
            Debug.LogError($"This selection contains no connections {craftingNode.id} - {craftingNode.CraftingType} - {isLeft}");
        }
        return countedItems;
    }
    
    
    public ItemNode GetItemNode(string recipeSetUniqueName, int id) {
        foreach (var set in allRecipeSets) {
            if (set.recipeSetUniqueName == recipeSetUniqueName) {
                foreach (var itemNode in set.GetItemNodes()) {
                    if (itemNode.id == id) {
                        return itemNode;
                    }
                }
            }
        }

        return null;
    }

    public CraftingNode GetCraftingNode(string recipeSetUniqueName, int id) {
        foreach (var set in allRecipeSets) {
            if (set.recipeSetUniqueName == recipeSetUniqueName) {
                foreach (var craftingNode in set.GetCraftingNodes()) {
                    if (craftingNode.id == id) {
                        return craftingNode;
                    }
                }
            }
        }

        return null;
    }

    void GenerateCraftingProcessesArray () {
        List<CraftingNode> cp = new List<CraftingNode>();

        for (int i = 0; i < allRecipeSets.Length; i++) {
            for (int m = 0; m < allRecipeSets[i].GetCraftingNodes().Count; m++) {
                cp.Add((CraftingNode)allRecipeSets[i].GetCraftingNodes()[m]);
            }
        }

        myCraftingProcesses = cp.ToArray();
    }

    void DivideCraftingProcessesArray () {
        CraftingNodeTypeToIndex(CraftingNode.cTypes.Miner); // Generate the dictionary
        List<List<CraftingNode>> cp = new List<List<CraftingNode>>();

        for (int i = 0; i < cTypetoIndexMatch.Count; i++) {
            cp.Add(new List<CraftingNode>());
        }

        for (int i = 0; i < myCraftingProcesses.Length; i++) {
            try {
                cp[cTypetoIndexMatch[myCraftingProcesses[i].CraftingType]].Add(myCraftingProcesses[i]);
            } catch {
                Debug.LogError(i);
                Debug.LogError(myCraftingProcesses[i].CraftingType);
                Debug.LogError(cTypetoIndexMatch[myCraftingProcesses[i].CraftingType]);
                throw new Exception();
            }
        }

        myCraftingProcessesDivided = cp.Select(a => a.ToArray()).ToArray();
    }

    public CraftingNode[] GetCraftingProcessesOfTypeandTier (BuildingData.ItemType type, int tier) {
        int index = -1;
        switch (type) {
        case BuildingData.ItemType.Miner:           index = 0; break;
        case BuildingData.ItemType.Furnace:         index = 1; break;
        case BuildingData.ItemType.AssemblerSingle: index = 2; break;
        case BuildingData.ItemType.AssemblerDouble: index = 3; break;
        case BuildingData.ItemType.Press:           index = 4; break;
        case BuildingData.ItemType.Coiler:          index = 5; break;
        case BuildingData.ItemType.Cutter:          index = 6; break;
        case BuildingData.ItemType.Lab:             index = 7; break;
        case BuildingData.ItemType.Building:        index = 8; break;
        case BuildingData.ItemType.Processor:       index = 9; break;
        }
        if (index == -1) {
            //Debug.Log("Building does not support crafting! >> " + type.ToString());
            return null;
        }

        if (index < myCraftingProcessesDivided.Length) {
            return Array.FindAll(myCraftingProcessesDivided[index], x => x.tier == tier);
        } else {
            Debug.LogError("Crafting Process of correct type not found!");
            return null;
        }
    }

    public CraftingNode[][] GetAllCraftingProcessNodesDivided() {
        return myCraftingProcessesDivided;
    }


    Dictionary<CraftingNode.cTypes, int> cTypetoIndexMatch;
    public int CraftingNodeTypeToIndex(CraftingNode.cTypes type) {
        if (cTypetoIndexMatch == null) {
            cTypetoIndexMatch = new Dictionary<CraftingNode.cTypes, int>();
            cTypetoIndexMatch[CraftingNode.cTypes.Miner] = 0;
            cTypetoIndexMatch[CraftingNode.cTypes.Furnace] = 1;
            cTypetoIndexMatch[CraftingNode.cTypes.AssemblerSingle] = 2;
            cTypetoIndexMatch[CraftingNode.cTypes.AssemblerDouble] = 3;
            cTypetoIndexMatch[CraftingNode.cTypes.Press] = 4;
            cTypetoIndexMatch[CraftingNode.cTypes.Coiler] = 5;
            cTypetoIndexMatch[CraftingNode.cTypes.Cutter] = 6;
            cTypetoIndexMatch[CraftingNode.cTypes.Lab] = 7;
            cTypetoIndexMatch[CraftingNode.cTypes.Building] = 8;
            cTypetoIndexMatch[CraftingNode.cTypes.Processor] = 9;
        }

        return cTypetoIndexMatch[type];
        // Make sure this matches with the switch statement in GetCraftingProcessesOfType function!
    }
}

public delegate void GenericCallback ();
public delegate void ShipCardAddedCallback (ShipCard card, Position location);
public delegate void SuccessFailCallback (bool isSuccess);

