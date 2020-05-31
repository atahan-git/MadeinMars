using UnityEngine;
using System.Collections;

public class BeltSpeed : MonoBehaviour {

	public static BeltSpeed s;

	public float speed = 1.8f;
	public bool mode = true;

	public float randomValue = 0.05f;

	// Use this for initialization
	void Awake () {
		s = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
