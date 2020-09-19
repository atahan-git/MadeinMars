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

public class BeltMaster : MonoBehaviour {
	public static BeltMaster s;

	[SerializeField] protected List<BeltObject> allBelts = new List<BeltObject>();
	[SerializeField] protected List<BeltPreProcessor.BeltGroup> beltGroups = new List<BeltPreProcessor.BeltGroup>();

	[SerializeField] protected List<BeltItem> allBeltItems = new List<BeltItem>();

	protected BeltPreProcessor beltPreProc;
	protected BeltItemSlotUpdateProcessor beltItemSlotProc;

	public bool autoStart = true;
	public bool debugDraw = false;

	public const float beltUpdatePerSecond = 4;
	public const float itemWorldPositionZOffset = -1f;

	protected Dictionary<BeltObject.Position, BeltObject> allBeltsCoords = new Dictionary<BeltObject.Position, BeltObject>();

	public ObjectPoolSimple<BeltItem> itemPool;
	 public ObjectPoolECS entityPoolEcs; //refactor this asap pls, belt item slot should not access this


	protected List<MagicItemCreator> allCreators = new List<MagicItemCreator>();
	protected List<MagicItemDestroyer> allDestroyers = new List<MagicItemDestroyer>();

	public int maxItemCount = 4000;
	// Start is called before the first frame update
	void Start() {
		s = this;

		if (autoStart) {
			Invoke("LateStart", 5f);
		}
	}

	public void LateStart () {
		print("Starting Belt System");
		itemPool = new ObjectPoolSimple<BeltItem>(maxItemCount, maxItemCount);
		itemPool.SetUp();

		entityPoolEcs = GetComponent<ObjectPoolECS>();

		SetupBeltSystem();

		//StartCoroutine(CreateGfxsSlowly());
		StartBeltSystemLoops();


		allCreators = new List<MagicItemCreator>(FindObjectsOfType<MagicItemCreator>());
		allDestroyers = new List<MagicItemDestroyer>(FindObjectsOfType<MagicItemDestroyer>());

		FindObjectOfType<AutoRotate>().speed *= -1f;
	}

	IEnumerator CreateGfxsSlowly () {
		
		for (int i = 0; i < allBelts.Count; i++) {
			BeltObject belt = allBelts[i];

			belt.GetComponent<BeltGfx>().UpdateGraphics(belt.beltInputs, belt.beltOutputs);

			if (i % 10 == 0)
				yield return null;
		}

		yield return null;
	}

	public void SetupBeltSystem () {
		allBelts = new List<BeltObject>(FindObjectsOfType<BeltObject>());

		for (int i = 0; i <  allBelts.Count; i++) {
			BeltObject belt = allBelts[i];
			belt.SetPosBasedOnWorlPos();
			allBeltsCoords[belt.pos] = belt;
		}

		beltPreProc = new BeltPreProcessor(beltGroups, allBeltItems, GetBeltAtLocation);

		beltPreProc.PrepassBelts(allBelts);

		beltItemSlotProc = new BeltItemSlotUpdateProcessor(itemPool, beltGroups);
	}

	public void StartBeltSystemLoops () {
		StartCoroutine(BeltItemSlotUpdateLoop());
	}

	public void AddOneBelt (BeltObject newBelt) {
		allBelts.Add(newBelt);
		ChangeOneBelt(newBelt);
	}

	public void ChangeOneBelt (BeltObject updatedBelt) {
		beltPreProc.ProcessOneBeltChange(updatedBelt);
	}

	IEnumerator BeltItemSlotUpdateLoop () {
		while (true) {
			beltItemSlotProc.UpdateBeltItemSlots();

			ApplyPositionsToEntities();

			CreateItems();

			DestroyItems();


			//BeltItemGfxUpdateProcessor.UpdateBeltItemPositions();

			yield return new WaitForSeconds(1f / beltUpdatePerSecond);
		}
	}


	void ApplyPositionsToEntities () {
		for (int i = 0; i < itemPool.objectPool.Length; i++) {
			if (itemPool.objectPool[i].isMovedThisLoop) {
				float3 pos = new float3(
				BeltMaster.s.itemPool.objectPool[i].myRandomOffset.x + BeltMaster.s.itemPool.objectPool[i].mySlot.position.x,
				BeltMaster.s.itemPool.objectPool[i].myRandomOffset.y + BeltMaster.s.itemPool.objectPool[i].mySlot.position.y,
				BeltMaster.itemWorldPositionZOffset);

				World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(BeltMaster.s.entityPoolEcs.GetEntity(i), new ItemMovement { targetWithOffset = pos });
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


	float timer = 500f;
	float maxTime = 500f;
	// Update is called once per frame
	void Update () {
		if (debugDraw) {
			if (timer > maxTime) {
				foreach (BeltPreProcessor.BeltGroup beltGroup in beltGroups)
					foreach (List<BeltItemSlot> beltItemSlotGroup in beltGroup.beltItemSlotGroups)
						foreach (BeltItemSlot beltItemSlot in beltItemSlotGroup)
							beltItemSlot.DebugDraw();
				timer = 0;


			}


			/*for (int i = 0; i < itemPool.objectpool.Length; i++)
				itemPool.objectpool[i].DebugDraw();*/
		}


		timer += Time.deltaTime;
		//print("####");
		//print(beltGroups.Count);
		//print(beltGroups[0].beltItemSlotGroups.Count);
		//print(beltGroups[0].beltItemSlotGroups[0].Count);
	}

	protected BeltObject GetBeltAtLocation (BeltObject.Position pos) {
		BeltObject belt;
		allBeltsCoords.TryGetValue(pos, out belt);
		return belt;
	}


	public int activeItemCount = 0;
	public bool CreateItemAtBeltSlot (BeltItemSlot slot /*, int itemTyep*/) {
		if (slot != null) {
			if (slot.myItem == null) {
				slot.myItem = itemPool.Spawn();
				slot.myItem.myEntityId = entityPoolEcs.Spawn(slot.position, slot.position);
				activeItemCount++;
				return true;
			} 
		}

		return false;
	}

	public void DestroyItemAtSlot (BeltItemSlot slot) {
		if (slot != null) {
			if (slot.myItem != null) {
				entityPoolEcs.DestroyPooledObject(slot.myItem.myEntityId);
				itemPool.DestroyPooledObject(slot.myItem);
				slot.myItem = null;
				activeItemCount--;
			}	
		}
	}
}
