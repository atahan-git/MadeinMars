using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BeltMaster : MonoBehaviour {
	protected static BeltMaster s;

	[SerializeField]protected List<BeltObject> allBelts = new List<BeltObject>();
	protected List<List<BeltObject>> beltGroups = new List<List<BeltObject>>();

	[SerializeField] protected List<BeltItemSlot> allBeltItemsSlots = new List<BeltItemSlot>();
	protected List<List<BeltItemSlot>> beltItemSlotGroups = new List<List<BeltItemSlot>>();

	[SerializeField] protected List<BeltItem> allBeltItems = new List<BeltItem>();

	protected BeltPreProcessor beltProc;
	protected BeltItemSlotUpdateProcessor beltItemSlotProc;
	protected BeltItemGfxUpdateProcessor beltItemGfxProc;

	public bool autoStart = true;
	public bool debugDraw = false;

	public float beltUpdatePerSecond = 4;

	// Start is called before the first frame update
	void Start() {
		s = this;

		if (autoStart) {
			print("Starting Belt System");
			SetupBeltSystem();
			StartBeltSystemLoops();			
		}
	}

	public void SetupBeltSystem () {
		allBelts = new List<BeltObject>(FindObjectsOfType<BeltObject>());

		foreach (BeltObject belt in allBelts) {
			belt.SetPosBasedOnWorlPos();
			belt.GetComponent<BeltGfx>().UpdateGraphics(belt.beltInputs, belt.beltOutputs);
		}

		beltProc = new BeltPreProcessor(allBelts, beltGroups, allBeltItemsSlots, beltItemSlotGroups, allBeltItems, GetBeltAtLocation);

		beltProc.PrepassBelts();

		beltItemSlotProc = new BeltItemSlotUpdateProcessor(allBeltItems, beltItemSlotGroups);

		beltItemGfxProc = new BeltItemGfxUpdateProcessor(allBeltItems);
	}

	public void StartBeltSystemLoops () {
		StartCoroutine(BeltItemSlotUpdateLoop());
		StartCoroutine(BeltItemGfxUpdateLoop());
	}

	IEnumerator BeltItemSlotUpdateLoop () {
		while (true) {
			beltItemSlotProc.UpdateBeltItemSlots();

			yield return new WaitForSeconds(1f / beltUpdatePerSecond);
		}
	}

	IEnumerator BeltItemGfxUpdateLoop () {
		while (true) {
			beltItemGfxProc.UpdateBeltItemGfxs(beltUpdatePerSecond, Time.deltaTime);
			yield return null;
		}
	}


	// Update is called once per frame
	void Update()
    {
		if (debugDraw)
			foreach (BeltItemSlot beltItemSlot in allBeltItemsSlots) { beltItemSlot.DebugDraw(); }
    }

	protected BeltObject GetBeltAtLocation (int x, int y) {
		foreach (BeltObject belt in allBelts) {
			if (belt.pos.x == x && belt.pos.y == y) {
				return belt;
			}
		}

		return null;
	}

	public static bool CreateItemAtBeltSlot (BeltItem item, BeltItemSlot slot) {
		if (slot.myItem == null) {
			s.allBeltItems.Add(item);
			item.transform.position = slot.position;
			item.mySlot = slot;
			slot.myItem = item;
			return true;
		} else {
			item.DestroyItem();
			return false;
		}
	}

	public static void DestroyItem (BeltItem item) {
		if (item != null) {
			item.mySlot.myItem = null;
			s.allBeltItems.Remove(item);
			item.DestroyItem();
		}
	}
}