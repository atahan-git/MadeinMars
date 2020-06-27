using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCraftingController : MonoBehaviour
{
    public BuildingWorldObject myObj;
    public List<BeltBuildingObject> myBelts;

    public bool isActive = false;

    public int[] inputItemCounts = new int[1];
    public int[] inputItemIds = new int[] { 0 };
    public int[] inputItemRequirements = new int []{ 5 };

    public bool isCrafting = false;
    public int curCraftingProgress = 0;
    public int craftingProgressTickReq = 20;

    public int[] outputItemCounts = new int[1];
    public int[] outputItemIds = new int[] { 1 };
    public int[] outputItemAmounts = new int[] { 2 };


    public void SwapCraftingProcess () {

    }

    public void TakeItemsIn () {
        for (int i = 0; i < myBelts.Count; i++) {
            for (int k = 0; k < myBelts[i].myInputSlots.Count; k++) {
                if (myBelts[i].myInputSlots[k].TakeItem(inputItemIds[0]) != -1) {

                    inputItemCounts[0]++;
                } 
            }
        }

    }

    public void UpdateCraftingProcess () {
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
            }
        }
    }


    int outputIndex = 0;
    public void PutItemsOut () {
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
