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

    public GameObject canvas;
    public Transform Parent;

    public List<GameObject> craftingProcesses = new List<GameObject>();
    public List<float> timeoutCounters = new List<float>();

    public GameObject ProcessDisplayPrefab;
    public GameObject ProcessinoutPrefab;
    
    static float infoDisplayTimeoutTime = 5f;

    private void Start() {
        SetUp();
    }

    // Update is called once per frame
    void Update () {
        if (isExtraInfoVisible) {
            if (craftingProcesses.Count > 0) {
                UpdateValues();
            }
        }
        canvas.SetActive(isExtraInfoVisible);
        //progressBar.value = (float)crafter.curCraftingProgress / (float)crafter.craftingProgressTickReq;
    }

    public void SetUp () {
        crafter = GetComponent<BuildingCraftingController>();
        craftingProcesses = new List<GameObject>();
        timeoutCounters = new List<float>();
        for (int i = 0; i < crafter.myCraftingProcesses.Length; i++) {
            CraftingProcess curProcess = crafter.myCraftingProcesses[i] as CraftingProcess;
            if (curProcess != null) {
                GameObject pDisp = Instantiate(ProcessDisplayPrefab, Parent);
                craftingProcesses.Add(pDisp);
                timeoutCounters.Add(infoDisplayTimeoutTime-1f);
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
            } else {
                MiningProcess miningProcess = crafter.myCraftingProcesses[i] as MiningProcess;
                if (miningProcess != null) {
                    GameObject pDisp = Instantiate(ProcessDisplayPrefab, Parent);
                    craftingProcesses.Add(pDisp);
                    timeoutCounters.Add(infoDisplayTimeoutTime-1f);

                    /*GameObject pIn = Instantiate(ProcessinoutPrefab, pDisp.transform.GetChild(0));
                    pIn.GetComponentInChildren<Image>().sprite = DataHolder.s.GetItem(curProcess.outputItemIds[0]).GetSprite();
                    pIn.transform.GetChild(1).GetComponent<Text>().text = curProcess.outputItemAmounts[0].ToString();*/


                    for (int output = 0; output < miningProcess.outputItemIds.Length; output++) {
                        GameObject pOut = Instantiate(ProcessinoutPrefab, pDisp.GetComponent<MiniGUI_CraftingProcess>().OutputsParent);
                        pOut.GetComponent<MiniGUI_InOutDisplay>().itemImage.sprite = DataHolder.s.GetItem(miningProcess.outputItemIds[output]).GetSprite();
                        pOut.GetComponent<MiniGUI_InOutDisplay>().totalText.text = miningProcess.outputItemAmounts[output].ToString();
                    }

                    //craftingProcesses[craftingProcesses.Count - 1].SetActive(false);
                }
            }
        }
    }

    public void UpdateValues () {
        if (crafter != null) {
            for (int i = 0; i < crafter.myCraftingProcesses.Length; i++) {
                CraftingProcess curProcess = crafter.myCraftingProcesses[i] as CraftingProcess;
                if (curProcess != null) {
                    craftingProcesses[i].GetComponentInChildren<Slider>().value = (float)curProcess.curCraftingProgress / (float)curProcess.craftingProgressTickReq;
                    int totalItemCount = 0;
                    for (int input = 0; input < curProcess.inputItemIds.Length; input++) {
                        // Manually set madness
                        craftingProcesses[i].GetComponent<MiniGUI_CraftingProcess>().InputsParent.GetChild(input).GetComponent<MiniGUI_InOutDisplay>().valueText.text = curProcess.toBeTakenFromBeltsAmounts[input].ToString();
                        totalItemCount += curProcess.toBeTakenFromBeltsAmounts[input];
                    }

                    for (int output = 0; output < curProcess.inputItemIds.Length; output++) {
                        // Manually set madness
                        craftingProcesses[i].GetComponent<MiniGUI_CraftingProcess>().OutputsParent.GetChild(output).GetComponent<MiniGUI_InOutDisplay>().valueText.text = curProcess.toBePutOnBeltsAmounts[output].ToString();
                        totalItemCount += curProcess.toBePutOnBeltsAmounts[output];
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
                } else {
                    MiningProcess miningProcess = crafter.myCraftingProcesses[i] as MiningProcess;
                    if (miningProcess != null) {
                        craftingProcesses[i].GetComponentInChildren<Slider>().value = (float)miningProcess.curCraftingProgress / (float)miningProcess.craftingProgressTickReq;
                        int totalItemCount = 0;

                        for (int output = 0; output < miningProcess.outputItemIds.Length; output++) {
                            // Manually set madness
                            craftingProcesses[i].GetComponent<MiniGUI_CraftingProcess>().OutputsParent.GetChild(output).GetComponent<MiniGUI_InOutDisplay>().valueText.text = miningProcess.toBePutOnBeltsAmounts[output].ToString();
                            totalItemCount += miningProcess.toBePutOnBeltsAmounts[output];
                        }

                        if (miningProcess.isCrafting)
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
}