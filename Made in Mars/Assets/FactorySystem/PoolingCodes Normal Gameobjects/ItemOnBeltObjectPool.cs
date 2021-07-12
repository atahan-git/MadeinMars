using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class ItemOnBeltObjectPool : MonoBehaviour {

	public bool autoExpand = true; //dont change this at runtime
	public GameObject myObject;
	public int poolSize = 50;
	public int maxPoolSize = 3000;
	[Space]
	public int ExistingObjects;
	public int ActiveObjects;

	void Start (){
		if (myObject.GetComponent<ItemOnBeltObject> () == null)
			myObject.AddComponent<ItemOnBeltObject> ();

		myObject.GetComponent<ItemOnBeltObject> ().myPool = this;

		SetUp (poolSize);
	}

	public ItemOnBeltObject[] objs;
	Queue<int> activeIds = new Queue<int>();


	void SetUp (int poolsize){
		objs = new ItemOnBeltObject[poolsize];
		for (int i = 0; i < poolsize; i++) {
			var inst = Instantiate (myObject, transform).GetComponent<ItemOnBeltObject>();
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
				var inst = Instantiate(myObject, transform).GetComponent<ItemOnBeltObject>();
				ExistingObjects += 1;
				inst.myId = i;
				inst.DisableObject();
				ActiveObjects += 1;
				objs[i] = inst;
			}
		}
	}

	public void ResetIndex() {
		index = 0;
	}

	public int index = 0;
	public bool canExpand = true;

	public void ExpandPool() {
		if (canExpand) {
			ItemOnBeltObject[] temp = objs;
			objs = new ItemOnBeltObject[Mathf.Min(objs.Length * 2, maxPoolSize)];
			temp.CopyTo(objs, 0);

			FillArrayWithObjects();
			poolSize = objs.Length;

			if (poolSize == maxPoolSize) {
				autoExpand = false;
			}
		}
	}


	public ItemOnBeltObject Spawn (Vector3 pos, Sprite sprite, int direction, bool isBeltObject){
		if (index < objs.Length) {
			int i = index;
			objs[i].EnableObject(pos, sprite, direction, isBeltObject);

			index++;
			return objs[i];
		} else {
			return null;
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

			objectDisableBuffer.Clear();
		}
	}


	public void DestroyPooledObject (int id){
		objectDisableBuffer.Add(id);
		objs[id].isActive = false;
		//objs[id].DisableObject();
	}
}