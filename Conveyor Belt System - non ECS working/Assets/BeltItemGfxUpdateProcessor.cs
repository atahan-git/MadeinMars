using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltItemGfxUpdateProcessor
{
	public List<BeltItem> allBeltItems = new List<BeltItem>();

	public BeltItemGfxUpdateProcessor (List<BeltItem> _allBeltItems) {
		allBeltItems = _allBeltItems;
	}


	public void UpdateBeltItemGfxs (float beltUpdatePerSecond, float deltaTime) {
		foreach (BeltItem beltItem in allBeltItems) {
			beltItem.transform.position = Vector3.MoveTowards(beltItem.transform.position, beltItem.mySlot.position + beltItem.myRandomOffset, BeltObject.beltItemSlotDistance * beltUpdatePerSecond * deltaTime);
		}
	}
}
