using UnityEngine;
using System.Collections;

public class ItemSprite : MonoBehaviour {

	public int x = 0;
	public int y = 0;


	SpriteRenderer mySprite;
	// Use this for initialization
	void Start () {
		mySprite = GetComponentInChildren<SpriteRenderer> ();
		CantPlace ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void CantPlace () {
		//print ("cant");
		mySprite.color = Color.red;
	}

	public void Placeable () {
		//print ("place");
		mySprite.color = Color.green;
	}
}
