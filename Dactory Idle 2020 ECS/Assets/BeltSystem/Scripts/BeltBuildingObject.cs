using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Spawn these under a building to act as a belt input & output point
/// These belts don't have center segments, and provide helpful methods to put/collect items from correct places
/// </summary>
public class BeltBuildingObject : BeltObject {

	public List<ItemCreationSlot> myCreationSlots = new List<ItemCreationSlot>();
	public List<ItemInputSlot> myInputSlots = new List<ItemInputSlot>();

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

		CreateItemCreationSlots();
		CreateItemInputSlots();
	}

	public void Start () {
		InvokeRepeating("CreateItemsPeriodically", 1f, 1f);
	}

	public void CreateItemsPeriodically () {
		for (int i = 0; i < myCreationSlots.Count; i++) {
			ItemCreationSlot slot = myCreationSlots[i];
			if (slot != null) {
				slot.CreateItem();
			}
		}

		for (int i = 0; i < myInputSlots.Count; i++) {
			ItemInputSlot slot = myInputSlots[i];
			if (slot != null) {
				slot.TakeItem();
			}
		}
	}

	void CreateItemCreationSlots () {
		myCreationSlots.Clear();

		for (int i = 0; i < 4; i++) {
			if (beltOutputs[i]) {
				switch (i) {
				case 0:
					myCreationSlots.Add(new ItemCreationSlot(myBeltItemSlots[1, 0]));
					myCreationSlots.Add(new ItemCreationSlot(myBeltItemSlots[2, 0]));
					break;
				case 1:
					myCreationSlots.Add(new ItemCreationSlot(myBeltItemSlots[3, 1]));
					myCreationSlots.Add(new ItemCreationSlot(myBeltItemSlots[3, 2]));
					break;
				case 2:
					myCreationSlots.Add(new ItemCreationSlot(myBeltItemSlots[1, 3]));
					myCreationSlots.Add(new ItemCreationSlot(myBeltItemSlots[2, 3]));
					break;
				case 3:
					myCreationSlots.Add(new ItemCreationSlot(myBeltItemSlots[0, 1]));
					myCreationSlots.Add(new ItemCreationSlot(myBeltItemSlots[0, 2]));
					break;
				}
			}
		}
	}

	void CreateItemInputSlots () {
		myInputSlots.Clear();

		for (int i = 0; i < 4; i++) {
			if (beltInputs[i]) {
				switch (i) {
				case 0:
					myInputSlots.Add(new ItemInputSlot(myBeltItemSlots[1, 0]));
					myInputSlots.Add(new ItemInputSlot(myBeltItemSlots[2, 0]));
					break;
				case 1:
					myInputSlots.Add(new ItemInputSlot(myBeltItemSlots[3, 1]));
					myInputSlots.Add(new ItemInputSlot(myBeltItemSlots[3, 2]));
					break;
				case 2:
					myInputSlots.Add(new ItemInputSlot(myBeltItemSlots[1, 3]));
					myInputSlots.Add(new ItemInputSlot(myBeltItemSlots[2, 3]));
					break;
				case 3:
					myInputSlots.Add(new ItemInputSlot(myBeltItemSlots[0, 1]));
					myInputSlots.Add(new ItemInputSlot(myBeltItemSlots[0, 2]));
					break;
				}
			}
		}
	}
}

public class ItemInputSlot {
	private BeltItemSlot mySlot;

	public ItemInputSlot (BeltItemSlot slot) {
		mySlot = slot;
	}

	public bool TakeItem () {
		return BeltMaster.s.DestroyItemAtSlot(mySlot);
	}
}

public class ItemCreationSlot {
	private BeltItemSlot mySlot;

	public ItemCreationSlot (BeltItemSlot slot) {
		mySlot = slot;
	}

	public void CreateItem () {
		BeltMaster.s.CreateItemAtBeltSlot(mySlot);
	}
}
