using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// All belts have these BeltItemSlots. This is the smallest part of the belt, and each slot can hold one item, and store its connections.
/// This object is also used in the BeltSlotUpdateProcessor to move items around.
/// </summary>
public class BeltItemSlot {

	// Various flags for the processing
	public bool isProcessed = false;
	public int index;
	
	// The processing group we belong to.
	public List<BeltItemSlot> beltItemSlotGroup;

	public Vector3 position = new Vector3();

	public List<BeltItemSlot> outsideConnections = new List<BeltItemSlot>();
	public List<BeltItemSlot> insideConnections = new List<BeltItemSlot>(); 

	public BeltItem myItem;

	// Debugging bools
	const bool coreDraw = true;
	const bool numberDraw = false;

	public void DebugDraw () {
		// Draw a yellow cube at the transform position
		if (coreDraw) {
			//DebugExtensions.DrawSquare(position, new Vector3(0.08f, 0.08f, 0.08f), Color.black);

			/*foreach (BeltItemSlot connection in outsideConnections)
				DebugExtensions.DrawArrow(position, Vector3.Lerp(position, connection.position, 0.5f), Color.green);
				*/
			foreach (BeltItemSlot connection in insideConnections)
				DebugExtensions.DrawArrow(Vector3.Lerp(position, connection.position, 0.5f), position, Color.red);
		}
		if (numberDraw) {
			DebugExtensions.DrawNumber(position, index);
		}
	}

	int updateOffset = 0;
	/// <summary>
	/// The updating for the belt slot. This will get called by the slot processor.
	/// </summary>
	/// <param name="isMarkedUpdate"></param>
	public void BeltItemSlotUpdate (bool isMarkedUpdate) {
		//Go through all of the possible outside connections, using update Offset as the starting point so that we use a different one each tick
		if (myItem != null) {
			for (int offset = 0; offset < outsideConnections.Count; offset++) {
				int index = ((updateOffset + offset) % outsideConnections.Count);
				if (TryToMoveItem(this, outsideConnections[index], isMarkedUpdate)) {
					updateOffset++;
					updateOffset %= outsideConnections.Count;
					break;
				}

			}
		}
	}

	//tries to move the item in one of the slot to the other if the slot is empty
	static bool TryToMoveItem (BeltItemSlot from, BeltItemSlot to, bool isMarkedUpdate) {
		//Debug.Log("Trying to move item");
		if (from.myItem != null && to.myItem == null) {
			// If this item is not 'processed' ie not moved this update
			if (!from.myItem.isProcessedThisLoop) {
				// do it either if its the marked update loop and we are marked or if it is not the marked update loop
				if (!isMarkedUpdate || from.myItem.isMarkedThisLoop) {
					from.myItem.isProcessedThisLoop = true;
					to.myItem = from.myItem;
					from.myItem = null;
					to.myItem.mySlot = to;
					to.myItem.isMovedThisLoop = true;
					return true;
				}
			}
		} else if (from.myItem != null) {
			from.myItem.isMarkedThisLoop = true;
			//Debug.Log("Item Marked");
		} 
		return false;
	}

	/// <summary>
	/// Connects two slots together.
	/// </summary>
	/// <param name="from"></param>
	/// <param name="to"></param>
	public static void ConnectBeltItemSlots (BeltItemSlot from, BeltItemSlot to) {
		if (from == null || to == null)
			return;

		from.outsideConnections.Remove(to);
		from.insideConnections.Remove(to);
		to.outsideConnections.Remove(from);
		to.insideConnections.Remove(from);

		from.outsideConnections.Add(to);
		to.insideConnections.Add(from);
	}

	/// <summary>
	/// Disconnects two slots
	/// </summary>
	/// <param name="from"></param>
	/// <param name="to"></param>
	public static void DisconnectBeltItemSlots (BeltItemSlot from, BeltItemSlot to) {
		if (from == null || to == null)
			return;

		from.outsideConnections.Remove(to);
		from.insideConnections.Remove(to);
		to.outsideConnections.Remove(from);
		to.insideConnections.Remove(from);
	}


	/// <summary>
	/// "Resets" the slot, deleting the item on top, and removing the connections
	/// </summary>
	/// <param name="target"></param>
	public static void ResetBeltItemSlot (BeltItemSlot target) {
		if (target == null)
			return;
		if(target.myItem != null)
			BeltMaster.s.DestroyItemAtSlot(target);

		RemoveAllConnections(target);
	}

	/// <summary>
	/// Removes all the connections from the slot
	/// </summary>
	/// <param name="from"></param>
	static void RemoveAllConnections (BeltItemSlot from) {
		if (from == null)
			return;

		while (from.outsideConnections.Count > 0) {
			DisconnectBeltItemSlots(from, from.outsideConnections[0]);
		}

		while (from.insideConnections.Count > 0) {
			DisconnectBeltItemSlots(from, from.insideConnections[0]);
		}
	}

	// Constructor
	public BeltItemSlot (Vector3 p) {
		position = p;
	}
}
