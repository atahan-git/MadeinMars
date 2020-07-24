﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingInfoDisplay : MonoBehaviour {
    public static bool isExtraInfoVisible = true; //Controlled by gui inventory controller

    BuildingCraftingController crafter;

    public GameObject canvas;
    public Transform Parent;

    public List<GameObject> craftingProcesses = new List<GameObject>();
    public List<float> timeoutCounters = new List<float>();

    public GameObject ProcessDisplayPrefab;
    public GameObject ProcessinoutPrefab;


    static float infoDisplayTimeoutTime = 5f;

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
                timeoutCounters.Add(0);
                for (int input = 0; input < curProcess.inputItemIds.Length; input++) {
                    GameObject pIn = Instantiate(ProcessinoutPrefab, pDisp.transform.GetChild(0));
                    pIn.GetComponentInChildren<RawImage>().material =
                        DataHolder.s.GetItem(curProcess.inputItemIds[input]).GetMaterial();
                    pIn.transform.GetChild(1).GetComponent<Text>().text =
                        curProcess.inputItemRequirements[input].ToString();
                }

                for (int output = 0; output < curProcess.outputItemIds.Length; output++) {
                    GameObject pOut = Instantiate(ProcessinoutPrefab, pDisp.transform.GetChild(1));
                    pOut.GetComponentInChildren<RawImage>().material =
                        DataHolder.s.GetItem(curProcess.outputItemIds[output]).GetMaterial();
                    pOut.transform.GetChild(1).GetComponent<Text>().text =
                        curProcess.outputItemAmounts[output].ToString();
                }
                craftingProcesses[craftingProcesses.Count-1].SetActive(false);
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
                        craftingProcesses[i].transform.GetChild(0).GetChild(input).GetChild(0).GetComponent<Text>().text =
                            curProcess.inputItemCounts[input].ToString();
                        totalItemCount += curProcess.inputItemCounts[input];
                    }

                    for (int output = 0; output < curProcess.outputItemIds.Length; output++) {
                        // Manually set madness
                        craftingProcesses[i].transform.GetChild(1).GetChild(output).GetChild(0).GetComponent<Text>().text =
                            curProcess.outputItemCounts[output].ToString();
                        totalItemCount += curProcess.outputItemCounts[output];
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