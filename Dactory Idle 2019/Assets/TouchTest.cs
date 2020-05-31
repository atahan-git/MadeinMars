using UnityEngine;
using System.Collections;

public class TouchTest : MonoBehaviour {

	Camera mycam;

	public GameObject[] testObjects;

	public GameObject myPrefab;

	// Use this for initialization
	void Start () {
		mycam = Camera.main;

	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < testObjects.Length; i++) {
			try{
				Ray myRay = mycam.ScreenPointToRay (Input.GetTouch (i).position);
				RaycastHit hit = new RaycastHit ();
				if(Physics.Raycast(myRay, out hit)){

					testObjects[i].transform.position = hit.point;
				}
					
			}catch{}
		}

		if (Input.GetKeyDown (KeyCode.T)) {
			Instantiate (myPrefab, myPrefab.transform.position, myPrefab.transform.rotation);
		}
	}
}
