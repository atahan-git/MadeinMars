using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Spawn these under a building to act as a belt input & output point
/// These belts don't have center segments, and provide helpful methods to put/collect items from correct places
/// </summary>
public class BeltBuildingObject : BeltObject {

	public delegate bool CreateItemAtOpenSlot (int i);

	public List<CreateItemAtOpenSlot> myPossibleItemCreators;

	public override void CreateBeltItemSlots () {
		myBeltItemSlots = new BeltItemSlot[4, 4];
		myBeltItemSlotsLayer2 = new BeltItemSlot[4, 4];
		allBeltItemSlots.Clear();

		// only put belt item slots on the edges
		for (int x = 0; x < 4; x++) {
			for (int y = 0; y < 4; y++) {
				if (x + y != 3) {
					if (math.abs(x - y) == 1 || math.abs(x - y) == 2) {
						myBeltItemSlots[x, y] = new BeltItemSlot(GetBeltItemSlotPos(x, y));
					}
				}
			}
		}

		allBeltItemSlotsArray = allBeltItemSlots.ToArray();
	}


	void CreateItemCreationSlots () {

	}

	void CreateItemInputSlots () {

	}
}
