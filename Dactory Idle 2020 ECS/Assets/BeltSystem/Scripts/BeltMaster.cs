using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;



/// <summary>
/// Controls the flow of the belt processes.
/// The belt update loop starts here, and generally all of the belt connection/destruction operations should go through here as well.
/// </summary>
public class BeltMaster : MonoBehaviour {
	public static BeltMaster s;

	// These are serialized for debugging ease from the inspector.
	[SerializeField] protected List<BeltObject> allBelts = new List<BeltObject>();
	[SerializeField] protected List<BeltPreProcessor.BeltGroup> beltGroups = new List<BeltPreProcessor.BeltGroup>();

	[SerializeField] protected List<BeltItem> allBeltItems = new List<BeltItem>();

	// We need a copy of these classes to run the updates
	protected BeltPreProcessor beltPreProc;
	protected BeltItemSlotUpdateProcessor beltItemSlotProc;

	// Starts the belt loops automatically
	public bool autoStart = true;
	// debug overlay draws for belt slots
	public bool debugDraw = false;

	// Every update items on belt will move one slot.
	public const float beltUpdatePerSecond = 4;
	public const float itemWorldPositionZOffset = -1f;
	
	public ObjectPoolSimple<BeltItem> itemPool;
	[HideInInspector]
	public ObjectPool entityPool; //refactor this asap pls, belt item slot should not access this


	private bool debugCreatorsActive = false;
	protected List<MagicItemCreator> allCreators = new List<MagicItemCreator>();
	protected List<MagicItemDestroyer> allDestroyers = new List<MagicItemDestroyer>();

	public int maxItemCount = 4000;

	EntityManager entityManager;

	private void Awake() {
		if (s != null) {
			Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
		}
		s = this;
	}

	void Start() {
		entityManager = World.Active.EntityManager;

		if (autoStart)
			StartBeltSystem();
	}

	/// <summary>
	/// Starts the belt systems. Should be called at the start of the game once, after the belts are created.
	/// </summary>
	public void StartBeltSystem() {
		print("Starting Belt System");
		itemPool = new ObjectPoolSimple<BeltItem>(maxItemCount, maxItemCount);
		itemPool.SetUp();

		entityPool = GetComponent<ObjectPool>();
		
		SetupBeltSystem();

		CreateGfxs();
		StartBeltSystemLoops();

		if (debugCreatorsActive) {
			allCreators = new List<MagicItemCreator>(FindObjectsOfType<MagicItemCreator>());
			allDestroyers = new List<MagicItemDestroyer>(FindObjectsOfType<MagicItemDestroyer>());
		}
	}

	/// <summary>
	/// Belts need an initial update to their graphics. After starting this is handled by the building belts systems
	/// </summary>
	void CreateGfxs () {
		for (int i = 0; i < allBelts.Count; i++) {
			allBelts[i].UpdateGraphics();
		}
	}

	public void SetupBeltSystem () {
		// Get all the belts
		allBelts = new List<BeltObject>(FindObjectsOfType<BeltObject>());

		// Update the belts internal position data based on world position
		for (int i = 0; i <  allBelts.Count; i++) {
			BeltObject belt = allBelts[i];
			belt.SetPosBasedOnWorlPos();
		}

		// Create the helper class, and run the prepass
		beltPreProc = new BeltPreProcessor(beltGroups, allBeltItems, GetBeltAtLocation);
		beltPreProc.PrepassBelts(allBelts);
		// Create the other helper class
		beltItemSlotProc = new BeltItemSlotUpdateProcessor(itemPool, beltGroups);
	}

	/// <summary>
	/// Starts the belt loops so that items actually move. Should only be called once.
	/// </summary>
	public void StartBeltSystemLoops () {
		StartCoroutine(BeltItemSlotUpdateLoop());
	}

	/// <summary>
	/// Used to remove a belt from the belt update system.
	/// This does NOT physical delete the belt!
	/// </summary>
	/// <param name="Destroyed Belt"></param>
	public void DestroyABelt (BeltObject destroyedBelt){
		if (beltPreProc == null) {
			Debug.LogError("Belt Processor not found! Please run the SetupBeltSystem before deleting any belts!");
			return;
		}
		allBelts.Remove(destroyedBelt);
		beltPreProc.RemoveBelt(destroyedBelt);
	}

	/// <summary>
	/// Used to add a belt connected to another belt in the belt system.
	/// This does NOT physically add a belt!
	/// </summary>
	/// <param name="newBelt"></param>
	/// <param name="updatedBelt"></param>
	public void AddOneBeltConnectedToOne (BeltObject newBelt, BeltObject updatedBelt) {
		allBelts.Add(newBelt);
		beltPreProc.ResetBeltSlots(newBelt);
		beltPreProc.ResetBeltSlots(updatedBelt);
		ProcessBeltGroupingChange(newBelt);
	}

	/// <summary>
	/// Used to add a single belt to the belt update system, not connected to anything yet
	/// This does NOT physically add a belt!
	/// </summary>
	/// <param name="newBelt"></param>
	public void AddOneBelt (BeltObject newBelt) {
		allBelts.Add(newBelt);
		beltPreProc.ResetBeltSlots(newBelt);
		ProcessBeltGroupingChange(newBelt);
	}

	/// <summary>
	/// Used when adding a belt from a save file.
	/// This actually does nothing because you need to run the "SetupBeltSystem", so doing updates for every new belt is redundant.
	/// </summary>
	/// <param name="savedBelt"></param>
	public void AddOneBeltFromSave(BeltObject savedBelt) {
		return;
		savedBelt.CreateBeltItemSlots();
		savedBelt.UpdateGraphics();
		allBelts.Add(savedBelt);
	}

	/// <summary>
	/// Used when connections of a belt has changed, and underlying system needs to be re-calculated
	/// This does NOT physically change a belt.
	/// </summary>
	/// <param name="updatedBelt"></param>
	public void ChangeOneBelt (BeltObject updatedBelt) {
		beltPreProc.ResetBeltSlots(updatedBelt);
		ProcessBeltGroupingChange(updatedBelt);
	}

	/// <summary>
	/// Internal function to update a belt's grouping when anything has happened to the belt
	/// </summary>
	/// <param name="updatedBelt"></param>
	private void ProcessBeltGroupingChange (BeltObject updatedBelt) {
		beltPreProc.ProcessBeltGroupingChange(updatedBelt);
	}

	/// <summary>
	/// The main update loop for the belt system. Makes sure the relevant updates are called in the correct order
	/// </summary>
	IEnumerator BeltItemSlotUpdateLoop () {
		while (true) {
			// Update the internal item positions
			beltItemSlotProc.UpdateBeltItemSlots();
			// Update the visual item positions
			ApplyPositionsToEntities();

			if (debugCreatorsActive) {
				CreateItems();
				DestroyItems();
			}

			//BeltItemGfxUpdateProcessor.UpdateBeltItemPositions();

			yield return new WaitForSeconds(1f / beltUpdatePerSecond);
		}
	}

	/// <summary>
	/// Applies the internal lightweight items' positions to the ECS system entities.
	/// </summary>
	void ApplyPositionsToEntities () {
		for (int i = 0; i < itemPool.objectPool.Length; i++) {
			if (itemPool.objectPool[i].isMovedThisLoop) {
				float3 pos = new float3(
				BeltMaster.s.itemPool.objectPool[i].myRandomOffset.x + BeltMaster.s.itemPool.objectPool[i].mySlot.position.x,
				BeltMaster.s.itemPool.objectPool[i].myRandomOffset.y + BeltMaster.s.itemPool.objectPool[i].mySlot.position.y,
				BeltMaster.itemWorldPositionZOffset);

				entityManager.SetComponentData(BeltMaster.s.entityPool.GetEntity(i), new ItemMovement { targetWithOffset = pos });
			}
		}
	}

	void CreateItems () {
		for (int i = 0; i < allCreators.Count; i++) {
			allCreators[i].CreateItemsBasedOnTick();
		}
	}

	void DestroyItems () {
		for (int i = 0; i < allDestroyers.Count; i++) {
			allDestroyers[i].DestroyItemsOnSlots();
		}
	}


	
	/// <summary>
	/// The actual Update is only needed for debug drawing.
	/// </summary>
	public float timer = 20f;
	public float maxTime = 20f;
	void Update () {
		if (debugDraw) {
			if (timer > maxTime) {
				foreach (BeltPreProcessor.BeltGroup beltGroup in beltGroups)
					foreach (List<BeltItemSlot> beltItemSlotGroup in beltGroup.beltItemSlotGroups)
						foreach (BeltItemSlot beltItemSlot in beltItemSlotGroup)
							beltItemSlot.DebugDraw();
				timer = 0;


			}
			
			foreach (BeltPreProcessor.BeltGroup beltGroup in beltGroups)
				foreach (List<BeltItemSlot> beltItemSlotGroup in beltGroup.beltItemSlotGroups)
					foreach (BeltItemSlot beltItemSlot in beltItemSlotGroup)
						beltItemSlot.DebugItemDraw();

			/*for (int i = 0; i < itemPool.objectpool.Length; i++)
				itemPool.objectpool[i].DebugDraw();*/
			
			timer += Time.deltaTime;
			
			//print("####");
			//print(beltGroups.Count);
			//print(beltGroups[0].beltItemSlotGroups.Count);
			//print(beltGroups[0].beltItemSlotGroups[0].Count);
		}
	}

	/// <summary>
	/// An easy to edit function that the helper methods require to find the belts.
	/// Can be replaced with other functions for unit testing, or edited if the underlying tile system changes.
	/// </summary>
	/// <param name="pos"></param>
	/// <returns></returns>
	protected BeltObject GetBeltAtLocation (Position pos) {
		BeltObject belt = null;
		try {
			belt = Grid.s.GetTile(pos).myBelt.GetComponent<BeltObject>();
		} catch { }
		return belt;
	}


	
	public int activeItemCount = 0;
	/// <summary>
	/// ALL item creation must go through this method.
	/// Creates 1 item at the designated BeltItemSlot
	/// </summary>
	/// <param name="slot"></param>
	/// <param name="itemId">Use the Ids generated by the DataHolder</param>
	/// <returns>Return true if item creation was successful</returns>
	public bool CreateItemAtBeltSlot (BeltItemSlot slot , int itemId) {
		if (slot != null) {
			if (slot.myItem == null) {
				slot.myItem = itemPool.Spawn();
				slot.myItem.myItemId = itemId;
				slot.myItem.myEntityId = entityPool.Spawn(slot.position, slot.position, DataHolder.s.GetItem(itemId));
				activeItemCount++;
				return true;
			} 
		}

		return false;
	}

	/// <summary>
	/// ALL item destruction should go through this method
	/// Destroys the item at the given BeltItemSlot
	/// </summary>
	/// <param name="slot"></param>
	/// <returns>Returns the itemId for successful destruction, -1 if failed.</returns>
	public int DestroyItemAtSlot (BeltItemSlot slot) {
		if (slot != null) {
			if (slot.myItem != null) {
				int itemId = slot.myItem.myItemId;
				entityPool.DestroyPooledObject(slot.myItem.myEntityId);
				itemPool.DestroyPooledObject(slot.myItem);
				slot.myItem.myItemId = -1;
				slot.myItem = null;
				activeItemCount--;
				return itemId;
			}	
		}
		return -1;
	}

	/// <summary>
	/// Used when wanting to destroy a particular item
	/// Only destroys the correct item
	/// </summary>
	/// <param name="slot"></param>
	/// <param name="itemId"></param>
	/// <returns>Return the itemId on success, -1 on failure</returns>
	public int DestroyItemAtSlot (BeltItemSlot slot, int itemId) {
		if (slot != null)
			if (slot.myItem != null)
				if (slot.myItem.myItemId == itemId)
					return DestroyItemAtSlot(slot);
		return -1;
	}
}
