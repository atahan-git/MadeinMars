using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltItemSlotUpdateProcessor
{
	public List<BeltItem> allBeltItems = new List<BeltItem>();
	public List<List<BeltItemSlot>> beltItemSlotGroups = new List<List<BeltItemSlot>>();

	public BeltItemSlotUpdateProcessor (List<BeltItem> _allBeltItems, List<List<BeltItemSlot>> _beltItemSlotGroups) {
		allBeltItems = _allBeltItems;
		beltItemSlotGroups = _beltItemSlotGroups;
	}

	long updateOffset = 0;
	public void UpdateBeltItemSlots () {
		foreach (BeltItem item in allBeltItems) {
			item.isProcessedThisLoop = false;
			item.isMarkedThisLoop = false;
		}

		foreach (List<BeltItemSlot> beltItemSection in beltItemSlotGroups) {
			for (int i = 0; i < beltItemSection.Count; i++) {
				beltItemSection[i].BeltItemSlotUpdate(false);
			}
			for (int i = 0; i < beltItemSection.Count; i++) {
				beltItemSection[i].BeltItemSlotUpdate(true);
			}
		}
		updateOffset++;
		//updateIndex = updateIndex % 12;
	}
}
