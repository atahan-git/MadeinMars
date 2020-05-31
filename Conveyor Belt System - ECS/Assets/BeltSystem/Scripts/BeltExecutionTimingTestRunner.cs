using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeltExecutionTimingTestRunner : BeltMaster {

    public List<string> allResults = new List<string>();

    int prePassTrialCount = 10;
    int updateSlotTrialCount = 100;
    int updateGfxTrialCount = 100;
    public Text[] textses;
    // Start is called before the first frame update
    void Start () {
        s = this;

        allBelts = new List<BeltObject>(FindObjectsOfType<BeltObject>());

        allCreators = new List<MagicItemCreator>(FindObjectsOfType<MagicItemCreator>());
        allDestroyers = new List<MagicItemDestroyer>(FindObjectsOfType<MagicItemDestroyer>());

        foreach (BeltObject belt in allBelts) {
            belt.SetPosBasedOnWorlPos();
            belt.GetComponent<BeltGfx>().UpdateGraphics(belt.beltInputs, belt.beltOutputs);
            allBeltsCoords[belt.pos] = belt;
        }

        beltPreProc = new BeltPreProcessor(beltGroups, allBeltItems, GetBeltAtLocation);
        //beltItemSlotProc = new BeltItemSlotUpdateProcessor(allBeltItems, beltGroups);
        //beltItemGfxProc = new BeltItemGfxUpdateProcessor(allBeltItems);

        print("Belt Count = " + allBelts.Count.ToString());
        allResults.Add("Belt Count = " + allBelts.Count.ToString());

        allResults.Add("Trial Counts: " + "Prepass - " + prePassTrialCount.ToString() + ", Update Slot - " + updateSlotTrialCount.ToString() + ", Update Gfx - " + updateGfxTrialCount.ToString());



        var temp = Time.realtimeSinceStartup;

        TestStartupTime();
        TestBeltSlotUpdateTime();
        //TestBeltSlotGfxUpdateTime();

        temp = Time.realtimeSinceStartup - temp;
        print("Total test time: " + (temp).ToString("f6"));
        allResults.Add("Total test time: " + (temp).ToString("f6"));

        int n = 0;
        foreach (string s in allResults) {
            print(s);
            if (n < textses.Length) {
                textses[n].text = s;
            }
            n++;
        }

        TimingTestRunSaver.WriteTestRunDataToFile(allResults);
    }



    public void TestStartupTime () {
        var total = 0f;
        for (int i = 0; i < prePassTrialCount; i++) {
            var temp = Time.realtimeSinceStartup;
            beltPreProc.PrepassBelts(allBelts);
            temp = Time.realtimeSinceStartup - temp;
            total += temp;

            //yield return null;
        }
        print("Prepass belts average: " + (total / prePassTrialCount).ToString("f6"));
        allResults.Add("Prepass belts average: " + (total / prePassTrialCount).ToString("f6"));
    }


    public void TestBeltSlotUpdateTime () {
        var totalMagic = 0f;
        var total = 0f;
        for (int i = 0; i < updateSlotTrialCount; i++) {
            var temp = Time.realtimeSinceStartup;
            foreach (MagicItemCreator creator in allCreators) {
                creator.CreateItemsBasedOnTick();
            }
            foreach (MagicItemDestroyer destroyer in allDestroyers) {
                destroyer.DestroyItemsOnSlots();
            }
            temp = Time.realtimeSinceStartup - temp;
            totalMagic += temp;

            temp = Time.realtimeSinceStartup;
            beltItemSlotProc.UpdateBeltItemSlots();
            temp = Time.realtimeSinceStartup - temp;
            total += temp;

            //yield return null;
        }
        print("Magic update average: " + (totalMagic / updateSlotTrialCount).ToString("f6"));
        allResults.Add("Magic update average: " + (totalMagic / updateSlotTrialCount).ToString("f6"));
        print("Belt slot update average: " + (total / updateSlotTrialCount).ToString("f6"));
        allResults.Add("Belt slot update average: " + (total / updateSlotTrialCount).ToString("f6"));
    }

    /*public void TestBeltSlotGfxUpdateTime () {
        var total = 0f;
        for (int i = 0; i < updateGfxTrialCount; i++) {
            var temp = Time.realtimeSinceStartup;
            beltItemGfxProc.UpdateBeltItemGfxs(beltUpdatePerSecond, Time.deltaTime);
            temp = Time.realtimeSinceStartup - temp;
            total += temp;

            //yield return null;
        }
        print("Belt slot Gfx average: " + (total / updateGfxTrialCount).ToString("f6"));
        allResults.Add("Belt slot Gfx average: " + (total / updateGfxTrialCount).ToString("f6"));
    }*/ 
}
