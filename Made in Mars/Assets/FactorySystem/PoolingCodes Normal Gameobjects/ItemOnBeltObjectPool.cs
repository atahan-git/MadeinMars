using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

		StartCoroutine(SetUp (poolSize));
	}

	public ItemOnBeltObject[] objs;
	Queue<int> activeIds = new Queue<int>();


	IEnumerator SetUp (int poolsize){
		objs = new ItemOnBeltObject[poolsize];
		for (int i = 0; i < poolsize; i++) {
			var inst = Instantiate (myObject, transform).GetComponent<ItemOnBeltObject>();
			ExistingObjects += 1;
			inst.myId = i;
			inst.DisableObject ();
			ActiveObjects += 1;
			objs[i] = inst;
			if(i%100 ==0)
				yield return 0;
		}
	}

	IEnumerator FillArrayWithObjects () {
		for (int i = 0; i < objs.Length; i++) {
			if (objs[i] == null) {
				var inst = Instantiate(myObject, transform).GetComponent<ItemOnBeltObject>();
				ExistingObjects += 1;
				inst.myId = i;
				inst.DisableObject();
				ActiveObjects += 1;
				objs[i] = inst;
				if (i % 100 == 0)
					yield return 0;
			}
		}
	}


	public ItemOnBeltObject Spawn (Vector3 pos, Sprite sprite, int direction, bool isBeltObject){
		for (int i = 0; i < objs.Length; i++) {
			if (objs[i] == null) {
				var inst = Instantiate(myObject, transform).GetComponent<ItemOnBeltObject>();
				ExistingObjects += 1;
				inst.myId = i;
				inst.DisableObject();
				ActiveObjects += 1;
				objs[i] = inst;
			}
			
			if (!objs [i].isActive) {

				objs [i].EnableObject (pos, sprite, direction, isBeltObject);

				if (!autoExpand)
					activeIds.Enqueue (i);
				
				return objs [i];
			}
		}
		print ("Not enough pooled objects detected");

		//there is no free object left
		if (autoExpand) {
			ItemOnBeltObject[] temp = objs;
			objs = new ItemOnBeltObject[Mathf.Min(objs.Length*2, maxPoolSize)];
			temp.CopyTo(objs, 0);
			var inst = Instantiate (myObject, transform).GetComponent<ItemOnBeltObject>();
			ExistingObjects += 1;
			inst.transform.position = pos;
			inst.EnableObject (pos, sprite, direction,isBeltObject);

			objs[temp.Length] =  inst;
			inst.myId = temp.Length;

			StopAllCoroutines();
			StartCoroutine(FillArrayWithObjects());
			poolSize = objs.Length;

			if (poolSize == maxPoolSize) {
				autoExpand = false;
			}
			
			return objs [temp.Length];
		} else {
			int toReuse = activeIds.Dequeue ();
			activeIds.Enqueue (toReuse);

			objs [toReuse].transform.position = pos;
			objs [toReuse].EnableObject (pos, sprite, direction, isBeltObject);
			ActiveObjects -= 1;
			return objs [toReuse];
		}
	}
	

	/*public GameObject Spawn (){
		return Spawn (myObject.transform.center, myObject.transform.rotation);
	}*/



	public void DestroyPooledObject (int id){
		objs[id].DisableObject();
	}
}