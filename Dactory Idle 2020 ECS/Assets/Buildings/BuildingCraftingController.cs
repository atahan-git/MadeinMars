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
    BuildingWorldObject myObj; // The master script
    List<BeltBuildingObject> myBelts; // The BeltBuildingObjects our buildings occupy. We will put items on these and take items from these
    List<TileData> myTiles; // the tiles the building occupies
    
    BuildingInventoryController inventory;

    public bool isActive = false;

    public IProcess[] myCraftingProcesses = new IProcess[0];

    /// <summary>
    /// Do the setup tasks, which means figuring out which crafting processes this building can do based on the BuildingData and the RecipeSet
    /// </summary>
    /// <param name="mydat"></param>
    public void SetUp(BuildingData mydat, BuildingWorldObject _myObj, BuildingInventoryController _inv) {
        myObj = _myObj;
        myBelts = myObj.myBelts;
        myTiles = myObj.myTiles;
        inventory = _inv;
        
        CraftingNode[] ps = DataHolder.s.GetCraftingProcessesOfType(mydat.myType);
        if (ps != null) {
            if (mydat.myType == BuildingData.ItemType.Miner) {
                myCraftingProcesses = new IProcess[ps.Length];
                
                for (int i = 0; i < ps.Length; i++) {
                    if (DataHolder.s.UniqueNameToOreId(ps[i].outputs[0].itemUniqueName, out int oreId)) {
                        myCraftingProcesses[i] = new CraftingProcess(myBelts,
                            ps[i].outputs,
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
            //myCraftingProcesses = new IProcess[1];
            //myCraftingProcesses[0] = new InputProcess(myBelts);
            
            // This logic is now handled by the BuildingInventoryController and the BuildingInOutController
            
        } else {
            Debug.Log(gameObject.name + " my type: " + mydat.myType.ToString() + " doesnt have any processess!");
        }

        if (myCraftingProcesses.Length > 0) {
            isActive = true;
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
            if (myCraftingProcesses[lastCheckId].UpdateCraftingProcess(efficiency, inventory)) {
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
    bool UpdateCraftingProcess (float efficiency, BuildingInventoryController inventory);

    (Item,int)[] GetInputItems();
    (Item,int)[] GetOutputItems();
}



/// <summary>
/// This is used by all the crafter buildings
/// </summary>
public class CraftingProcess : IProcess {
    
    public int[] inputItemIds = new int[] {1};
    public Item[] inputItems = new Item[0];
    public int[] inputItemAmounts = new int[] {2};

    public bool isCrafting = false;
    public float curCraftingProgress = 0;
    public float craftingProgressTickReq = 20;

    public int[] outputItemIds = new int[] {1};
    public Item[] outputItems = new Item[0];
    public int[] outputItemAmounts = new int[] {2};

    List<BeltBuildingObject> myBelts;

    public CraftingProcess(List<BeltBuildingObject> belts, List<CountedItemNode> _inputs, List<CountedItemNode> _outputs, float timeReq) {
        myBelts = belts;

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


        craftingProgressTickReq = (timeReq * BuildingMaster.buildingUpdatePerSecond);
    }
    

    public bool UpdateCraftingProcess(float efficiency, BuildingInventoryController inventory) {
        if (!isCrafting) {
            for (int i = 0; i < outputItems.Length; i++) {
                if (!inventory.CheckAddItem(outputItems[i], outputItemAmounts[i])) {
                    return false;
                }
            }

            for (int i = 0; i < inputItems.Length; i++) {
                if (!inventory.CheckTakeItem(inputItems[i],inputItemAmounts[i])) {
                    return false;
                }
            }

            for (int i = 0; i < inputItems.Length; i++) {
                inventory.TryTakeItem(inputItems[i], inputItemAmounts[i]);
            }

            isCrafting = true;
        }

        if (isCrafting) {
            curCraftingProgress += efficiency;

            if (curCraftingProgress >= craftingProgressTickReq) {
                for (int i = 0; i < outputItems.Length; i++) {
                    inventory.TryAddItem(outputItems[i], outputItemAmounts[i]);
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
