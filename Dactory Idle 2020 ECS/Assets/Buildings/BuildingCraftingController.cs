using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingCraftingController : MonoBehaviour
{
    public BuildingWorldObject myObj;
    public List<BeltBuildingObject> myBelts;
    public List<TileData> myTiles;

    public bool isActive = false;

    public IProcess[] myCraftingProcesses = new IProcess[0];

    public void SetUpCraftingProcesses(BuildingData mydat) {
        CraftingProcessNode[] ps = DataHolder.s.GetCraftingProcessesOfType(mydat.myType);
        if (ps != null) {
            if (mydat.myType == BuildingData.ItemType.Miner) {
                myCraftingProcesses = new IProcess[ps.Length];

                for (int i = 0; i < ps.Length; i++) {
                    if (DataHolder.s.UniqueNameToOreId(ps[i].outputItemUniqueNames[0], out int oreId)) {
                        myCraftingProcesses[i] = new MiningProcess(myBelts, myTiles, oreId,
                            ps[i].outputItemUniqueNames.GetRange(0, ps[i].outputItemUniqueNames.Count - 1),
                            ps[i].outputItemCounts.GetRange(0, ps[i].outputItemCounts.Count),
                            ps[i].timeCost
                        );
                    }
                }
            } else {
                myCraftingProcesses = new IProcess[ps.Length];

                for (int i = 0; i < ps.Length; i++) {
                    myCraftingProcesses[i] = new CraftingProcess(myBelts,
                        ps[i].inputItemUniqueNames.GetRange(0, ps[i].inputItemUniqueNames.Count - 1),
                        ps[i].inputItemCounts.GetRange(0, ps[i].inputItemCounts.Count),
                        ps[i].outputItemUniqueNames.GetRange(0, ps[i].outputItemUniqueNames.Count - 1),
                        ps[i].outputItemCounts.GetRange(0, ps[i].outputItemCounts.Count),
                        ps[i].timeCost
                    );
                }
            }
        } else if (mydat.myType == BuildingData.ItemType.Base) {
            myCraftingProcesses = new IProcess[1];
            myCraftingProcesses[0] = new InputProcess(myBelts);
        } else {
            Debug.Log(gameObject.name + " my type: " + mydat.myType.ToString() + " doesnt have any processess!");
        }

        if (myCraftingProcesses.Length > 0) {
            isActive = true;
        }


        // Miners are special, they should only be mining if there is an item deposit under them!
        // This is now done in a check in the mining process thing
        /*if (mydat.myType == BuildingData.ItemType.Miner && isActive) {
            List<IProcess> possibleOreProcesses = new List<IProcess>();
            foreach (TileData tile in myObj.myTiles) {
                if (tile.oreAmount > 0) { 

                if (DataHolder.s.OreIdtoUniqueName(tile.oreType, out string oreType)) {
                    int itemId = DataHolder.s.GetItemIDFromName(oreType);
                    foreach (MiningProcess cp in myCraftingProcesses) {
                        if (cp.outputItemIds[0] == itemId) {
                            possibleOreProcesses.Add(cp);
                            break;
                        }
                    }
                }}
            }

            if (possibleOreProcesses.Count > 0) {
                isActive = true;
                myCraftingProcesses = possibleOreProcesses.ToArray();
            } else {
                isActive = false;
            }
        }*/

        if (isActive && mydat.myType != BuildingData.ItemType.Base)
            GetComponent<BuildingInfoDisplay>().SetUp();
    }

    public void TakeItemsIn () {
        for (int i = 0; i < myCraftingProcesses.Length; i++) {
            myCraftingProcesses[i].TakeItemsIn();
        }
    }

    public int lastCheckId = 0;
    public float UpdateCraftingProcess (float efficiency) {
        for (int i = 0; i < myCraftingProcesses.Length; i++) {
            // Always continue from the last crafting we've made, so that we continue the same process
            if (myCraftingProcesses[lastCheckId].UpdateCraftingProcess(efficiency)) {
                return myObj.myData.energyUse;
            } else {
                // if we can't process this one, continue with the next one
                lastCheckId++;
                lastCheckId = lastCheckId % myCraftingProcesses.Length;
            }
        }
        return 0;
    }


    int outputIndex = 0;
    public void PutItemsOut () {
        for (int i = 0; i < myCraftingProcesses.Length; i++) {
            myCraftingProcesses[i].PutItemsOut(ref outputIndex);
        }
    }
}

public interface IProcess {
    void TakeItemsIn();
    bool UpdateCraftingProcess (float efficiency);
    void PutItemsOut (ref int outputIndex);
}

public class InputProcess : IProcess {

    List<BeltBuildingObject> myBelts;

    public InputProcess (List<BeltBuildingObject> belts) {
        myBelts = belts;
    }
    public void TakeItemsIn () {
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
    public bool UpdateCraftingProcess (float efficiency) {
        return false;
    }
    public void PutItemsOut (ref int outputIndex) {
		
	}


}

public class MiningProcess : IProcess {
    public int oreInId = -1;

    public bool stillHaveOreLeft = true;
    public bool isCrafting = false;
    public float curCraftingProgress = 0;
    public float craftingProgressTickReq = 20;

    public int[] outputItemCounts = new int[1];
    public int[] outputItemIds = new int[] {1};
    public int[] outputItemAmounts = new int[] {2};

    private List<BeltBuildingObject> myBelts;

    private List<TileData> myTiles;

    public MiningProcess(List<BeltBuildingObject> belts, List<TileData> tiles, int orein, List<string> outputItems, List<int> outputCounts, float timeReq) {
        myBelts = belts;
        myTiles = tiles;

        oreInId = orein;

        outputItemCounts = new int[outputItems.Count];
        outputItemIds = new int[outputItems.Count];
        outputItemAmounts = outputCounts.ToArray();

        for (int i = 0; i < outputItems.Count; i++) {
            outputItemIds[i] = DataHolder.s.GetItemIDFromName(outputItems[i]);
        }

        craftingProgressTickReq = (timeReq * BuildingMaster.buildingUpdatePerSecond);
    }

    private int cycleoffset = 0;
    public void TakeItemsIn() {
        if (stillHaveOreLeft && !isCrafting) {
            for (int i = 0; i < myTiles.Count; i++) {
                if (myTiles[(i + cycleoffset) % myTiles.Count].oreAmount > 0 && myTiles[(i + cycleoffset) % myTiles.Count].oreType == oreInId) {
                    myTiles[(i + cycleoffset) % myTiles.Count].oreAmount -= 1;
                    cycleoffset = (cycleoffset + i + 1) % myTiles.Count;
                    isCrafting = true;
                    break;
                }
            }

            if (!isCrafting) {
                stillHaveOreLeft = false;
            }
        }
    }

    public bool UpdateCraftingProcess(float efficiency) {
        if (!isCrafting) {
            return false;
        }

        if (isCrafting) {
            curCraftingProgress += efficiency;

            if (curCraftingProgress >= craftingProgressTickReq) {
                outputItemCounts[0] += outputItemAmounts[0];
                isCrafting = false;
                curCraftingProgress = 0;
                return false;
            }

            return true;
        }

        return false;
    }

    public void PutItemsOut(ref int outputIndex) {
        int totalItemToOutput = 0;
        for (int i = 0; i < outputItemCounts.Length; i++) {
            totalItemToOutput += outputItemCounts[i];
        }

        if (totalItemToOutput <= 0) {
            return;
        }

        bool isFirstLoopDone = false;
        while (totalItemToOutput > 0) {
            int curCount = 0;
            for (int i = 0; i < myBelts.Count; i++) {
                for (int k = 0; k < myBelts[i].myCreationSlots.Count; k++) {
                    if (curCount == outputIndex) {
                        outputIndex++;
                        if (myBelts[i].myCreationSlots[k].CreateItem(outputItemIds[0])) {
                            outputItemCounts[0]--;
                            totalItemToOutput--;

                            if (totalItemToOutput <= 0) {
                                return;
                            }
                        }
                    }

                    curCount++;
                }
            }

            outputIndex = 0;
            if (isFirstLoopDone)
                return;
            isFirstLoopDone = true;
        }
    }
}


public class CraftingProcess : IProcess {
    public int[] inputItemCounts = new int[1];
    public int[] inputItemIds = new int[] {0};
    public int[] inputItemRequirements = new int[] {5};

    public bool isCrafting = false;
    public float curCraftingProgress = 0;
    public float craftingProgressTickReq = 20;

    public int[] outputItemCounts = new int[1];
    public int[] outputItemIds = new int[] {1};
    public int[] outputItemAmounts = new int[] {2};

    List<BeltBuildingObject> myBelts;

    public CraftingProcess(List<BeltBuildingObject> belts, List<string> inputItems, List<int> inputCounts,
        List<string> outputItems, List<int> outputCounts, float timeReq) {
        myBelts = belts;

        inputItemCounts = new int[inputItems.Count];
        inputItemIds = new int[inputItems.Count];
        inputItemRequirements = inputCounts.ToArray();

        for (int i = 0; i < inputItems.Count; i++) {
            inputItemIds[i] = DataHolder.s.GetItemIDFromName(inputItems[i]);
        }

        outputItemCounts = new int[outputItems.Count];
        outputItemIds = new int[outputItems.Count];
        outputItemAmounts = outputCounts.ToArray();

        for (int i = 0; i < outputItems.Count; i++) {
            outputItemIds[i] = DataHolder.s.GetItemIDFromName(outputItems[i]);
        }

        craftingProgressTickReq = (timeReq * BuildingMaster.buildingUpdatePerSecond);
    }


    public void TakeItemsIn() {
        for (int i = 0; i < myBelts.Count; i++) {
            for (int k = 0; k < myBelts[i].myInputSlots.Count; k++) {
                for (int item = 0; item < inputItemIds.Length; item++) {
                    if (inputItemCounts[item] < inputItemRequirements[item] * 2) {
                        if (myBelts[i].myInputSlots[k].TakeItem(inputItemIds[item]) != -1) {
                            inputItemCounts[item]++;
                        }
                    }
                }
            }
        }
    }

    public bool UpdateCraftingProcess(float efficiency) {
        if (!isCrafting) {
            for (int i = 0; i < outputItemCounts.Length; i++) {
                if (outputItemCounts[i] >= outputItemAmounts[i] * 2) {
                    return false;
                }
            }

            for (int i = 0; i < inputItemCounts.Length; i++) {
                if (inputItemCounts[i] < inputItemRequirements[i]) {
                    return false;
                }
            }

            for (int i = 0; i < inputItemCounts.Length; i++) {
                inputItemCounts[i] -= inputItemRequirements[i];
            }

            isCrafting = true;
        }

        if (isCrafting) {
            curCraftingProgress += efficiency;

            if (curCraftingProgress >= craftingProgressTickReq) {
                outputItemCounts[0] += outputItemAmounts[0];
                isCrafting = false;
                curCraftingProgress = 0;
                return false;
            }

            return true;
        }

        return false;
    }

    public void PutItemsOut(ref int outputIndex) {
        int totalItemToOutput = 0;
        for (int i = 0; i < outputItemCounts.Length; i++) {
            totalItemToOutput += outputItemCounts[i];
        }

        if (totalItemToOutput <= 0) {
            return;
        }

        bool isFirstLoopDone = false;
        while (totalItemToOutput > 0) {
            int curCount = 0;
            for (int i = 0; i < myBelts.Count; i++) {
                for (int k = 0; k < myBelts[i].myCreationSlots.Count; k++) {
                    if (curCount == outputIndex) {
                        outputIndex++;
                        if (myBelts[i].myCreationSlots[k].CreateItem(outputItemIds[0])) {
                            outputItemCounts[0]--;
                            totalItemToOutput--;

                            if (totalItemToOutput <= 0) {
                                return;
                            }
                        }
                    }

                    curCount++;
                }
            }

            outputIndex = 0;
            if (isFirstLoopDone)
                return;
            isFirstLoopDone = true;
        }
    }
}
