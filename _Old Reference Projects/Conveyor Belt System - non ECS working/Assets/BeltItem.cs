using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltItem : MonoBehaviour {

	public bool isProcessedThisLoop = false;
	public bool isMarkedThisLoop = false;
	//public Vector3 position = new Vector3();
	public BeltItemSlot mySlot;
	public Vector3 myRandomOffset;

	public float randAmount = 0.03f;
	private void Start () {
		myRandomOffset = new Vector3(Random.Range(-randAmount, randAmount), Random.Range(-randAmount, randAmount), 0);
	}
	public void DebugDraw () {
		// Draw a yellow cube at the transform position
		DebugExtensions.DrawSquare(transform.position, new Vector3(0.05f, 0.05f, 0.05f), Color.blue);
	}


	public void DestroyItem () {
		Destroy(gameObject);
	}
}