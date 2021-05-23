using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Displays the "details" button building information
/// </summary>
public class BuildingInfoDisplay : MonoBehaviour {
    public static bool isExtraInfoVisible = false; //Controlled by gui inventory controller

    BuildingCraftingController crafter;
    BuildingInventoryController inventory;

    public GameObject canvas;
    public Transform Parent;

    public List<GameObject> craftingProcesses = new List<GameObject>();
    public List<float> timeoutCounters = new List<float>();

    public GameObject ProcessDisplayPrefab;
    public GameObject ProcessinoutPrefab;

    public GameObject InventoryListingPrefab;
    public Transform InventoryParent;
    
    static float infoDisplayTimeoutTime = 5f;

    private void Start() {
        var worldObject = GetComponent<BuildingWorldObject>();
        if (worldObject.isInventorySetup) {
            SetUp();
        } else {
            worldObject.buildingInventoryUpdatedCallback += SetUp;
        }
    }


    // Update is called once per frame
    void Update() {
        if (isExtraInfoVisible) {
            if (craftingProcesses.Count > 0) {
                UpdateValues();
            }
        }

        canvas.SetActive(isExtraInfoVisible);
    }

    public void SetUp () {
        int childs = InventoryParent.childCount;
        for (int i = childs - 1; i >= 0; i--) {
            Destroy(InventoryParent.GetChild(i).gameObject);
        }
        
        childs = Parent.childCount;
        for (int i = childs - 1; i > 0; i--) {
            Destroy(Parent.GetChild(i).gameObject);
        }
        
        var worldObject = GetComponent<BuildingWorldObject>();
        crafter = worldObject.myCrafter;
        
        inventory = worldObject.myInventory;
        inventory.drawInventoryEvent += SetUp;

        craftingProcesses.Clear();
        timeoutCounters.Clear();
        if (crafter != null) {
            for (int i = 0; i < crafter.myCraftingProcesses.Length; i++) {
                CraftingProcess curProcess = crafter.myCraftingProcesses[i] as CraftingProcess;
                if (curProcess != null) {
                    GameObject pDisp = Instantiate(ProcessDisplayPrefab, Parent);
                    craftingProcesses.Add(pDisp);
                    timeoutCounters.Add(infoDisplayTimeoutTime - 1f);
                    for (int input = 0; input < curProcess.inputItemIds.Length; input++) {
                        GameObject pIn = Instantiate(ProcessinoutPrefab, pDisp.GetComponent<MiniGUI_CraftingProcess>().InputsParent);
                        pIn.GetComponent<MiniGUI_InOutDisplay>().itemImage.sprite = DataHolder.s.GetItem(curProcess.inputItemIds[input]).GetSprite();
                        pIn.GetComponent<MiniGUI_InOutDisplay>().totalText.text = curProcess.inputItemAmounts[input].ToString();
                    }

                    for (int output = 0; output < curProcess.outputItemIds.Length; output++) {
                        GameObject pOut = Instantiate(ProcessinoutPrefab, pDisp.GetComponent<MiniGUI_CraftingProcess>().OutputsParent);
                        pOut.GetComponent<MiniGUI_InOutDisplay>().itemImage.sprite = DataHolder.s.GetItem(curProcess.outputItemIds[output]).GetSprite();
                        pOut.GetComponent<MiniGUI_InOutDisplay>().totalText.text = curProcess.outputItemAmounts[output].ToString();
                    }

                    //craftingProcesses[craftingProcesses.Count-1].SetActive(false);
                }
            }
        }


        foreach (InventoryItemSlot it in inventory.inventory) {
            Instantiate(InventoryListingPrefab, InventoryParent).GetComponent<MiniGUI_InventoryListing>().SetUp(it, inventory,true);
        }
    }

    public void UpdateValues () {
        if (crafter != null) {
            for (int i = 0; i < crafter.myCraftingProcesses.Length; i++) {
                CraftingProcess curProcess = crafter.myCraftingProcesses[i] as CraftingProcess;
                if (curProcess != null) {
                    craftingProcesses[i].GetComponentInChildren<Slider>().value = (float)curProcess.curCraftingProgress / (float)curProcess.craftingProgressTickReq;
                    int totalItemCount = 0;
                    for (int input = 0; input < curProcess.inputItems.Length; input++) {
                        // Manually set madness
                        craftingProcesses[i].GetComponent<MiniGUI_CraftingProcess>().InputsParent.GetChild(input).GetComponent<MiniGUI_InOutDisplay>().valueText.text 
                            = inventory.GetAmountOfItems(curProcess.inputItems[input]).ToString();
                        totalItemCount += inventory.GetAmountOfItems(curProcess.inputItems[input]);
                    }

                    for (int output = 0; output < curProcess.outputItems.Length; output++) {
                        // Manually set madness
                        craftingProcesses[i].GetComponent<MiniGUI_CraftingProcess>().OutputsParent.GetChild(output).GetComponent<MiniGUI_InOutDisplay>().valueText.text 
                            = inventory.GetAmountOfItems(curProcess.outputItems[output]).ToString();
                        totalItemCount += inventory.GetAmountOfItems(curProcess.outputItems[output]);
                    }

                    if (curProcess.isCrafting)
                        totalItemCount += 1;

                    // Disable the crafting process display if there are no items
                    if (totalItemCount > 0) {
                        timeoutCounters[i] = -1;
                        craftingProcesses[i].SetActive(true);
                    } else {
                        if (timeoutCounters[i] == -1)
                            timeoutCounters[i] = 0;
                        else if (timeoutCounters[i] < infoDisplayTimeoutTime)
                            timeoutCounters[i] += Time.deltaTime;
                        else
                            craftingProcesses[i].SetActive(false);

                    }
                }
            }
        }
    }
}