using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicItemCreator : BeltObject {

    public GameObject itemPrefab;

    public bool isActive = true;

    public float delay = 0.5f;
    public int tickCount = 4;
    int curTick = 0;

    public bool isTimed = false;

    // Start is called before the first frame update
    void Start () {
        if (isTimed) {
            StartCoroutine(CreateItems());
        }
    }

    // Update is called once per frame
    IEnumerator CreateItems () {
        yield return new WaitForSeconds(1f);
        while (true) {
            if (isActive) {
                foreach (BeltItemSlot slot in myBeltItemSlots) {
                    if (slot != null) {
                        BeltMaster.CreateItemAtBeltSlot(Instantiate(itemPrefab).GetComponent<BeltItem>(), slot);
                    }
                }
            }
            yield return new WaitForSeconds(delay);
        }
    }

    public void CreateItemsBasedOnTick () {
        if (curTick >= tickCount) {
            foreach (BeltItemSlot slot in myBeltItemSlots) {
                if (slot != null) {
                    BeltMaster.CreateItemAtBeltSlot(Instantiate(itemPrefab).GetComponent<BeltItem>(), slot);
                }
            }
            curTick = 0;
        }

        curTick++;
    }
}
