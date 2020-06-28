using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingInfoDisplay : MonoBehaviour
{
    BuildingCraftingController crafter;

    public Transform Parent;

    public List<GameObject> craftingProcesses = new List<GameObject>();

    public GameObject ProcessDisplayPrefab;
    public GameObject ProcessinoutPrefab;

    // Update is called once per frame
    void Update()
    {
        if (craftingProcesses.Count > 0) {
            UpdateValues();
        }
        //progressBar.value = (float)crafter.curCraftingProgress / (float)crafter.craftingProgressTickReq;
    }

    public void SetUp () {
        crafter = GetComponent<BuildingCraftingController>();
        craftingProcesses = new List<GameObject>();
        for (int i = 0; i < crafter.myCraftingProcesses.Length; i++) {
            GameObject pDisp = Instantiate(ProcessDisplayPrefab, Parent);
            craftingProcesses.Add(pDisp);
            for (int input = 0; input < crafter.myCraftingProcesses[i].inputItemIds.Length; input++) {
                GameObject pIn = Instantiate(ProcessinoutPrefab, pDisp.transform.GetChild(0));
                pIn.GetComponentInChildren<RawImage>().material = 
                    DataHolder.s.GetItem(crafter.myCraftingProcesses[i].inputItemIds[input]).GetMaterial();
                pIn.transform.GetChild(1).GetComponent<Text>().text = 
                    crafter.myCraftingProcesses[i].inputItemRequirements[input].ToString();
            }

            for (int output = 0; output < crafter.myCraftingProcesses[i].outputItemIds.Length; output++) {
                GameObject pOut = Instantiate(ProcessinoutPrefab, pDisp.transform.GetChild(1));
                pOut.GetComponentInChildren<RawImage>().material = 
                    DataHolder.s.GetItem(crafter.myCraftingProcesses[i].outputItemIds[output]).GetMaterial();
                pOut.transform.GetChild(1).GetComponent<Text>().text = 
                    crafter.myCraftingProcesses[i].outputItemAmounts[output].ToString();
            }
        }
    }

    public void UpdateValues () {
        if (crafter != null) {
            for (int i = 0; i < crafter.myCraftingProcesses.Length; i++) {
                craftingProcesses[i].GetComponentInChildren<Slider>().value = (float)crafter.myCraftingProcesses[i].curCraftingProgress / (float)crafter.myCraftingProcesses[i].craftingProgressTickReq;
                for (int input = 0; input < crafter.myCraftingProcesses[i].inputItemIds.Length; input++) {
                    // Manually set madness
                    craftingProcesses[i].transform.GetChild(0).GetChild(input).GetChild(0).GetComponent<Text>().text =
                        crafter.myCraftingProcesses[i].inputItemCounts[input].ToString();
                }

                for (int output = 0; output < crafter.myCraftingProcesses[i].outputItemIds.Length; output++) {
                    // Manually set madness
                    craftingProcesses[i].transform.GetChild(1).GetChild(output).GetChild(0).GetComponent<Text>().text =
                        crafter.myCraftingProcesses[i].outputItemCounts[output].ToString();
                }
            }
        }
    }
}
