﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class BeltItem : IPoolableClass {

	public bool isProcessedThisLoop = false;
	public bool isMarkedThisLoop = false;
	public bool isMovedThisLoop = false;
	//public Vector3 position = new Vector3();
	public BeltItemSlot mySlot;
	public Vector3 myRandomOffset;

	public int myId { get; set; }
	public int myEntityId;

	/*public float randAmount = 0.03f;
	private void Start () {
		myRandomOffset = new Vector3(Random.Range(-randAmount, randAmount), Random.Range(-randAmount, randAmount), 0);
	}


	public void DestroyItem () {
		Destroy(gameObject);
	}*/

	public void DebugDraw () {
		if(mySlot!=null)
			DebugExtensions.DrawSquare(mySlot.position+myRandomOffset, new Vector3(0.05f, 0.05f, 0.05f), Color.blue,false);
	}
}