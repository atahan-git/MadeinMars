using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;



/// <summary>
/// This exists on all buildings that can craft.
/// Deals with everything related to building crafting
/// The periodic updates to this should come from the BuildingMaster.cs script.
/// </summary>
[Serializable]
public class BuildingCraftingController 
{
    
    BuildingInventoryController inventory;
    BuildingData myData;

    public bool isActive = false;
    public bool isCrafting = false;

    public int lastCheckId = 0;
    public CraftingProcess[] myCraftingProcesses = new CraftingProcess[0];

    public event GenericCallback continueAnimationsEvent;
    public event GenericCallback stopAnimationsEvent;


    public void SetMinerType(string oreUniqueName) {
        for (int i = 0; i < myCraftingProcesses.Length; i++) {
            if (myCraftingProcesses[i].outputItems[0].uniqueName != oreUniqueName) {
                myCraftingProcesses[i].isEnabled = false;
            }
        }
    }
    
    /// <summary>
    /// Do the setup, and continue leftover crafting processes (used when loading from save
    /// </summary>
    /// <param name="mydat"></param>
    public void SetUp(BuildingData mydat, BuildingInventoryController _inv, int _lastCheckId, float[] CraftingProcessProgress) {
        SetUp(mydat, _inv);

        lastCheckId = _lastCheckId;
        for (int i = 0; i < myCraftingProcesses.Length; i++) {
            if (i < CraftingProcessProgress.Length) {
                if (CraftingProcessProgress[i] >= 0) {
                    myCraftingProcesses[i].isCrafting = true;
                    myCraftingProcesses[i].curCraftingProgress = CraftingProcessProgress[i];
                }else if (CraftingProcessProgress[i] <= -2) {
                    myCraftingProcesses[i].isEnabled = false;
                }
            }
        }
        if(myCraftingProcesses.Length > 0)
            lastCheckId = lastCheckId % myCraftingProcesses.Length;
    }

    /// <summary>
    /// Do the setup tasks, which means figuring out which crafting processes this building can do based on the BuildingData and the RecipeSet
    /// </summary>
    /// <param name="mydat"></param>
    public void SetUp(BuildingData mydat, BuildingInventoryController _inv) {
        inventory = _inv;
        myData = mydat;

        CraftingNode[] ps = DataHolder.s.GetCraftingProcessesOfType(mydat.myType);
        if (ps != null) {
            if (mydat.myType == BuildingData.ItemType.Miner) {
                //myCraftingProcesses = new CraftingProcess[ps.Length];
                myCraftingProcesses = new CraftingProcess[ps.Length];
                
                for (int i = 0; i < ps.Length; i++) {
                    if (DataHolder.s.UniqueNameToOreId(DataHolder.s.GetConnections(ps[i], false)[0].itemUniqueName, out int oreId)) {
                        myCraftingProcesses[i] = new CraftingProcess(
                            new List<DataHolder.CountedItem>(),
                            DataHolder.s.GetConnections(ps[i], false),
                            ps[i].timeCost
                        );
                    }
                }
            } else {
                myCraftingProcesses = new CraftingProcess[ps.Length];

                for (int i = 0; i < ps.Length; i++) {
                    myCraftingProcesses[i] = new CraftingProcess(
                        DataHolder.s.GetConnections(ps[i], true),
                        DataHolder.s.GetConnections(ps[i], false),
                        ps[i].timeCost
                    );
                }
            }
        } else if (mydat.myType == BuildingData.ItemType.Base) {
            //myCraftingProcesses = new IProcess[1];
            //myCraftingProcesses[0] = new InputProcess(myBelts);
            
            // This logic is now handled by the BuildingInventoryController and the BuildingInOutController
            
        } 

        if (myCraftingProcesses.Length > 0) {
            isActive = true;
        } else {
            stopAnimationsEvent?.Invoke();
        }
    }

    /// <summary>
    /// Update the crafting processes. When we are done with one of them, try to continue crafting a different one
    /// </summary>
    /// <param name="energySupply"></param>
    /// <returns></returns>
    public float UpdateCraftingProcess (float energySupply) {
        var workerSupply = 1f;
        // If there are workers, its either worker working, or dwellers eating food.
        // They both affect the efficiency in the same way
        if (inventory.maxWorkers > 0) {
            workerSupply= (float)inventory.workerCount / inventory.maxWorkers;
        }

        if (inventory.maxDwellers > 0) {
            workerSupply= (float)inventory.dwellerCount/ inventory.maxDwellers;
        }

        var productionCapacity = energySupply * workerSupply;
        
        for (int i = 0; i < myCraftingProcesses.Length +1; i++) {
            // Always continue from the last crafting we've made, so that we continue the same process
            if (myCraftingProcesses[lastCheckId].UpdateCraftingProcess(productionCapacity, inventory)) {
                continueAnimationsEvent?.Invoke();
                isCrafting = true;
                return myData.energyUse; // Return the energy use back for energySupply calculations
            } else {
                // if we can't process this one, continue with the next one
                lastCheckId++;
                lastCheckId = lastCheckId % myCraftingProcesses.Length;
            }
        }

        stopAnimationsEvent?.Invoke();
        isCrafting = false;
        
        return 0;
    }


    public float[] GetCraftingProcessProgress() {
        var progress = new float[myCraftingProcesses.Length];
        for (int i = 0; i < myCraftingProcesses.Length; i++) {
            if (myCraftingProcesses[i].isEnabled) {
                if (myCraftingProcesses[i].isCrafting) {
                    progress[i] = myCraftingProcesses[i].curCraftingProgress;
                } else {
                    progress[i] = -1;
                }
            } else {
                progress[i] = -2;
            }
        }

        return progress;
    }

    // ------------------------------------------------------
    // The following are things to run the animations properly
    
}


/// <summary>
/// This is used by all the crafter buildings
/// </summary>
[Serializable]
public class CraftingProcess {
    public bool isEnabled = true;
    
    public int[] inputItemIds = new int[] {1};
    public Item[] inputItems = new Item[0];
    public int[] inputItemAmounts = new int[] {2};

    public bool isCrafting = false;
    public float curCraftingProgress = 0;
    public float craftingProgressTickReq = 20;

    public int[] outputItemIds = new int[] {1};
    public Item[] outputItems = new Item[0];
    public int[] outputItemAmounts = new int[] {2};


    public CraftingProcess(List<DataHolder.CountedItem> _inputs, List<DataHolder.CountedItem> _outputs, float timeReq) {

        inputItemIds = new int[_inputs.Count];
        inputItems = new Item[_inputs.Count];
        inputItemAmounts = new int[_inputs.Count];
        
        for (int i = 0; i < _inputs.Count; i++) {
            inputItemIds[i] = DataHolder.s.GetItemIDFromName(_inputs[i].itemUniqueName);
            inputItems[i] = DataHolder.s.GetItem(_inputs[i].itemUniqueName);
            inputItemAmounts[i] = _inputs[i].count;
        }
        
        outputItemIds = new int[_outputs.Count];
        outputItems = new Item[_outputs.Count];
        outputItemAmounts = new int[_outputs.Count];
        
        for (int i = 0; i < _outputs.Count; i++) {
            outputItemIds[i] = DataHolder.s.GetItemIDFromName(_outputs[i].itemUniqueName);
            outputItems[i] = DataHolder.s.GetItem(_outputs[i].itemUniqueName);
            outputItemAmounts[i] = _outputs[i].count;
        }


        craftingProgressTickReq = (timeReq * FactoryMaster.SimUpdatePerSecond);
    }
    

    public bool UpdateCraftingProcess(float efficiency, BuildingInventoryController inventory) {
        if (!isEnabled)
            return false;
        
        if (!isCrafting) {
            for (int i = 0; i < outputItems.Length; i++) {
                if (!inventory.CheckAddItem(outputItems[i], outputItemAmounts[i], true)) {
                    return false;
                }
            }

            for (int i = 0; i < inputItems.Length; i++) {
                if (!inventory.CheckTakeItem(inputItems[i],inputItemAmounts[i], false)) {
                    return false;
                }
            }

            for (int i = 0; i < inputItems.Length; i++) {
                inventory.TryTakeItem(inputItems[i], inputItemAmounts[i], false);
            }

            isCrafting = true;
        }

        if (isCrafting) {
            curCraftingProgress += efficiency;

            if (curCraftingProgress >= craftingProgressTickReq) {
                for (int i = 0; i < outputItems.Length; i++) {
                    inventory.TryAddItem(outputItems[i], outputItemAmounts[i], true);
                }
                isCrafting = false;
                curCraftingProgress = 0;
                return false;
            }

            return true;
        }

        return false;
    }


    public (Item, int)[] GetInputItems() {
        (Item, int)[] inputs = new (Item, int)[inputItems.Length];

        for (int i = 0; i < inputs.Length; i++) {
            inputs[i].Item1 = inputItems[i];
            inputs[i].Item2 = inputItemAmounts[i];
        }

        return inputs;
    }

    public (Item, int)[] GetOutputItems() {
        (Item, int)[] outputs = new (Item, int)[outputItems.Length];

        for (int i = 0; i < outputs.Length; i++) {
            outputs[i].Item1 = outputItems[i];
            outputs[i].Item2 = outputItemAmounts[i];
        }

        return outputs;
    }
}
