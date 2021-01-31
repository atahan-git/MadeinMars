using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPool : MonoBehaviour {

	public bool autoExpand = true; //dont change this at runtime
	public GameObject myObject;
	public int poolSize = 50;
	[Space]
	public int ExistingObjects;
	public int ActiveObjects;

	void Start (){
		if (myObject.GetComponent<PooledObject> () == null)
			myObject.AddComponent<PooledObject> ();

		myObject.GetComponent<PooledObject> ().myPool = this;

		StartCoroutine(SetUp (poolSize));
	}

	GameObject[] objs;
	Queue<int> activeIds = new Queue<int>();


	IEnumerator SetUp (int poolsize){
		objs = new GameObject[poolsize];
		for (int i = 0; i < poolsize; i++) {
			GameObject inst = (GameObject)Instantiate (myObject, transform);
			ExistingObjects += 1;
			inst.GetComponent<PooledObject> ().myId = i;
			inst.GetComponent<PooledObject> ().DisableObject ();
			ActiveObjects += 1;
			objs[i] = (inst);
			if(i%10 ==0)
			yield return 0;
		}
	}

	IEnumerator FillArrayWithObjects () {
		for (int i = 0; i < objs.Length; i++) {
			if (objs[i] == null) {
				GameObject inst = (GameObject)Instantiate(myObject, transform);
				ExistingObjects += 1;
				inst.GetComponent<PooledObject>().myId = i;
				inst.GetComponent<PooledObject>().DisableObject();
				ActiveObjects += 1;
				objs[i] = (inst);
				if (i % 10 == 0)
					yield return 0;
			}
		}
	}


	GameObject _Spawn (Vector3 pos, Quaternion rot){
		for (int i = 0; i < objs.Length; i++) {
			if (!objs [i].GetComponent<PooledObject>().isActive) {

				objs [i].transform.position = pos;
				objs [i].transform.rotation = rot;
				objs [i].GetComponent<PooledObject> ().EnableObject ();

				if (!autoExpand)
					activeIds.Enqueue (i);
				
				return objs [i];
			}
		}
		print ("Not enough pooled objects detected");

		//there is no free object left
		if (autoExpand) {
			GameObject[] temp = objs;
			objs = new GameObject[objs.Length*2];
			temp.CopyTo(objs, 0);
			GameObject inst = (GameObject)Instantiate (myObject, transform);
			ExistingObjects += 1;
			inst.transform.position = pos;
			inst.transform.rotation = rot;
			inst.GetComponent<PooledObject>().EnableObject();

			objs[temp.Length] =  (inst);
			inst.GetComponent<PooledObject> ().myId = temp.Length;

			StopAllCoroutines();
			StartCoroutine(FillArrayWithObjects());
			poolSize = objs.Length;
			return objs [temp.Length];
		} else {
			int toReuse = activeIds.Dequeue ();
			activeIds.Enqueue (toReuse);

			objs [toReuse].transform.position = pos;
			objs [toReuse].transform.rotation = rot;
			objs [toReuse].GetComponent<PooledObject> ().EnableObject ();
			ActiveObjects -= 1;
			return objs [toReuse];
		}
	}





	public GameObject Spawn(Vector3 pos, Quaternion rot){
		return _Spawn (pos, rot);
	}
		
		
	public GameObject Spawn (Vector3 pos){
		return Spawn (pos, Quaternion.identity);
	}
		

	public GameObject Spawn (float x, float y, float z){
		return Spawn (new Vector3 (x, y, z));
	}

	/*public GameObject Spawn (){
		return Spawn (myObject.transform.position, myObject.transform.rotation);
	}*/



	public void DestroyPooledObject (int id){
		if (objs[id] != null) {
			objs[id].GetComponent<PooledObject>().DisableObject();
		} else {
			Debug.LogError("Pooled object with wrong id detected");
		}
	}
}