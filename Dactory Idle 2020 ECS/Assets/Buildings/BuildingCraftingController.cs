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
public class BuildingCraftingController : MonoBehaviour
{
    public BuildingWorldObject myObj; // The master script
    public List<BeltBuildingObject> myBelts; // The BeltBuildingObjects our buildings occupy. We will put items on these and take items from these
    public List<TileData> myTiles; // the tiles the building occupies

    public bool isActive = false;

    public IProcess[] myCraftingProcesses = new IProcess[0];

    /// <summary>
    /// Do the setup tasks, which means figuring out which crafting processes this building can do based on the BuildingData and the RecipeSet
    /// </summary>
    /// <param name="mydat"></param>
    public void SetUpCraftingProcesses(BuildingData mydat) {
        CraftingNode[] ps = DataHolder.s.GetCraftingProcessesOfType(mydat.myType);
        if (ps != null) {
            if (mydat.myType == BuildingData.ItemType.Miner) {
                myCraftingProcesses = new IProcess[ps.Length];
                
                for (int i = 0; i < ps.Length; i++) {
                    if (DataHolder.s.UniqueNameToOreId(ps[i].outputs[0].itemUniqueName, out int oreId)) {
                        myCraftingProcesses[i] = new MiningProcess(myBelts, myTiles, oreId,
                            ps[i].outputs,
                            ps[i].timeCost
                        );
                    }
                }
            } else {
                myCraftingProcesses = new IProcess[ps.Length];

                for (int i = 0; i < ps.Length; i++) {
                    myCraftingProcesses[i] = new CraftingProcess(myBelts,
                        ps[i].inputs,
                        ps[i].outputs,
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
        // This is now done in a check in the mining IProcess update check
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

        /*if (isActive && mydat.myType != BuildingData.ItemType.Base)
            GetComponent<BuildingInfoDisplay>().SetUp();*/
    }

    /// <summary>
    /// Take items in for all the crafting processes
    /// </summary>
    public void TakeItemsIn () {
        for (int i = 0; i < myCraftingProcesses.Length; i++) {
            myCraftingProcesses[i].TakeItemsIn();
        }
    }

    public int lastCheckId = 0;
    /// <summary>
    /// Update the crafting processes. When we are done with one of them, try to continue crafting a different one
    /// </summary>
    /// <param name="efficiency"></param>
    /// <returns></returns>
    public float UpdateCraftingProcess (float efficiency) {
        for (int i = 0; i < myCraftingProcesses.Length +1; i++) {
            // Always continue from the last crafting we've made, so that we continue the same process
            if (myCraftingProcesses[lastCheckId].UpdateCraftingProcess(efficiency)) {
                ContinueAnimations();
                return myObj.myData.energyUse; // Return the energy use back for efficiency calculations
            } else {
                // if we can't process this one, continue with the next one
                lastCheckId++;
                lastCheckId = lastCheckId % myCraftingProcesses.Length;
            }
        }

        StopAnimations();

        return 0;
    }


    int outputIndex = 0;
    /// <summary>
    /// Put the crafting results out
    /// </summary>
    public void PutItemsOut () {
        for (int i = 0; i < myCraftingProcesses.Length; i++) {
            myCraftingProcesses[i].PutItemsOut(ref outputIndex);
        }
    }

    
    // ------------------------------------------------------
    // The following are things to run the animations properly
    
    
    public bool animationState = true;
    public AnimatedSpriteController[] anims = new AnimatedSpriteController[0];
    public bool isAnimated = true;
    public ParticleSystem[] particles = new ParticleSystem[0];
    public bool isParticled = true;
    
    bool GetAnims() {
        if (anims.Length <= 0) {
            anims = GetComponentsInChildren<AnimatedSpriteController>();
        }

        if (anims.Length <= 0) {
            isAnimated = false;
            return false;
        } else
            return true;
    }
    
    bool GetParticles() {
        if (particles.Length <= 0) {
            particles = GetComponentsInChildren<ParticleSystem>();
        }

        if (particles.Length <= 0) {
            isParticled = false;
            return false;
        } else
            return true;
    }
    void ContinueAnimations() {
        if (isAnimated) {
            if (!animationState) {
                if (GetAnims()) {
                    for (int i = 0; i < anims.Length; i++) {
                        anims[i].Play();
                    }

                    animationState = true;
                }
                
                if (isParticled) {
                    if (GetParticles()) {
                        for (int i = 0; i < particles.Length; i++) {
                            particles[i].Play();
                        }
                    }
                }
            }
        }
    }

    void StopAnimations() {
        if (isAnimated) {
            if (animationState) {
                if (GetAnims()) {
                    for (int i = 0; i < anims.Length; i++) {
                        anims[i].SmoothStop();
                    }

                    animationState = false;
                }

                if (isParticled) {
                    if (GetParticles()) {
                        for (int i = 0; i < particles.Length; i++) {
                            particles[i].Stop();
                        }
                    }
                }
            }
        }
    }
}

/// <summary>
/// Every crafting processes will be an IProcess
/// </summary>
public interface IProcess {
    void TakeItemsIn();
    bool UpdateCraftingProcess (float efficiency);
    void PutItemsOut (ref int outputIndex);
}

/// <summary>
/// This is the simplest process. Belongs to the "Base", this process will just take the items in, and put them to the player inventory.
/// </summary>
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

/// <summary>
/// This is the process used by the miners.
/// </summary>
public class MiningProcess : IProcess {
    public int oreInId = -1;

    public bool stillHaveOreLeft = true;
    public bool isCrafting = false;
    public float curCraftingProgress = 0;
    public float craftingProgressTickReq = 20;

    public int[] toBePutOnBeltsAmounts = new int[1];
    public int[] outputItemIds = new int[] {1};
    public int[] outputItemAmounts = new int[] {2};

    private List<BeltBuildingObject> myBelts;

    private List<TileData> myTiles;

    public MiningProcess(List<BeltBuildingObject> belts, List<TileData> tiles, int orein, List<CountedItemNode> _outputs, float timeReq) {
        myBelts = belts;
        myTiles = tiles;

        oreInId = orein;

        toBePutOnBeltsAmounts = new int[_outputs.Count];
        outputItemIds = new int[_outputs.Count];
        outputItemAmounts = new int[_outputs.Count];
        
        for (int i = 0; i < _outputs.Count; i++) {
            outputItemIds[i] = DataHolder.s.GetItemIDFromName(_outputs[i].itemUniqueName); 
            outputItemAmounts[i] = _outputs[i].count;
        }

        craftingProgressTickReq = (timeReq * BuildingMaster.buildingUpdatePerSecond);
    }

    private int cycleoffset = 0;
    private int oreUsed = 0;
    private bool initialized = false;
    public void TakeItemsIn() {
        if (stillHaveOreLeft && (!initialized || oreUsed>0)) {
            stillHaveOreLeft = false;
            
            for (int i = 0; i < myTiles.Count; i++) {
                int index = (i + cycleoffset) % myTiles.Count;
                if (myTiles[index].oreAmount > 0 && myTiles[index].oreType == oreInId) {
                    myTiles[index].oreAmount -= oreUsed;
                    oreUsed = 0;
                    if (myTiles[index].oreAmount < 0)
                        myTiles[index].oreAmount = 0;
                    
                    cycleoffset = (index + 1) % myTiles.Count;
                    stillHaveOreLeft = true;
                    initialized = true;
                    isCrafting = true;
                    break;
                }
            }
        }
    }

    public bool UpdateCraftingProcess(float efficiency) {
        if (stillHaveOreLeft && initialized) {
            curCraftingProgress += efficiency;

            if (curCraftingProgress >= craftingProgressTickReq) {
                oreUsed += outputItemAmounts[0];
                toBePutOnBeltsAmounts[0] += outputItemAmounts[0];
                curCraftingProgress = 0;
                isCrafting = false;
                return false;
            }

            return true;
        }

        return false;
    }

    public void PutItemsOut(ref int outputIndex) {
        int totalItemToOutput = 0;
        for (int i = 0; i < toBePutOnBeltsAmounts.Length; i++) {
            totalItemToOutput += toBePutOnBeltsAmounts[i];
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
                            toBePutOnBeltsAmounts[0]--;
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


/// <summary>
/// This is used by all the crafter buildings
/// </summary>
public class CraftingProcess : IProcess {
    public int[] toBeTakenFromBeltsAmounts = new int[1];
    public int[] inputItemIds = new int[] {1};
    public int[] inputItemAmounts = new int[] {2};

    public bool isCrafting = false;
    public float curCraftingProgress = 0;
    public float craftingProgressTickReq = 20;

    public int[] toBePutOnBeltsAmounts = new int[1];
    public int[] outputItemIds = new int[] {1};
    public int[] outputItemAmounts = new int[] {2};

    List<BeltBuildingObject> myBelts;

    public CraftingProcess(List<BeltBuildingObject> belts, List<CountedItemNode> _inputs, List<CountedItemNode> _outputs, float timeReq) {
        myBelts = belts;

        toBeTakenFromBeltsAmounts = new int[_inputs.Count];
        inputItemIds = new int[_inputs.Count];
        inputItemAmounts = new int[_inputs.Count];
        
        for (int i = 0; i < _inputs.Count; i++) {
            inputItemIds[i] = DataHolder.s.GetItemIDFromName(_inputs[i].itemUniqueName);
            inputItemAmounts[i] = _inputs[i].count;
        }


        toBePutOnBeltsAmounts = new int[_outputs.Count];
        outputItemIds = new int[_outputs.Count];
        outputItemAmounts = new int[_outputs.Count];
        
        for (int i = 0; i < _outputs.Count; i++) {
            outputItemIds[i] = DataHolder.s.GetItemIDFromName(_outputs[i].itemUniqueName);
            outputItemAmounts[i] = _outputs[i].count;
        }


        craftingProgressTickReq = (timeReq * BuildingMaster.buildingUpdatePerSecond);
    }


    public void TakeItemsIn() {
        for (int i = 0; i < myBelts.Count; i++) {
            for (int k = 0; k < myBelts[i].myInputSlots.Count; k++) {
                for (int item = 0; item < inputItemIds.Length; item++) {
                    if (toBeTakenFromBeltsAmounts[item] < inputItemAmounts[item] * 2) {
                        if (myBelts[i].myInputSlots[k].TakeItem(inputItemIds[item]) != -1) {
                            toBeTakenFromBeltsAmounts[item]++;
                        }
                    }
                }
            }
        }
    }

    public bool UpdateCraftingProcess(float efficiency) {
        if (!isCrafting) {
            for (int i = 0; i < toBePutOnBeltsAmounts.Length; i++) {
                if (toBePutOnBeltsAmounts[i] >= outputItemAmounts[i] * 2) {
                    return false;
                }
            }

            for (int i = 0; i < toBeTakenFromBeltsAmounts.Length; i++) {
                if (toBeTakenFromBeltsAmounts[i] < inputItemAmounts[i]) {
                    return false;
                }
            }

            for (int i = 0; i < toBeTakenFromBeltsAmounts.Length; i++) {
                toBeTakenFromBeltsAmounts[i] -= inputItemAmounts[i];
            }

            isCrafting = true;
        }

        if (isCrafting) {
            curCraftingProgress += efficiency;

            if (curCraftingProgress >= craftingProgressTickReq) {
                toBePutOnBeltsAmounts[0] += outputItemAmounts[0];
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
        for (int i = 0; i < toBePutOnBeltsAmounts.Length; i++) {
            totalItemToOutput += toBePutOnBeltsAmounts[i];
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
                            toBePutOnBeltsAmounts[0]--;
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
