using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltItemSlotUpdateProcessor
{
	public List<BeltItem> allBeltItems = new List<BeltItem>();
	public List<BeltPreProcessor.BeltGroup> beltGroups = new List<BeltPreProcessor.BeltGroup>();

	public BeltItemSlotUpdateProcessor (List<BeltItem> _allBeltItems, List<BeltPreProcessor.BeltGroup> _beltGroups) {
		allBeltItems = _allBeltItems;
		beltGroups = _beltGroups;
	}

	long updateOffset = 0;
	public void UpdateBeltItemSlots () {
		foreach (BeltItem item in allBeltItems) {
			item.isProcessedThisLoop = false;
			item.isMarkedThisLoop = false;
		}

		foreach (BeltPreProcessor.BeltGroup beltGroup in beltGroups) {
			foreach (List<BeltItemSlot> beltItemSlotSection in beltGroup.beltItemSlotGroups) {
				for (int i = 0; i < beltItemSlotSection.Count; i++) {
					beltItemSlotSection[i].BeltItemSlotUpdate(false);
				}
				for (int i = 0; i < beltItemSlotSection.Count; i++) {
					beltItemSlotSection[i].BeltItemSlotUpdate(true);
				}
			}
		}
		updateOffset++;
		//updateIndex = updateIndex % 12;
	}
}
