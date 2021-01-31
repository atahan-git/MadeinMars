using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicItemCreator : BeltObject {

    public GameObject itemPrefab;

    public bool isActive = true;

    public float delay = 0.5f;
    public int tickCount = 4;
    int curTick = 0;


    // Start is called before the first frame update
    void Start () {
            StartCoroutine(CreateItems());
        
    }

    // Update is called once per frame
    IEnumerator CreateItems () {
        yield return new WaitForSeconds(1f);
        while (true) {
            if (isActive) {
                for(int i = 0; i < allBeltItemSlotsArray.Length; i++) {
                    BeltItemSlot slot = allBeltItemSlotsArray[i];
                    if (slot != null) {
                        if(slot.myItem == null)
                            BeltMaster.CreateItemAtBeltSlot(Instantiate(itemPrefab).GetComponent<BeltItem>(), slot);
                    }
                }
            }
            yield return new WaitForSeconds(delay);
        }
    }

    bool isFirstRun = true;
    public void CreateItemsBasedOnTick () {
        if (isFirstRun) {
            isFirstRun = false;
            StopAllCoroutines();
        }

        if (curTick >= tickCount) {
                for (int i = 0; i < allBeltItemSlotsArray.Length; i++) {
                    BeltItemSlot slot = allBeltItemSlotsArray[i]; 
                    if (slot != null) {
                        if (slot.myItem == null)
                            BeltMaster.CreateItemAtBeltSlot(Instantiate(itemPrefab).GetComponent<BeltItem>(), slot);
                }
            }
            curTick = 0;
        }

        curTick++;
    }
}
