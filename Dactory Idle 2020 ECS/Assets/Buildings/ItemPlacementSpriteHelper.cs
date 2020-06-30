using UnityEngine;
using System.Collections;

public class ItemPlacementSpriteHelper : MonoBehaviour {

	public int x = 0;
	public int y = 0;


	SpriteRenderer mySprite;
	// Use this for initialization
	void Start () {
		mySprite = GetComponentInChildren<SpriteRenderer>();
		CantPlace();
	}

	public void CantPlace () {
		if (mySprite == null)
			mySprite = GetComponentInChildren<SpriteRenderer>();

		//print ("cant");
		mySprite.color = Color.red;
	}

	public void Placeable () {
		if (mySprite == null)
			mySprite = GetComponentInChildren<SpriteRenderer>();
		//print ("place");
		mySprite.color = Color.green;
	}
}
