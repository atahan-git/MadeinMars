using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeltExecutionTimingTestRunner : BeltMaster {

    List<MagicItemCreator> allCreators = new List<MagicItemCreator>();
    List<MagicItemDestroyer> allDestroyers = new List<MagicItemDestroyer>();

    public List<string> allResults = new List<string>();

    int prePassTrialCount = 3;
    int updateSlotTrialCount = 300;
    int updateGfxTrialCount = 300;
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
        }

        beltProc = new BeltPreProcessor(allBelts, beltGroups, allBeltItemsSlots, beltItemSlotGroups, allBeltItems, GetBeltAtLocation);

        beltItemSlotProc = new BeltItemSlotUpdateProcessor(allBeltItems, beltItemSlotGroups);

        beltItemGfxProc = new BeltItemGfxUpdateProcessor(allBeltItems);

        print("Belt Count = " + allBelts.Count.ToString());
        allResults.Add("Belt Count = " + allBelts.Count.ToString());



        var temp = Time.realtimeSinceStartup;

        TestStartupTime();
        TestBeltSlotUpdateTime();
        TestBeltSlotGfxUpdateTime();

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

       
    }



    public void TestStartupTime () {
        var total = 0f;
        for (int i = 0; i < prePassTrialCount; i++) {
            var temp = Time.realtimeSinceStartup;
            beltProc.PrepassBelts();
            temp = Time.realtimeSinceStartup - temp;
            if (i % Mathf.Max((int)(prePassTrialCount) / 10,1) == 0)
                print("Prepass belts run " + i.ToString() + ": " + (temp).ToString("f6"));
            total += temp;

            //yield return null;
        }
        print("Prepass belts average: " + (total / prePassTrialCount).ToString("f6"));
        allResults.Add("Prepass belts average: " + (total / prePassTrialCount).ToString("f6"));
    }


    public void TestBeltSlotUpdateTime () {
        var total = 0f;
        for (int i = 0; i < updateSlotTrialCount; i++) {
            foreach (MagicItemCreator creator in allCreators) {
                creator.CreateItemsBasedOnTick();
            }
            foreach (MagicItemDestroyer destroyer in allDestroyers) {
                destroyer.DestroyItemsOnSlots();
            }

            var temp = Time.realtimeSinceStartup;
            beltItemSlotProc.UpdateBeltItemSlots();
            temp = Time.realtimeSinceStartup - temp;
            if (i % (int)(updateSlotTrialCount / 10) == 0)
                print("Belt slot update run " + i.ToString() + ": " + (temp).ToString("f6"));
            total += temp;

            //yield return null;
        }
        print("Belt slot update average: " + (total / updateSlotTrialCount).ToString("f6"));
        allResults.Add("Belt slot update average: " + (total / updateSlotTrialCount).ToString("f6"));
    }

    public void TestBeltSlotGfxUpdateTime () {
        var total = 0f;
        for (int i = 0; i < updateGfxTrialCount; i++) {
            var temp = Time.realtimeSinceStartup;
            beltItemGfxProc.UpdateBeltItemGfxs(beltUpdatePerSecond, Time.deltaTime);
            temp = Time.realtimeSinceStartup - temp;
            if(i%(int)(updateGfxTrialCount/10) == 0)
            print("Belt slot Gfx update run " + i.ToString() + ": " + (temp).ToString("f6"));
            total += temp;

            //yield return null;
        }
        print("Belt slot Gfx average: " + (total / updateGfxTrialCount).ToString("f6"));
        allResults.Add("Belt slot Gfx average: " + (total / updateGfxTrialCount).ToString("f6"));
    }
}
