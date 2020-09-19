using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GUI_CraftingController : MonoBehaviour {
    private List<CraftingProcessNode> allCraftingProcesses = new List<CraftingProcessNode>();

    public Transform CraftingDisplayParent;
    public GameObject CraftingDisplayPrefab;

    public Transform CraftingQueueParent;
    public GameObject CraftingQueuePrefab;
    Queue<MiniGUI_CraftingQueueDisplay> CraftingQueue = new Queue<MiniGUI_CraftingQueueDisplay>();
    
    void Start() {
        var dividedCraftingProcesses = DataHolder.s.GetAllCraftingProcessNodesDivided();
        for (int i = 1; i <= 6; i++) {
            allCraftingProcesses.AddRange(dividedCraftingProcesses[i]);
        }

        for (int i = 1; i < allCraftingProcesses.Count; i++) {
            var disp = Instantiate(CraftingDisplayPrefab, CraftingDisplayParent);
            disp.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = DataHolder.s.GetItem(allCraftingProcesses[i].inputItemUniqueNames[0]).GetSprite();
            disp.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = allCraftingProcesses[i].inputItemCounts[0].ToString();
            
            if (allCraftingProcesses[i].inputItemUniqueNames.Count > 1 && allCraftingProcesses[i].inputItemUniqueNames[1] != "Empty") {
                disp.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = DataHolder.s.GetItem(allCraftingProcesses[i].inputItemUniqueNames[1]).GetSprite();
                disp.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = allCraftingProcesses[i].inputItemCounts[1].ToString();
            } else {
                disp.transform.GetChild(0).GetChild(1).GetComponent<Image>().enabled = false;
                disp.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "";
            }

            disp.transform.GetChild(3).GetComponent<Text>().text = allCraftingProcesses[i].timeCost.ToString() + " s";
            
            disp.transform.GetChild(5).GetChild(0).GetComponent<Image>().sprite = DataHolder.s.GetItem(allCraftingProcesses[i].outputItemUniqueNames[0]).GetSprite();
            disp.transform.GetChild(6).GetChild(0).GetComponent<Text>().text = allCraftingProcesses[i].outputItemCounts[0].ToString();
            
            int x = new int();
            x = i;
            disp.transform.GetChild(7).GetComponent<Button>().onClick.AddListener(delegate { CraftItem(x); });
        }
    }

    public void CraftItem(int craftingProcessIndex) {
        print("Crafting item " + craftingProcessIndex.ToString());
        
        var newCraftingProcess = Instantiate(CraftingQueuePrefab, CraftingQueueParent).GetComponent<MiniGUI_CraftingQueueDisplay>();
        newCraftingProcess.transform.SetAsFirstSibling();
        
        newCraftingProcess.SetUp(allCraftingProcesses[craftingProcessIndex],this);
        CraftingQueue.Enqueue(newCraftingProcess);
    }

    public void CancelCraftItem(MiniGUI_CraftingQueueDisplay target) {
        if (target == curActiveProcess) {
            curActiveProcess.DestroySelf();
            curActiveProcess = null;
        } else {
            CraftingQueue = new Queue<MiniGUI_CraftingQueueDisplay>(CraftingQueue.Where(x => x != target));
            target.DestroySelf();
        }
    }

    private MiniGUI_CraftingQueueDisplay curActiveProcess;
    private void Update() {
        if (curActiveProcess == null) {
            if (CraftingQueue.Count > 0) {
                curActiveProcess = CraftingQueue.Dequeue();
            }
        } else {
            curActiveProcess.progress += (1f / curActiveProcess.timeReq) * Time.deltaTime;
            curActiveProcess.UpdateDisplay();
            if (curActiveProcess.progress >= 1) {
                Player_InventoryController.s.TryAddItem(curActiveProcess.myItem);
                curActiveProcess.DestroySelf();
                curActiveProcess = null;
            }
        }
    }

}
