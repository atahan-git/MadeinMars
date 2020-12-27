using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInOutController : MonoBehaviour
{
    
    BuildingWorldObject myObj; // The master script
    List<BeltBuildingObject> myBelts; // The BeltBuildingObjects our buildings occupy. We will put items on these and take items from these
    List<TileData> myTiles; // the tiles the building occupies
    
    BuildingInventoryController inventory;

    private BuildingData myData;
    
    public bool isActive = true;
    
    public enum InOutType {
        NormalBuilding, Miner, Base
    }

    public InOutType myType = InOutType.NormalBuilding;


    public void SetUp(BuildingWorldObject _myObj,BuildingData mydat, BuildingInventoryController _inventory) {
        myObj = _myObj;
        myBelts = myObj.myBelts;
        myTiles = myObj.myTiles;
        inventory = _inventory;
        myData = mydat;

        switch (myData.myType) {
            case BuildingData.ItemType.Base:
                myType = InOutType.Base;
                break;

            case BuildingData.ItemType.Miner:
                myType = InOutType.Miner;
                break;
            default:
                myType = InOutType.NormalBuilding;
                break;
        }

        // If our type is miner, we will occupy more tiles for mining purposes
        if (myType == InOutType.Miner) {
            myTiles = new List<TileData>();
            
            // A range of 2 means we will mine on the tiles:
            /*
             *    X  X  X  X  X
             *    X  X  X  X  X
             *    X  X  O  X  X
             *    X  X  X  X  X
             *    X  X  X  X  X
             *
             *  Where "O" is the place the center of the miner is.
             */
            int range = myData.buildingGrade + 2;

            for (int x = _myObj.myPos.x - range; x <= _myObj.myPos.x + range; x++) {
                for (int y = _myObj.myPos.y - range; y <= _myObj.myPos.y + range; y++) {
                    myTiles.Add(Grid.s.GetTile(new Position(x,y)));
                }
            }
        }
    }

    public void TakeItemsIn() {
        switch (myType) {
            case InOutType.Base:
                Base_TakeItemsIn();
                break;
            case InOutType.Miner:
                Miner_TakeItemsIn();
                break;
            case InOutType.NormalBuilding:
                Normal_TakeItemsIn();
                break;
        }

        inventory.InventoryContentsChanged();
    }

    /// <summary>
    /// take the relevant items out of the belts, if there is inventory space available
    /// </summary>
    void Normal_TakeItemsIn() {
        for (int i = 0; i < myBelts.Count; i++) {
            for (int k = 0; k < myBelts[i].myInputSlots.Count; k++) {
                for (int item = 0; item < inventory.inventory.Count; item++) {
                    if (!inventory.inventory[item].isOutputSlot) {
                        if (inventory.inventory[item].count < inventory.inventory[item].maxCount) {
                            if (myBelts[i].myInputSlots[k].TakeItem(inventory.inventory[item].myItem.myId) != -1) {
                                inventory.inventory[item].count++;
                            }
                        }
                    }
                }
            }
        }
    }

    
    /// <summary>
    /// Count all the ore we are sitting on top of
    /// </summary>
    void Miner_TakeItemsIn() {
        for (int item = 0; item < inventory.inventory.Count; item++) {
            if (!inventory.inventory[item].isOutputSlot) {
                inventory.inventory[item].count = 0;
                for (int i = 0; i < myTiles.Count; i++) {
                    DataHolder.s.UniqueNameToOreId(inventory.inventory[item].myItem.uniqueName, out int oreId);
                    if (myTiles[i].oreAmount > 0 && myTiles[i].oreType == oreId) {
                        inventory.inventory[item].count += myTiles[i].oreAmount;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Take all the items, and put them to the Buildings's inventory
    /// </summary>
    void Base_TakeItemsIn() {
        for (int i = 0; i < myBelts.Count; i++) {
            for (int k = 0; k < myBelts[i].myInputSlots.Count; k++) {
                int myItem = myBelts[i].myInputSlots[k].ItemId();
                if (myItem != -1) {
                    if (Player_InventoryController.s.TryAddItem(DataHolder.s.GetItem(myItem))){
                        myBelts[i].myInputSlots[k].TakeItem(myItem);
                    }
                }
            }
        }
    }


    public int beltOutOffset = 0;
    /// <summary>
    /// Take items in the inventory marked as 'output slot' out
    /// </summary>
    public void PutItemsOut() {
        
        int totalItemToOutput = 0;
        for (int i = 0; i < inventory.inventory.Count; i++) {
            if (inventory.inventory[i].isOutputSlot) {
                totalItemToOutput += inventory.inventory[i].count;
            }
        }

        if (totalItemToOutput <= 0) {
            return;
        }

        var curOffset = 0;
        for (int m = 0; m < inventory.inventory.Count; m++) {
            if (inventory.inventory[m].isOutputSlot && inventory.inventory[m].count > 0) {
                for (int i = 0; i < myBelts.Count; i++) {
                    var beltIndex = (i + beltOutOffset) % myBelts.Count;
                    for (int k = 0; k < myBelts[beltIndex].myCreationSlots.Count; k++) {

                        if (inventory.inventory[m].count > 0) {
                            var slotIndex = (k);
                            if (myBelts[beltIndex].myCreationSlots[slotIndex].CreateItem(inventory.inventory[m].myItem.myId)) {
                                inventory.inventory[m].count--;
                                curOffset = beltIndex;
                                if (myType == InOutType.Miner)
                                    Miner_RemoveItemFromTiles(inventory.inventory[m].myItem.uniqueName);
                            }
                        }
                    }
                }
            }
        }

        beltOutOffset = curOffset+1;
        
        if (beltOutOffset > myBelts.Count)
            beltOutOffset = 0;
        
        inventory.InventoryContentsChanged();
    }


    private int offset = 0;
    void Miner_RemoveItemFromTiles(string uniqueName) {
        for (int i = 0; i < myTiles.Count; i++) {
            DataHolder.s.UniqueNameToOreId(uniqueName, out int oreId);
            if (myTiles[(i + offset) % myTiles.Count].oreAmount > 0 && myTiles[i].oreType == oreId) {
                myTiles[(i + offset) % myTiles.Count].oreAmount -= 1;
                offset = i+1;
                return;
            }
        }
    }
}
