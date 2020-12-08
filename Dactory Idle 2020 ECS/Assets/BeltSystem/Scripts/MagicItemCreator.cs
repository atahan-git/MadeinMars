using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Continuously creates items for debug purposes.
/// </summary>
public class MagicItemCreator : BeltObject {


    public bool isActive = true;

    public int tickCount = 4;
    int curTick = 0;

    public int itemIdToCreate = 0;

    public void CreateItemsBasedOnTick() {

        if (curTick >= tickCount) {
            for (int i = 0; i < allBeltItemSlotsArray.Length; i++) {
                BeltItemSlot slot = allBeltItemSlotsArray[i];
                if (slot != null) {
                    if (slot.myItem == null)
                        BeltMaster.s.CreateItemAtBeltSlot(slot, itemIdToCreate);
                }
            }

            curTick = 0;
        }

        curTick++;
    }
}
