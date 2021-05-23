using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnBeltObject : MonoBehaviour {

	public int myId = -1;
	public ItemOnBeltObjectPool myPool;

	public bool isActive = false;

	public float lifetime = -1f; // to be updated externally
	public int direction;
	public bool isBeltObject;
	
	/// <summary>
	/// Should be called from the object pool
	/// </summary>
	public void EnableObject (Vector3 position, Sprite mySprite, int myDirection, bool _isBeltObject) {
		transform.position = position;
		GetComponent<SpriteRenderer>().sprite = mySprite;
		direction = myDirection;
		lifetime = 0f;
		/*if (!isActive) {
			gameObject.SetActive(true);
		}*/
		isActive = true;
		myPool.ActiveObjects += 1;
		isBeltObject = _isBeltObject;
	}


	private readonly Vector3 dissappearLand = new Vector3(-100, -100, 0f);
	/// <summary>
	/// Should be called from the object pool
	/// </summary>
	public void DisableObject (){
		/*gameObject.SetActive(false);*/
		transform.position = dissappearLand;
		isActive = false;
		myPool.ActiveObjects -= 1;
	}

	public void DestroyPooledObject (){
		myPool.DestroyPooledObject (myId);
	}
}
