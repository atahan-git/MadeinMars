using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BuildingCraftingController : MonoBehaviour
{
    public BuildingWorldObject myObj;
    public List<BeltBuildingObject> myBelts;

    public bool isActive = false;

    public CraftingProcess[] myCraftingProcesses = new CraftingProcess[0];


    public void SetUpCraftingProcesses (BuildingData mydat) {
        CraftingProcessNode[] ps = DataHolder.s.GetCraftingProcessesOfType(mydat.myType);
        if (ps != null) {
            myCraftingProcesses = new CraftingProcess[ps.Length];

            for (int i = 0; i < ps.Length; i++) {
                myCraftingProcesses[i] = new CraftingProcess(myBelts,
                    ps[i].inputItemUniqueNames.GetRange(0, ps[i].inputItemUniqueNames.Count - 1),
                    ps[i].inputItemCounts.GetRange(0, ps[i].inputItemCounts.Count),
                    ps[i].outputItemUniqueNames.GetRange(0, ps[i].outputItemUniqueNames.Count - 1),
                    ps[i].outputItemCounts.GetRange(0, ps[i].outputItemCounts.Count),
                    ps[i].timeCost
                    );
            }
            GetComponent<BuildingInfoDisplay>().SetUp();
        } else {
            Debug.Log(gameObject.name + " my type: " + mydat.myType.ToString() + " doesnt have any processess!");
        }
    }

    public void TakeItemsIn () {
        for (int i = 0; i < myCraftingProcesses.Length; i++) {
            myCraftingProcesses[i].TakeItemIn();
        }
    }

    public int lastCheckId = 0;
    public void UpdateCraftingProcess () {
        for (int i = 0; i < myCraftingProcesses.Length; i++) {
            // Always continue from the last crafting we've made, so that we continue the same process
            if (myCraftingProcesses[lastCheckId].UpdateCraftingProcess()) {
                return;
            } else {
                // if we can't process this one, continue with the next one
                lastCheckId++;
                lastCheckId = lastCheckId % myCraftingProcesses.Length;
            }
        }
    }


    int outputIndex = 0;
    public void PutItemsOut () {
        for (int i = 0; i < myCraftingProcesses.Length; i++) {
            myCraftingProcesses[i].PutItemsOut(ref outputIndex);
        }
    }
}

public class CraftingProcess {
    public int[] inputItemCounts = new int[1];
    public int[] inputItemIds = new int[] { 0 };
    public int[] inputItemRequirements = new int[] { 5 };

    public bool isCrafting = false;
    public int curCraftingProgress = 0;
    public int craftingProgressTickReq = 20;

    public int[] outputItemCounts = new int[1];
    public int[] outputItemIds = new int[] { 1 };
    public int[] outputItemAmounts = new int[] { 2 };

    List<BeltBuildingObject> myBelts;

    public CraftingProcess (List<BeltBuildingObject> belts, List<string> inputItems, List<int> inputCounts, List<string> outputItems, List<int> outputCounts, float timeReq) {
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

        craftingProgressTickReq = (int)(timeReq * BuildingMaster.buildingUpdatePerSecond);
    }


    public void TakeItemIn () {
        for (int i = 0; i < myBelts.Count; i++) {
            for (int k = 0; k < myBelts[i].myInputSlots.Count; k++) {
                if (myBelts[i].myInputSlots[k].TakeItem(inputItemIds[0]) != -1) {
                    inputItemCounts[0]++;
                }
            }
        }
    }

    public bool UpdateCraftingProcess () {
        if (!isCrafting && inputItemCounts[0] >= inputItemRequirements[0]) {
            inputItemCounts[0] -= inputItemRequirements[0];
            isCrafting = true;
        }

        if (isCrafting) {
            curCraftingProgress++;

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

    public void PutItemsOut (ref int outputIndex) {
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
