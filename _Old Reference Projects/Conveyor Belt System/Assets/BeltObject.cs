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
	public BeltItemSlot[,] myBeltItemSlots = new BeltItemSlot[4, 4];

	// Grid Coordinates
	public Position pos;

	public struct Position {
		public int x;
		public int y;
	}

	// Belt inputs follow N E S W pattern
	public bool[] beltInputs = new bool[4];
	public bool[] beltOutputs = new bool[4];


	public void SetPosBasedOnWorlPos () {
		pos.x = (int)transform.position.x;
		pos.y = (int)transform.position.y;
	}


	// belt item slot coordinates start from top left corner
	/* The Logic goes like this:
	 * 1. check and connect all the middle belts if and only if there is a 'running line' through them, ie behind them is an input and front is output
	 * 2. connect all the edge connections to all the middle pieces
	 * 3. this leaves 3 edge cases out, later correct them
	 *		the edges cases are:
	 *			-corners
	 *			-1 out 2 split in '>v<' and reverse of that
	 *			-2 in 2 out in a corner style - ie put two corners by rotating one by 180
	 */
	public List<BeltItemSlot> CreateBeltItemSlots () {
		myBeltItemSlots = new BeltItemSlot[4, 4];
		List<BeltItemSlot> allBeltItemSlots = new List<BeltItemSlot>();

		for (int x = 1; x < 3; x++) {
			for (int y = 1; y < 3; y++) {
				myBeltItemSlots[x, y] = new BeltItemSlot(GetBeltPos(x, y));
				allBeltItemSlots.Add(myBeltItemSlots[x, y]);
			}
		}

		runningLineCount = 0;
		for (int x = 1; x < 3; x++) {
			for (int y = 1; y < 3; y++) {
				if (beltInputs[0] && beltOutputs[2]) {
					runningLineCount++;
					BeltItemSlot.ConnectBelts(myBeltItemSlots[x, y], myBeltItemSlots[x, y + 1]);
				}
				if (beltInputs[1] && beltOutputs[3]) {
					runningLineCount++;
					BeltItemSlot.ConnectBelts(myBeltItemSlots[x, y], myBeltItemSlots[x - 1, y]);
				}
				if (beltInputs[2] && beltOutputs[0]) {
					runningLineCount++;
					BeltItemSlot.ConnectBelts(myBeltItemSlots[x, y], myBeltItemSlots[x, y - 1]);
				}
				if (beltInputs[3] && beltOutputs[1]) {
					runningLineCount++;
					BeltItemSlot.ConnectBelts(myBeltItemSlots[x, y], myBeltItemSlots[x + 1, y]);
				}
			}
		}
		runningLineCount /= 4;

		if (beltInputs[0] || beltOutputs[0])
			for (int x = 1; x < 3; x++) {
				myBeltItemSlots[x, 0] = new BeltItemSlot(GetBeltPos(x, 0));
				allBeltItemSlots.Add(myBeltItemSlots[x, 0]);
				if (beltInputs[0]) // spawn the connection around if input or output
					BeltItemSlot.ConnectBelts(myBeltItemSlots[x, 0], myBeltItemSlots[x, 1]);
				else
					BeltItemSlot.ConnectBelts(myBeltItemSlots[x, 1], myBeltItemSlots[x, 0]);
			}
		if (beltInputs[1] || beltOutputs[1])
			for (int y = 1; y < 3; y++) {
				myBeltItemSlots[3, y] = new BeltItemSlot(GetBeltPos(3, y));
				allBeltItemSlots.Add(myBeltItemSlots[3, y]);
				if (beltInputs[1]) // spawn the connection around if input or output
					BeltItemSlot.ConnectBelts(myBeltItemSlots[3, y], myBeltItemSlots[2, y]);
				else
					BeltItemSlot.ConnectBelts(myBeltItemSlots[2, y], myBeltItemSlots[3, y]);
			}
		if (beltInputs[2] || beltOutputs[2])
			for (int x = 1; x < 3; x++) {
				myBeltItemSlots[x, 3] = new BeltItemSlot(GetBeltPos(x, 3));
				allBeltItemSlots.Add(myBeltItemSlots[x, 3]);
				if (beltInputs[2]) // spawn the connection around if input or output
					BeltItemSlot.ConnectBelts(myBeltItemSlots[x, 3], myBeltItemSlots[x, 2]);
				else
					BeltItemSlot.ConnectBelts(myBeltItemSlots[x, 2], myBeltItemSlots[x, 3]);
			}
		if (beltInputs[3] || beltOutputs[3])
			for (int y = 1; y < 3; y++) {
				myBeltItemSlots[0, y] = new BeltItemSlot(GetBeltPos(0, y));
				allBeltItemSlots.Add(myBeltItemSlots[0, y]);
				if (beltInputs[3]) // spawn the connection around if input or output
					BeltItemSlot.ConnectBelts(myBeltItemSlots[0, y], myBeltItemSlots[1, y]);
				else
					BeltItemSlot.ConnectBelts(myBeltItemSlots[1, y], myBeltItemSlots[0, y]);
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
					BeltItemSlot.RemoveAllConnections(myBeltItemSlots[x, y]);
				}
			}

			if (beltInputs[0] && beltInputs[1]) {
				BeltItemSlot.ConnectBelts(myBeltItemSlots[1, 0], myBeltItemSlots[1, 1]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[1, 1], myBeltItemSlots[0, 1]);

				BeltItemSlot.ConnectBelts(myBeltItemSlots[3, 2], myBeltItemSlots[2, 2]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[2, 2], myBeltItemSlots[2, 3]);
			} else if (beltInputs[1] && beltInputs[2]) {
				BeltItemSlot.ConnectBelts(myBeltItemSlots[3, 1], myBeltItemSlots[2, 1]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[2, 1], myBeltItemSlots[2, 0]);

				BeltItemSlot.ConnectBelts(myBeltItemSlots[1, 3], myBeltItemSlots[1, 2]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[1, 2], myBeltItemSlots[0, 2]);
			} else if (beltInputs[2] && beltInputs[3]) {
				BeltItemSlot.ConnectBelts(myBeltItemSlots[2, 3], myBeltItemSlots[2, 2]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[2, 2], myBeltItemSlots[3, 2]);

				BeltItemSlot.ConnectBelts(myBeltItemSlots[0, 1], myBeltItemSlots[1, 1]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[1, 1], myBeltItemSlots[1, 0]);
			} else if (beltInputs[3] && beltInputs[0]) {
				BeltItemSlot.ConnectBelts(myBeltItemSlots[0, 2], myBeltItemSlots[1, 2]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[1, 2], myBeltItemSlots[1, 3]);

				BeltItemSlot.ConnectBelts(myBeltItemSlots[2, 0], myBeltItemSlots[2, 1]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[2, 1], myBeltItemSlots[3, 1]);
			}
		} else if (runningLineCount == 0 && totalEdgeCount == 2) {
			if (beltInputs[0] && beltOutputs[1]) {
				BeltItemSlot.ConnectBelts(myBeltItemSlots[1, 1], myBeltItemSlots[1, 2]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[1, 2], myBeltItemSlots[2, 2]);
			} else if (beltInputs[1] && beltOutputs[2]) {
				BeltItemSlot.ConnectBelts(myBeltItemSlots[2, 1], myBeltItemSlots[1, 1]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[1, 1], myBeltItemSlots[1, 2]);
			} else if (beltInputs[2] && beltOutputs[3]) {
				BeltItemSlot.ConnectBelts(myBeltItemSlots[2, 2], myBeltItemSlots[2, 1]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[2, 1], myBeltItemSlots[1, 1]);
			} else if (beltInputs[3] && beltOutputs[0]) {
				BeltItemSlot.ConnectBelts(myBeltItemSlots[1, 2], myBeltItemSlots[2, 2]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[2, 2], myBeltItemSlots[2, 1]);
			}

			if (beltInputs[1] && beltOutputs[0]) {
				BeltItemSlot.ConnectBelts(myBeltItemSlots[2, 2], myBeltItemSlots[1, 2]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[1, 2], myBeltItemSlots[1, 1]);
			} else if (beltInputs[2] && beltOutputs[1]) {
				BeltItemSlot.ConnectBelts(myBeltItemSlots[1, 2], myBeltItemSlots[1, 1]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[1, 1], myBeltItemSlots[2, 1]);
			} else if (beltInputs[3] && beltOutputs[2]) {
				BeltItemSlot.ConnectBelts(myBeltItemSlots[1, 1], myBeltItemSlots[2, 1]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[2, 1], myBeltItemSlots[2, 2]);
			} else if (beltInputs[0] && beltOutputs[3]) {
				BeltItemSlot.ConnectBelts(myBeltItemSlots[2, 1], myBeltItemSlots[2, 2]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[2, 2], myBeltItemSlots[1, 2]);
			}
		} else if (runningLineCount == 0 && totalEdgeCount == 3) {
			if (beltInputs[0] && beltOutputs[1] && beltOutputs[3]) {
				BeltItemSlot.ConnectBelts(myBeltItemSlots[2, 1], myBeltItemSlots[2, 2]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[2, 2], myBeltItemSlots[3, 2]);

				BeltItemSlot.ConnectBelts(myBeltItemSlots[1, 1], myBeltItemSlots[1, 2]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[1, 2], myBeltItemSlots[0, 2]);
			} else if (beltInputs[1] && beltOutputs[2] && beltOutputs[0]) {
				BeltItemSlot.ConnectBelts(myBeltItemSlots[2, 1], myBeltItemSlots[1, 1]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[1, 1], myBeltItemSlots[1, 0]);

				BeltItemSlot.ConnectBelts(myBeltItemSlots[2, 2], myBeltItemSlots[1, 2]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[1, 2], myBeltItemSlots[1, 3]);
			} else if (beltInputs[2] && beltOutputs[3] && beltOutputs[1]) {
				BeltItemSlot.ConnectBelts(myBeltItemSlots[2, 2], myBeltItemSlots[2, 1]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[2, 1], myBeltItemSlots[3, 1]);

				BeltItemSlot.ConnectBelts(myBeltItemSlots[1, 2], myBeltItemSlots[1, 1]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[1, 1], myBeltItemSlots[0, 1]);
			} else if (beltInputs[3] && beltOutputs[0] && beltOutputs[2]) {
				BeltItemSlot.ConnectBelts(myBeltItemSlots[1, 2], myBeltItemSlots[2, 2]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[2, 2], myBeltItemSlots[2, 3]);

				BeltItemSlot.ConnectBelts(myBeltItemSlots[1, 1], myBeltItemSlots[2, 1]);
				BeltItemSlot.ConnectBelts(myBeltItemSlots[2, 1], myBeltItemSlots[2, 0]);
			}

			
		}

		return allBeltItemSlots;
	}

	const float gridSize = 1f;
	const float beltZOffset = -0.1f;
	public const float beltItemSlotDistance = gridSize / 4f;
	Vector3 GetBeltPos (int x, int y) {
		return transform.position - new Vector3(gridSize / 2f - gridSize / 8f, -gridSize / 2f + gridSize / 8f, 0) + new Vector3(gridSize / 4f * (float)x, -gridSize / 4f * (float)y, beltZOffset);
	}


	public static void ConnectBelts (BeltObject from, BeltObject to, int connectionSide) {
		if (from == null || to == null)
			return;

		switch (connectionSide) {
		case 0:
			for (int x = 1; x < 3; x++) {
				BeltItemSlot.ConnectBelts(from.myBeltItemSlots[x, 0], to.myBeltItemSlots[x, 3]);
				BeltItemSlot.ConnectBelts(from.myBeltItemSlots[x, 0], to.myBeltItemSlots[x, 3]);
			}
			break;
		case 1:
			for (int y = 1; y < 3; y++) {
				BeltItemSlot.ConnectBelts(from.myBeltItemSlots[3, y], to.myBeltItemSlots[0, y]);
				BeltItemSlot.ConnectBelts(from.myBeltItemSlots[3, y], to.myBeltItemSlots[0, y]);
			}
			break;
		case 2:
			for (int x = 1; x < 3; x++) {
				BeltItemSlot.ConnectBelts(from.myBeltItemSlots[x, 3], to.myBeltItemSlots[x, 0]);
				BeltItemSlot.ConnectBelts(from.myBeltItemSlots[x, 3], to.myBeltItemSlots[x, 0]);
			}
			break;
		case 3:
			for (int y = 1; y < 3; y++) {
				BeltItemSlot.ConnectBelts(from.myBeltItemSlots[0, y], to.myBeltItemSlots[3, y]);
				BeltItemSlot.ConnectBelts(from.myBeltItemSlots[0, y], to.myBeltItemSlots[3, y]);
			}
			break;
		}
	}
}
