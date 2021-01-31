using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltPreProcessor {

	public List<BeltObject> allBelts = new List<BeltObject>();
	public List<List<BeltObject>> beltGroups = new List<List<BeltObject>>();

	public List<BeltItemSlot> allBeltItemsSlots = new List<BeltItemSlot>();
	public List<List<BeltItemSlot>> beltItemSlotGroups = new List<List<BeltItemSlot>>();


	public List<BeltItem> allBeltItems = new List<BeltItem>();

	public delegate T GetItemAtLocationDelegate<T>(int x, int y);
	public GetItemAtLocationDelegate<BeltObject> GetBeltAtLocation;

	public BeltPreProcessor (List<BeltObject> _allBelts, List<List<BeltObject>> _beltGroups, List<BeltItemSlot> _allBeltItemsSlots, List<List<BeltItemSlot>> _beltItemSlotGroups, List<BeltItem> _allBeltItems, GetItemAtLocationDelegate<BeltObject> _GetBeltAtLocation) {
		allBelts = _allBelts;
		beltGroups = _beltGroups;
		allBeltItemsSlots = _allBeltItemsSlots;
		beltItemSlotGroups = _beltItemSlotGroups;
		allBeltItems = _allBeltItems;
		GetBeltAtLocation = _GetBeltAtLocation;
	}

	/* We will do a few important things here for the belt system to work
		1. Split up belts into sperate list based on if they are feeding into each other
		2. Make item lines, and put the 'lines' on lists
		3. go through those lines recursively to find their update order using A* like method
		4. sort these lists based on the found update order

	*/
	public void PrepassBelts () {
		Debug.Log("Clear old things");
		beltGroups.Clear();
		allBeltItemsSlots.Clear();
		beltItemSlotGroups.Clear();


		Debug.Log("Creating belt item slots");
		foreach (BeltObject belt in allBelts) {
			belt.isProcessed = false;
			allBeltItemsSlots.AddRange(belt.CreateBeltItemSlots());
		}


		Debug.Log("Splitting belts into lists");
		List<BeltObject> cleanBelts = new List<BeltObject>(allBelts);
		while (cleanBelts.Count > 0) {
			List<BeltObject> newBeltGroup = new List<BeltObject>();
			beltGroups.Add(newBeltGroup);
			RecursiveGroupBelts(cleanBelts[0], cleanBelts, newBeltGroup);
			foreach (BeltObject belt in newBeltGroup) {
				belt.beltGroup = beltGroups.Count;
			}
		}


		Debug.Log("Splitting belts item slots into lists");
		List<BeltItemSlot> cleanBeltItemSlots = new List<BeltItemSlot>(allBeltItemsSlots);
		while (cleanBeltItemSlots.Count > 0) {
			List<BeltItemSlot> newBeltItemSlotGroup = new List<BeltItemSlot>();
			beltItemSlotGroups.Add(newBeltItemSlotGroup);
			RecursiveGroupBeltItemSlotss(cleanBeltItemSlots[0], cleanBeltItemSlots, newBeltItemSlotGroup, 0);
		}

		Debug.Log("Sorting belt item slots");
		foreach (List<BeltItemSlot> beltItemSlotGroup in beltItemSlotGroups) {
			beltItemSlotGroup.Sort((x, y) => x.index.CompareTo(y.index));
			beltItemSlotGroup.Reverse();
		}

		int n = 0;
		Debug.Log("Shifting belt item slots");
		foreach (List<BeltItemSlot> beltItemSlotGroup in beltItemSlotGroups) {
			int endPointIndex = 0;
			while (beltItemSlotGroup[endPointIndex].outsideConnections.Count > 0 && beltItemSlotGroup[endPointIndex].insideConnections.Count > 0) {
				endPointIndex++;
				if (endPointIndex >= allBeltItemsSlots.Count)
					break;
			}

			//DebugExtensions.DrawSquare(beltItemSlotGroup[endPointIndex].position, new Vector3(0.5f, 0.5f, 0.5f), Color.white, 200f);

			if (endPointIndex != allBeltItemsSlots.Count) {
				var rightPart = beltItemSlotGroup.GetRange(endPointIndex, beltItemSlotGroup.Count - endPointIndex);
				var leftPart = beltItemSlotGroup.GetRange(0, endPointIndex);

				beltItemSlotGroup.Clear();
				beltItemSlotGroup.AddRange(rightPart);
				beltItemSlotGroup.AddRange(leftPart);	
			}


			//if (n == 2) {
			for (int i = 0; i < beltItemSlotGroup.Count - 1; i++) {
				//Debug.DrawLine(beltItemSlotGroup[i].position - Vector3.forward * i * 0.05f, beltItemSlotGroup[i + 1].position - Vector3.forward * (i + 1) * 0.05f, Color.magenta, 200f);
			}
			//}
			n++;
		}
	}

	void RecursiveGroupBeltItemSlotss (BeltItemSlot currentBeltItemSlot, List<BeltItemSlot> cleanBeltItemSlots, List<BeltItemSlot> processedBeltItemSlots, int index) {
		if (currentBeltItemSlot.isProcessed)
			return;

		//Debug.Log(index);
		currentBeltItemSlot.isProcessed = true;
		currentBeltItemSlot.index = index;
		cleanBeltItemSlots.Remove(currentBeltItemSlot);
		processedBeltItemSlots.Add(currentBeltItemSlot);


		foreach (BeltItemSlot outs in currentBeltItemSlot.outsideConnections) {
			//Debug.DrawLine(currentBeltItemSlot.position, outs.position, Color.green, 200f);
			RecursiveGroupBeltItemSlotss(outs, cleanBeltItemSlots, processedBeltItemSlots, index+1);
		}
		foreach (BeltItemSlot ins in currentBeltItemSlot.insideConnections) {
			//Debug.DrawLine(currentBeltItemSlot.position, ins.position, Color.red, 200f);
			RecursiveGroupBeltItemSlotss(ins, cleanBeltItemSlots, processedBeltItemSlots, index-1);
		}
	}

	void RecursiveGroupBelts (BeltObject currentBelt, List<BeltObject> cleanBelts, List<BeltObject> processedBelts) {
		if (currentBelt.isProcessed)
			return;

		currentBelt.isProcessed = true;
		cleanBelts.Remove(currentBelt);
		processedBelts.Add(currentBelt);


		for (int i = 0; i < 4; i++) {
			if (currentBelt.beltOutputs[i]) {
				// Get the next belt that out output is pointing at
				BeltObject nextBelt = GetBeltAtLocation(currentBelt.pos.x + IndexToXY(i).x, currentBelt.pos.y + IndexToXY(i).y);
				if (nextBelt != null) {
					// Check if they are getting input from our output, a requirement for us to be connected
					if (nextBelt.beltInputs[(i + 2) % 4]) {
						// Continue recursion with that next belt
						BeltObject.ConnectBelts(currentBelt, nextBelt, i);
						RecursiveGroupBelts(nextBelt, cleanBelts, processedBelts);
					} else
						currentBelt.isEndPoint = true;
				} else
					currentBelt.isEndPoint = true;
			}
			if (currentBelt.beltInputs[i]) {
				// Get the next belt that out output is pointing at
				BeltObject nextBelt = GetBeltAtLocation(currentBelt.pos.x + IndexToXY(i).x, currentBelt.pos.y + IndexToXY(i).y);
				if (nextBelt != null) {
					// Check if they are getting input from our output, a requirement for us to be connected
					if (nextBelt.beltOutputs[(i + 2) % 4]) {
						// Continue recursion with that next belt
						BeltObject.ConnectBelts(nextBelt, currentBelt, i + 2 % 4);
						RecursiveGroupBelts(nextBelt, cleanBelts, processedBelts);
					} else
						currentBelt.isEndPoint = true;
				} else
					currentBelt.isEndPoint = true;
			}
		}
	}

	BeltObject.Position IndexToXY (int i) {
		BeltObject.Position pos = new BeltObject.Position();
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
