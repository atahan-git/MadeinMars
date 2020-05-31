using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BeltMaster : MonoBehaviour {
	protected static BeltMaster s;

	[SerializeField] protected List<BeltObject> allBelts = new List<BeltObject>();
	[SerializeField] protected List<BeltPreProcessor.BeltGroup> beltGroups = new List<BeltPreProcessor.BeltGroup>();

	[SerializeField] protected List<BeltItem> allBeltItems = new List<BeltItem>();

	protected BeltPreProcessor beltPreProc;
	protected BeltItemSlotUpdateProcessor beltItemSlotProc;
	protected BeltItemGfxUpdateProcessor beltItemGfxProc;

	public bool autoStart = true;
	public bool debugDraw = false;

	public float beltUpdatePerSecond = 4;

	protected Dictionary<BeltObject.Position, BeltObject> allBeltsCoords = new Dictionary<BeltObject.Position, BeltObject>();

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
			allBeltsCoords[belt.pos] = belt;
		}

		beltPreProc = new BeltPreProcessor(beltGroups, allBeltItems, GetBeltAtLocation);

		beltPreProc.PrepassBelts(allBelts);

		beltItemSlotProc = new BeltItemSlotUpdateProcessor(allBeltItems, beltGroups);

		beltItemGfxProc = new BeltItemGfxUpdateProcessor(allBeltItems);
	}

	public void StartBeltSystemLoops () {
		StartCoroutine(BeltItemSlotUpdateLoop());
		StartCoroutine(BeltItemGfxUpdateLoop());
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

			yield return new WaitForSeconds(1f / beltUpdatePerSecond);
		}
	}

	IEnumerator BeltItemGfxUpdateLoop () {
		while (true) {
			beltItemGfxProc.UpdateBeltItemGfxs(beltUpdatePerSecond, Time.deltaTime);
			yield return null;
		}
	}


	float timer = 500f;
	float maxTime = 500f;
	// Update is called once per frame
	void Update () {
		if (debugDraw)
			if (timer > maxTime) {
				foreach (BeltPreProcessor.BeltGroup beltGroup in beltGroups)
					foreach (List<BeltItemSlot> beltItemSlotGroup in beltGroup.beltItemSlotGroups)
						foreach (BeltItemSlot beltItemSlot in beltItemSlotGroup)
							beltItemSlot.DebugDraw();
				timer = 0;
			}

		timer += Time.deltaTime;
		//print("####");
		//print(beltGroups.Count);
		//print(beltGroups[0].beltItemSlotGroups.Count);
		//print(beltGroups[0].beltItemSlotGroups[0].Count);
	}

	protected BeltObject GetBeltAtLocation (int x, int y) {
		BeltObject belt;
		allBeltsCoords.TryGetValue(new BeltObject.Position(x, y), out belt);
		return belt;
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