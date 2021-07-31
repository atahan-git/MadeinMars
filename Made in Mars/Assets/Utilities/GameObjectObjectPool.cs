using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectObjectPool : MonoBehaviour {

	public bool autoExpand = true; //dont change this at runtime
	public GameObject myObject;
	public int poolSize = 50;
	public int maxPoolSize = 3000;
	[Space]
	public int ExistingObjects;
	public int ActiveObjects;

	void Awake (){
		if (myObject.GetComponent<PooledGameObject> () == null)
			myObject.AddComponent<PooledGameObject> ();

		myObject.GetComponent<PooledGameObject> ().myPool = this;

		SetUp (poolSize);
	}

	public PooledGameObject[] objs = new PooledGameObject[0];
	Queue<int> activeIds = new Queue<int>();

	void SetUp (int poolsize){
		objs = new PooledGameObject[poolsize];
		for (int i = 0; i < poolsize; i++) {
			var inst = Instantiate (myObject, transform).GetComponent<PooledGameObject>();
			ExistingObjects += 1;
			inst.myId = i;
			inst.DisableObject ();
			ActiveObjects += 1;
			objs[i] = inst;
		}
	}

	void FillArrayWithObjects () {
		for (int i = 0; i < objs.Length; i++) {
			if (objs[i] == null) {
				var inst = Instantiate(myObject, transform).GetComponent<PooledGameObject>();
				ExistingObjects += 1;
				inst.myId = i;
				inst.DisableObject();
				ActiveObjects += 1;
				objs[i] = inst;
			}
		}
	}

	public PooledGameObject Spawn(int index) {
		if (index < objs.Length) {

			if (!objs[index].isActive) {
				objs[index].EnableObject();

				return objs[index];
			} else {
				return null;
			}
		} else {
			PooledGameObject[] temp = objs;
			objs = new PooledGameObject[Mathf.Min(objs.Length*2, maxPoolSize)];
			temp.CopyTo(objs, 0);
			var inst = Instantiate (myObject, transform).GetComponent<PooledGameObject>();
			ExistingObjects += 1;
			inst.EnableObject ();

			objs[temp.Length] =  inst;
			inst.myId = temp.Length;

			FillArrayWithObjects();
			poolSize = objs.Length;

			return inst;
		}
	}

	public PooledGameObject Spawn (){
		for (int i = 0; i < objs.Length; i++) {
			if (!objs [i].isActive) {

				objs [i].EnableObject ();

				if (!autoExpand)
					activeIds.Enqueue (i);
				
				return objs [i];
			}
		}
		Debug.Log ("Not enough pooled objects detected " + myObject.name);

		//there is no free object left
		if (autoExpand) {
			PooledGameObject[] temp = objs;
			objs = new PooledGameObject[Mathf.Min(objs.Length*2, maxPoolSize)];
			temp.CopyTo(objs, 0);
			var inst = Instantiate (myObject, transform).GetComponent<PooledGameObject>();
			ExistingObjects += 1;
			inst.EnableObject ();

			objs[temp.Length] =  inst;
			inst.myId = temp.Length;

			FillArrayWithObjects();
			poolSize = objs.Length;

			if (poolSize == maxPoolSize) {
				autoExpand = false;
			}
			
			return objs [temp.Length];
		} else {
			if (activeIds.Count > 0) {
				int toReuse = activeIds.Dequeue();
				activeIds.Enqueue(toReuse);
				objs[toReuse].BroadcastMessage("RemoveSelfFromTile");
				objs[toReuse].EnableObject();
				ActiveObjects -= 1;
				return objs[toReuse];
			} else {
				return null;
			}
		}
	}


	private List<int> objectDisableBuffer = new List<int>();
	private void Update() {
		if (objectDisableBuffer.Count > 0) {
			for (int i = 0; i < objectDisableBuffer.Count; i++) {
				if (!objs[objectDisableBuffer[i]].isActive) {
					objs[objectDisableBuffer[i]].DisableObject();
				}
			}

			objectDisableBuffer = new List<int>();
		}
	}


	public void DestroyPooledObject (int id){
		objectDisableBuffer.Add(id);
		objs[id].isActive = false;
		//objs[id].DisableObject();
	}

	public void DestroyAllPooledObjects() {
		for (int i = 0; i < objs.Length; i++) {
			if (objs[i].isActive) {
				DestroyPooledObject(i);
			}
		}
	}

	public List<PooledGameObject> GetActiveObjects() {
		List<PooledGameObject> active = new List<PooledGameObject>();
		for (int i = 0; i < objs.Length; i++) {
			if (objs[i].isActive) {
				active.Add(objs[i]);
			}
		}

		return active;
	}
}