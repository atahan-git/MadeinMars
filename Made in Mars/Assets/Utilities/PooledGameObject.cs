using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledGameObject : MonoBehaviour {

	public int myId = -1;
	public GameObjectObjectPool myPool;

	public bool isActive = false;

	/// <summary>
	/// Should be called from the object pool
	/// </summary>
	public void EnableObject () {
		gameObject.SetActive(true);
		isActive = true;
		myPool.ActiveObjects += 1;
	}

	
	/// <summary>
	/// Should be called from the object pool
	/// </summary>
	public void DisableObject (){
		gameObject.SetActive(false);
		isActive = false;
		myPool.ActiveObjects -= 1;
	}

	public void DestroyPooledObject (){
		myPool.DestroyPooledObject (myId);
	}
}
