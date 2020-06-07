using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltPreProcessor {

	public List<BeltGroup> allBeltGroups = new List<BeltGroup>();

	[System.Serializable]
	public class BeltGroup {
		public List<BeltObject> belts = new List<BeltObject>();
		public List<List<BeltItemSlot>> beltItemSlotGroups = new List<List<BeltItemSlot>>();
	}

	public List<BeltItem> allBeltItems = new List<BeltItem>();

	public delegate T GetItemAtLocationDelegate<T>(Position pos);
	public GetItemAtLocationDelegate<BeltObject> GetBeltAtLocation;

	public BeltPreProcessor (List<BeltGroup> _beltGroups, List<BeltItem> _allBeltItems, GetItemAtLocationDelegate<BeltObject> _GetBeltAtLocation) {
		allBeltGroups = _beltGroups;
		allBeltItems = _allBeltItems;
		GetBeltAtLocation = _GetBeltAtLocation;
	}

	/* We will do a few important things here for the belt system to work
		1. Split up belts into sperate list based on if they are feeding into each other
		2. Make item lines, and put the 'lines' on lists
		3. go through those lines recursively to find their update order using A* like method
		4. sort these lists based on the found update order

	*/
	public void PrepassBelts (List<BeltObject> allBelts) {
		var temp = Time.realtimeSinceStartup;
		Debug.Log("Clear old things");
		allBeltGroups.Clear();
		foreach (BeltObject belt in allBelts) {
			belt.isProcessed = false;
			belt.CreateBeltItemSlots();
		}
		temp = Time.realtimeSinceStartup - temp;
		Debug.Log("belt item slot creation: " + (temp).ToString("f6"));

		var groupBeltTotal = 0f;
		var groupBeltCount = 0f;
		var processSlotsTotal = 0f;
		var processSlotsCount = 0f;
		Debug.Log("Creating Belt Groups");
		for (int i = 0; i < allBelts.Count; i++) {
			if (allBelts[i].isProcessed)
				continue;

			temp = Time.realtimeSinceStartup;
			BeltGroup newBeltGroup = new BeltGroup();
			//newBeltGroup.belts = new List<BeltObject>();
			//newBeltGroup.beltItemSlotGroups = new List<List<BeltItemSlot>>();
			allBeltGroups.Add(newBeltGroup);
			RecursiveGroupBelts(allBelts[i], newBeltGroup.belts);

			foreach (BeltObject belt in newBeltGroup.belts) {
				belt.beltGroup = newBeltGroup;
			}
			temp = Time.realtimeSinceStartup - temp;
			groupBeltTotal += temp;
			groupBeltCount++;

			temp = Time.realtimeSinceStartup;
			ProcessBeltGroupItemSlots(newBeltGroup);
			temp = Time.realtimeSinceStartup - temp;
			processSlotsTotal += temp;
			processSlotsCount++;
			//Debug.Log(newBeltGroup.beltItemSlotGroups.Count);
		}

		Debug.Log("Grouping belts total: " + (groupBeltTotal).ToString("f6"));
		Debug.Log("Grouping Slots total: " + (processSlotsTotal).ToString("f6"));
	}

	void ProcessBeltGroupItemSlots (BeltGroup group) {
		List<BeltItemSlot> slots = new List<BeltItemSlot>();
		foreach(BeltObject belt in group.belts) {
			slots.AddRange(belt.allBeltItemSlots);
		}

		group.beltItemSlotGroups = new List<List<BeltItemSlot>>();
		//Debug.Log("Splitting belts item slots into lists");
		for (int i = 0; i < slots.Count; i++) {
			if (slots[i].isProcessed)
				continue;
			List<BeltItemSlot> newBeltItemSlotGroup = new List<BeltItemSlot>();
			group.beltItemSlotGroups.Add(newBeltItemSlotGroup);
			RecursiveGroupBeltItemSlots(slots[i], newBeltItemSlotGroup, 0);
			foreach (BeltItemSlot beltItemSlot in newBeltItemSlotGroup) {
				beltItemSlot.beltItemSlotGroup = slots.Count - 1;
			}
		}

		int n = 0;
		//Debug.Log("Sorting belt item slots");
		foreach (List<BeltItemSlot> beltItemSlotGroup in group.beltItemSlotGroups) {
			beltItemSlotGroup.Sort((x, y) => x.index.CompareTo(y.index));
			beltItemSlotGroup.Reverse();

			//if (n == 2) {
#if UNITY_EDITOR
			if (BeltMaster.s != null) {
				if (BeltMaster.s.debugDraw) {
					for (int i = 0; i < beltItemSlotGroup.Count - 1; i++) {
						Debug.DrawLine(beltItemSlotGroup[i].position - Vector3.forward * i * 0.05f, beltItemSlotGroup[i + 1].position - Vector3.forward * (i + 1) * 0.05f, Color.magenta, 200f);
					}
				}
			}
#endif
			//}
			n++;
		}

		//Debug.Log(group.beltItemSlotGroups.Count);
	}

	public void UpdateOneBeltBeltSlots (BeltObject belt) {
		try {
			belt.RemoveOldItemSlots(belt.beltGroup.beltItemSlotGroups);
		} catch (ArgumentOutOfRangeException e) {
			belt.RemoveOldItemSlots(null);
		}
		belt.CreateBeltItemSlots();
	}

	public void ProcessBeltGroupingChange (BeltObject belt) {
		List<BeltGroup> myTouchingBeltGroups = GetNearbyBeltGroups(belt);

		// If there is touching belt group, create a new one with this in it
		if (myTouchingBeltGroups.Count == 0) {
			AddBeltToNewBeltGroup(belt);

		// If there is one or more belt groups we are touching, merge them all and add us to it
		} else  {
			AddBeltToExistingBeltGroups(belt, myTouchingBeltGroups);
		}

		belt.isProcessed = true;
	}

	void AddBeltToNewBeltGroup (BeltObject belt) {
		Debug.Log("Creating new belt group for " + belt.name);

		BeltGroup targetBeltGroup = new BeltGroup();
		targetBeltGroup.belts = new List<BeltObject>();
		allBeltGroups.Add(targetBeltGroup);

		targetBeltGroup.belts.Add(belt);

		belt.beltGroup = targetBeltGroup;

		ProcessBeltGroupItemSlots(targetBeltGroup);

		belt.isProcessed = true;
	}

	void AddBeltToExistingBeltGroups (BeltObject belt, List<BeltGroup> touchingBeltGroups) {
		Debug.Log("Processing belt grouping change for belt with " + touchingBeltGroups.Count + " nearby belt groups");

		BeltGroup targetBeltGroup = touchingBeltGroups[0];
		for (int i = 1; i < touchingBeltGroups.Count; i++) {
			targetBeltGroup.belts.AddRange(touchingBeltGroups[i].belts); // add all the other belt groups to the first one
		}

		for (int i = 1; i < touchingBeltGroups.Count; i++) {
			allBeltGroups.Remove(touchingBeltGroups[i]); // remove the old belt groups
		}

		if (belt.beltGroup != null)
			if (belt.beltGroup != targetBeltGroup)
				targetBeltGroup.belts.Add(belt); // add our new belt
		
		foreach (BeltObject _belt in targetBeltGroup.belts) {
			_belt.beltGroup = targetBeltGroup;
		}

		ProcessBeltGroupItemSlots(targetBeltGroup);
	}

	List<BeltGroup> GetNearbyBeltGroups (BeltObject belt) {
		List<BeltGroup> nearbyBeltGroups = new List<BeltGroup>();
		for (int i = 0; i < 4; i++) {
			if (belt.beltInputs[i] || belt.beltOutputs[i]) {
				BeltObject neighborBelt = GetBeltAtLocation(belt.pos + IndexToXY(i));
				if (BeltObject.CanConnectBelts(belt, neighborBelt)) {
					if (neighborBelt.beltGroup == null)
						AddBeltToNewBeltGroup(neighborBelt);
					nearbyBeltGroups.Add(neighborBelt.beltGroup);
				}
			}
		}

		return nearbyBeltGroups;
	}


	void RecursiveGroupBeltItemSlots (BeltItemSlot currentBeltItemSlot, List<BeltItemSlot> processedBeltItemSlots, int index) {
		if (currentBeltItemSlot.isProcessed)
			return;

		//Debug.Log(index);
		currentBeltItemSlot.isProcessed = true;
		currentBeltItemSlot.index = index;
		processedBeltItemSlots.Add(currentBeltItemSlot);


		foreach (BeltItemSlot outs in currentBeltItemSlot.outsideConnections) {
			//Debug.DrawLine(currentBeltItemSlot.position, outs.position, Color.green, 200f);
			RecursiveGroupBeltItemSlots(outs, processedBeltItemSlots, index+1);
		}
		foreach (BeltItemSlot ins in currentBeltItemSlot.insideConnections) {
			//Debug.DrawLine(currentBeltItemSlot.position, ins.position, Color.red, 200f);
			RecursiveGroupBeltItemSlots(ins, processedBeltItemSlots, index-1);
		}
	}

	void RecursiveGroupBelts (BeltObject currentBelt, List<BeltObject> processedBelts) {
		if (currentBelt.isProcessed)
			return;

		currentBelt.isProcessed = true;
		processedBelts.Add(currentBelt);


		for (int i = 0; i < 4; i++) {
			if (currentBelt.beltOutputs[i]) {
				// Get the next belt that out output is pointing at
				BeltObject nextBelt = GetBeltAtLocation(currentBelt.pos +IndexToXY(i));
				if (nextBelt != null) {
					// Check if they are getting input from our output, a requirement for us to be connected
					if (nextBelt.beltInputs[(i + 2) % 4]) {
						// Continue recursion with that next belt
						BeltObject.ConnectBelts(currentBelt, nextBelt, i);
						RecursiveGroupBelts(nextBelt, processedBelts);
					} else
						currentBelt.isEndPoint = true;
				} else
					currentBelt.isEndPoint = true;
			}
			if (currentBelt.beltInputs[i]) {
				// Get the next belt that out output is pointing at
				BeltObject nextBelt = GetBeltAtLocation(currentBelt.pos+ IndexToXY(i));
				if (nextBelt != null) {
					// Check if they are getting input from our output, a requirement for us to be connected
					if (nextBelt.beltOutputs[(i + 2) % 4]) {
						// Continue recursion with that next belt
						BeltObject.ConnectBelts(nextBelt, currentBelt, i + 2 % 4);
						RecursiveGroupBelts(nextBelt, processedBelts);
					} else
						currentBelt.isEndPoint = true;
				} else
					currentBelt.isEndPoint = true;
			}
		}
	}

	Position IndexToXY (int i) {
		Position pos = new Position();
		int off = 1;
		switch (i) {
		case 0:
			pos.y = off;
			break;
		case 1:
			pos.x = off;
			break;
		case 2:
			pos.y = -off;
			break;
		case 3:
			pos.x = -off;
			break;
		}
		return pos;
	}
}
