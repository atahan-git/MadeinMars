using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltObject : MonoBehaviour {

	// variable to see if this belt was gone through the setup processing for list generation
	public bool isProcessed = false;
	// processing data
	public bool isEndPoint = false;
	public int beltGroup = -1;
	[SerializeField] int runningLineCount = -1;
	[SerializeField] int totalEdgeCount = -1;

	// this holds all the possible slots on a small 8x8 grid. Corners will never be used
	 BeltItemSlot[,] myBeltItemSlots = new BeltItemSlot[4, 4];

	// This is only used for overpass type
	 BeltItemSlot[,] myBeltItemSlotsLayer2 = new BeltItemSlot[4, 4];

	// A list for easily getting all of the belt item slots
	public List<BeltItemSlot> allBeltItemSlots = new List<BeltItemSlot>();
	public BeltItemSlot[] allBeltItemSlotsArray;

	// Grid Coordinates
	public Position pos;

	public struct Position {
		public int x;
		public int y;

		public Position (int _x, int _y) {
			x = _x;
			y = _y;
		}

		public static Position operator + (Position a, Position b) {
			return new Position(a.x + b.x, a.y + b.y);
		}
	}

	// Belt inputs follow N E S W pattern
	public bool[] beltInputs = new bool[4];
	public bool[] beltOutputs = new bool[4];


	public void SetPosBasedOnWorlPos () {
		pos.x = (int)transform.position.x;
		pos.y = (int)transform.position.y;
	}


	// call this if you wanna remove previous connections as well
	public void RemoveOldItemSlots (List<List<BeltItemSlot>> existingMaster) {
		// go around the edges and remove connections
		for (int x = 0; x < 4; x++) {
			for (int y = 0; y < 4; y++) {
				if (!(x < 4 && x > 1 && y < 4 && y > 1)) {
					BeltItemSlot.RemoveAllConnectionsFromBeltItemSlot(myBeltItemSlots[x, y]);
					BeltItemSlot.RemoveAllConnectionsFromBeltItemSlot(myBeltItemSlotsLayer2[x, y]);
				}
			}
		}

		for (int x = 0; x < 4; x++) {
			for (int y = 0; y < 4; y++) {
				if(myBeltItemSlots[x,y] != null)
					if (myBeltItemSlots[x,y].beltItemSlotGroup != -1)
						existingMaster[myBeltItemSlots[x, y].beltItemSlotGroup].Remove(myBeltItemSlots[x, y]);

				if (myBeltItemSlotsLayer2[x, y] != null)
					if (myBeltItemSlotsLayer2[x, y].beltItemSlotGroup != -1)
						existingMaster[myBeltItemSlotsLayer2[x, y].beltItemSlotGroup].Remove(myBeltItemSlotsLayer2[x, y]);
			}
		}
	}



	// belt item slot coordinates start from top left corner
	/* The Logic goes like this:
	 * 1. check and connect all the middle belts if and only if there is a 'running line' through them, ie behind them is an input and front is output
	 * 2. connect all the edge connections to all the middle pieces
	 * 3. this leaves 3 edge cases out, later correct them
	 *		the edges cases are:
	 *			-corners
	 *			-1 out 2 split in '>v<' and reverse of that
	 *			-2 in 2 out, ins and outs are next to each other - is an overpass
	 */
	public void CreateBeltItemSlots () {

		myBeltItemSlots = new BeltItemSlot[4, 4];
		myBeltItemSlotsLayer2 = new BeltItemSlot[4, 4];
		allBeltItemSlots.Clear();

		// populate the center
		for (int x = 1; x < 3; x++) {
			for (int y = 1; y < 3; y++) {
				myBeltItemSlots[x, y] = new BeltItemSlot(GetBeltItemSlotPos(x, y));
				allBeltItemSlots.Add(myBeltItemSlots[x, y]);
			}
		}

		// run lines through
		runningLineCount = 0;
		for (int x = 1; x < 3; x++) {
			for (int y = 1; y < 3; y++) {
				if (beltInputs[0] && beltOutputs[2]) {
					runningLineCount++;
					BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[x, y], myBeltItemSlots[x, y + 1]);
				}
				if (beltInputs[1] && beltOutputs[3]) {
					runningLineCount++;
					BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[x, y], myBeltItemSlots[x - 1, y]);
				}
				if (beltInputs[2] && beltOutputs[0]) {
					runningLineCount++;
					BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[x, y], myBeltItemSlots[x, y - 1]);
				}
				if (beltInputs[3] && beltOutputs[1]) {
					runningLineCount++;
					BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[x, y], myBeltItemSlots[x + 1, y]);
				}
			}
		}
		runningLineCount /= 4;

		// connect edges to the center
		if (beltInputs[0] || beltOutputs[0])
			for (int x = 1; x < 3; x++) {
				myBeltItemSlots[x, 0] = new BeltItemSlot(GetBeltItemSlotPos(x, 0));
				allBeltItemSlots.Add(myBeltItemSlots[x, 0]);
				if (beltInputs[0]) // spawn the connection around if input or output
					BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[x, 0], myBeltItemSlots[x, 1]);
				else
					BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[x, 1], myBeltItemSlots[x, 0]);
			}
		if (beltInputs[1] || beltOutputs[1])
			for (int y = 1; y < 3; y++) {
				myBeltItemSlots[3, y] = new BeltItemSlot(GetBeltItemSlotPos(3, y));
				allBeltItemSlots.Add(myBeltItemSlots[3, y]);
				if (beltInputs[1]) // spawn the connection around if input or output
					BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[3, y], myBeltItemSlots[2, y]);
				else
					BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[2, y], myBeltItemSlots[3, y]);
			}
		if (beltInputs[2] || beltOutputs[2])
			for (int x = 1; x < 3; x++) {
				myBeltItemSlots[x, 3] = new BeltItemSlot(GetBeltItemSlotPos(x, 3));
				allBeltItemSlots.Add(myBeltItemSlots[x, 3]);
				if (beltInputs[2]) // spawn the connection around if input or output
					BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[x, 3], myBeltItemSlots[x, 2]);
				else
					BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[x, 2], myBeltItemSlots[x, 3]);
			}
		if (beltInputs[3] || beltOutputs[3])
			for (int y = 1; y < 3; y++) {
				myBeltItemSlots[0, y] = new BeltItemSlot(GetBeltItemSlotPos(0, y));
				allBeltItemSlots.Add(myBeltItemSlots[0, y]);
				if (beltInputs[3]) // spawn the connection around if input or output
					BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[0, y], myBeltItemSlots[1, y]);
				else
					BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[1, y], myBeltItemSlots[0, y]);
			}

		// this part is pretty much the hardcoded way of how belts interlock.
		// check the design document to see what exactly this is doing
		// it is pretty much checking if the input/outputs match some of the 'templates' and acts accordingly
		totalEdgeCount = 0;
		for (int i = 0; i < 4; i++)
			totalEdgeCount += (beltInputs[i] || beltOutputs[i]) ? 1 : 0;

		if (runningLineCount == 2 && totalEdgeCount == 4) {
			for (int x = 1; x < 3; x++) {
				for (int y = 1; y < 3; y++) {
					BeltItemSlot.RemoveAllConnectionsFromBeltItemSlot(myBeltItemSlots[x, y]);
				}
			}

			for (int x = 0; x < 4; x++) {
				for (int y = 0; y < 4; y++) {
					myBeltItemSlotsLayer2[x, y] = myBeltItemSlots[x, y];
				}
			}
			for (int x = 1; x < 3; x++) {
				for (int y = 1; y < 3; y++) {
					myBeltItemSlotsLayer2[x, y] = new BeltItemSlot(GetBeltItemSlotPos(x, y));
					allBeltItemSlots.Add(myBeltItemSlotsLayer2[x, y]);
				}
			}

			if (beltInputs[0] && beltInputs[1]) {
				for (int y = 0; y < 3; y++) { BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[1, y], myBeltItemSlots[1, y + 1]); }
				for (int y = 0; y < 3; y++) { BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[2, y], myBeltItemSlots[2, y + 1]); }

				for (int x = 0; x < 3; x++) { BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlotsLayer2[x + 1, 1], myBeltItemSlotsLayer2[x, 1]); }
				for (int x = 0; x < 3; x++) { BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlotsLayer2[x + 1, 2], myBeltItemSlotsLayer2[x, 2]); }
			} else if (beltInputs[1] && beltInputs[2]) {
				for (int x = 0; x < 3; x++) { BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlotsLayer2[x + 1, 1], myBeltItemSlotsLayer2[x, 1]); }
				for (int x = 0; x < 3; x++) { BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlotsLayer2[x + 1, 2], myBeltItemSlotsLayer2[x, 2]); }

				for (int y = 0; y < 3; y++) { BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[1, y + 1], myBeltItemSlots[1, y]); }
				for (int y = 0; y < 3; y++) { BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[2, y + 1], myBeltItemSlots[2, y]); }
			} else if (beltInputs[2] && beltInputs[3]) {
				for (int y = 0; y < 3; y++) { BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[1, y + 1], myBeltItemSlots[1, y]); }
				for (int y = 0; y < 3; y++) { BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[2, y + 1], myBeltItemSlots[2, y]); }

				for (int x = 0; x < 3; x++) { BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlotsLayer2[x, 1], myBeltItemSlotsLayer2[x + 1, 1]); }
				for (int x = 0; x < 3; x++) { BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlotsLayer2[x, 2], myBeltItemSlotsLayer2[x + 1, 2]); }
			} else if (beltInputs[3] && beltInputs[0]) {
				for (int x = 0; x < 3; x++) { BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlotsLayer2[x, 1], myBeltItemSlotsLayer2[x + 1, 1]); }
				for (int x = 0; x < 3; x++) { BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlotsLayer2[x, 2], myBeltItemSlotsLayer2[x + 1, 2]); }

				for (int y = 0; y < 3; y++) { BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[1, y], myBeltItemSlots[1, y + 1]); }
				for (int y = 0; y < 3; y++) { BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[2, y], myBeltItemSlots[2, y + 1]); }
			}
		} else if (runningLineCount == 0 && totalEdgeCount == 2) {
			if (beltInputs[0] && beltOutputs[1]) {
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[1, 1], myBeltItemSlots[1, 2]);
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[1, 2], myBeltItemSlots[2, 2]);
			} else if (beltInputs[1] && beltOutputs[2]) {
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[2, 1], myBeltItemSlots[1, 1]);
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[1, 1], myBeltItemSlots[1, 2]);
			} else if (beltInputs[2] && beltOutputs[3]) {
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[2, 2], myBeltItemSlots[2, 1]);
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[2, 1], myBeltItemSlots[1, 1]);
			} else if (beltInputs[3] && beltOutputs[0]) {
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[1, 2], myBeltItemSlots[2, 2]);
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[2, 2], myBeltItemSlots[2, 1]);
			}

			if (beltInputs[1] && beltOutputs[0]) {
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[2, 2], myBeltItemSlots[1, 2]);
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[1, 2], myBeltItemSlots[1, 1]);
			} else if (beltInputs[2] && beltOutputs[1]) {
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[1, 2], myBeltItemSlots[1, 1]);
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[1, 1], myBeltItemSlots[2, 1]);
			} else if (beltInputs[3] && beltOutputs[2]) {
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[1, 1], myBeltItemSlots[2, 1]);
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[2, 1], myBeltItemSlots[2, 2]);
			} else if (beltInputs[0] && beltOutputs[3]) {
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[2, 1], myBeltItemSlots[2, 2]);
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[2, 2], myBeltItemSlots[1, 2]);
			}
		} else if (runningLineCount == 0 && totalEdgeCount == 3) {
			if (beltInputs[0] && beltOutputs[1] && beltOutputs[3]) {
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[2, 1], myBeltItemSlots[2, 2]);
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[2, 2], myBeltItemSlots[3, 2]);

				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[1, 1], myBeltItemSlots[1, 2]);
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[1, 2], myBeltItemSlots[0, 2]);
			} else if (beltInputs[1] && beltOutputs[2] && beltOutputs[0]) {
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[2, 1], myBeltItemSlots[1, 1]);
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[1, 1], myBeltItemSlots[1, 0]);

				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[2, 2], myBeltItemSlots[1, 2]);
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[1, 2], myBeltItemSlots[1, 3]);
			} else if (beltInputs[2] && beltOutputs[3] && beltOutputs[1]) {
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[2, 2], myBeltItemSlots[2, 1]);
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[2, 1], myBeltItemSlots[3, 1]);

				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[1, 2], myBeltItemSlots[1, 1]);
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[1, 1], myBeltItemSlots[0, 1]);
			} else if (beltInputs[3] && beltOutputs[0] && beltOutputs[2]) {
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[1, 2], myBeltItemSlots[2, 2]);
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[2, 2], myBeltItemSlots[2, 3]);

				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[1, 1], myBeltItemSlots[2, 1]);
				BeltItemSlot.ConnectBeltItemSlots(myBeltItemSlots[2, 1], myBeltItemSlots[2, 0]);
			}
		}

		allBeltItemSlotsArray = allBeltItemSlots.ToArray();
	}

	const float gridSize = 1f;
	const float beltZOffset = -0.1f;
	public const float beltItemSlotDistance = gridSize / 4f;
	Vector3 GetBeltItemSlotPos (int x, int y) {
		return transform.position - new Vector3(gridSize / 2f - gridSize / 8f, -gridSize / 2f + gridSize / 8f, 0) + new Vector3(gridSize / 4f * (float)x, -gridSize / 4f * (float)y, beltZOffset);
	}

	public static bool CanConnectBelts (BeltObject from, BeltObject to) {
		if (from == null || to == null)
			return false;

		int xDiff = from.pos.x - to.pos.x;
		int yDiff = from.pos.y - to.pos.y;

		if (xDiff == 1 && yDiff == 0) {
			if ((from.beltOutputs[3] && to.beltInputs[1]) || (from.beltInputs[3] && to.beltOutputs[1]))
				return true;
		} else if (xDiff == -1 && yDiff == 0) {
			if ((from.beltOutputs[1] && to.beltInputs[3]) || (from.beltInputs[1] && to.beltOutputs[3]))
				return true;
		} else if (xDiff == 0 && yDiff == 1) {
			if ((from.beltOutputs[2] && to.beltInputs[0]) || (from.beltInputs[2] && to.beltOutputs[0]))
				return true;
		} else if (xDiff == 0 && yDiff == -1) {
			if ((from.beltOutputs[0] && to.beltInputs[2]) || (from.beltInputs[0] && to.beltOutputs[2]))
				return true;
		}
		return false;
	}

	public static void ConnectBelts (BeltObject from, BeltObject to, int connectionSide) {
		if (from == null || to == null)
			return;

		switch (connectionSide) {
		case 0:
			for (int x = 1; x < 3; x++) {
				BeltItemSlot.ConnectBeltItemSlots(from.myBeltItemSlots[x, 0], to.myBeltItemSlots[x, 3]);
				BeltItemSlot.ConnectBeltItemSlots(from.myBeltItemSlots[x, 0], to.myBeltItemSlots[x, 3]);
			}
			break;
		case 1:
			for (int y = 1; y < 3; y++) {
				BeltItemSlot.ConnectBeltItemSlots(from.myBeltItemSlots[3, y], to.myBeltItemSlots[0, y]);
				BeltItemSlot.ConnectBeltItemSlots(from.myBeltItemSlots[3, y], to.myBeltItemSlots[0, y]);
			}
			break;
		case 2:
			for (int x = 1; x < 3; x++) {
				BeltItemSlot.ConnectBeltItemSlots(from.myBeltItemSlots[x, 3], to.myBeltItemSlots[x, 0]);
				BeltItemSlot.ConnectBeltItemSlots(from.myBeltItemSlots[x, 3], to.myBeltItemSlots[x, 0]);
			}
			break;
		case 3:
			for (int y = 1; y < 3; y++) {
				BeltItemSlot.ConnectBeltItemSlots(from.myBeltItemSlots[0, y], to.myBeltItemSlots[3, y]);
				BeltItemSlot.ConnectBeltItemSlots(from.myBeltItemSlots[0, y], to.myBeltItemSlots[3, y]);
			}
			break;
		}
	}
}