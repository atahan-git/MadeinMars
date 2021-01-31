using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicItemDestroyer : BeltObject
{
    // Update is called once per frame
    void Update()
    {
        DestroyItemsOnSlots();
    }

    public void DestroyItemsOnSlots () {
        foreach (BeltItemSlot slot in allBeltItemSlots) {
            if (slot != null) {
                BeltMaster.DestroyItem(slot.myItem);
            }
        }
    }
}
