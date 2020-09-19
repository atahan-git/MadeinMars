using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class BeltItemSlotUpdateProcessor
{
	public ObjectPoolSimple<BeltItem> beltItemPool;
	public List<BeltPreProcessor.BeltGroup> beltGroups = new List<BeltPreProcessor.BeltGroup>();

	public BeltItemSlotUpdateProcessor (ObjectPoolSimple<BeltItem> _beltItemPool, List<BeltPreProcessor.BeltGroup> _beltGroups) {
		beltItemPool = _beltItemPool;
		beltGroups = _beltGroups;
	}

	long updateOffset = 0;
	public void UpdateBeltItemSlots () {
		for (int i = 0; i< beltItemPool.objectPool.Length; i++) {
			beltItemPool.objectPool[i].isProcessedThisLoop = false;
			beltItemPool.objectPool[i].isMarkedThisLoop = false;
			beltItemPool.objectPool[i].isMovedThisLoop = false;
		}

		for (int k = 0; k< beltGroups.Count; k++) {
			BeltPreProcessor.BeltGroup beltGroup = beltGroups[k];
			for(int m = 0; m < beltGroup.beltItemSlotGroups.Count; m++) {
				List<BeltItemSlot> beltItemSlotSection = beltGroup.beltItemSlotGroups[m];
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
