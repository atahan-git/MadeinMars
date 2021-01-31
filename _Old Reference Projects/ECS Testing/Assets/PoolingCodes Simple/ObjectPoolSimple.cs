using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ObjectPoolSimple<T> where T : IPoolableClass, new(){

	bool autoExpand = true;
	T[] objectPool;

	int poolSize = 50;
	int maxPoolSize = 10000;

	bool[] isActiveArray;

	Queue<int> activeIds = new Queue<int>();

	public ObjectPoolSimple (bool _autoExpand, int _poolSize, int _maxPoolSize) {
		autoExpand = _autoExpand;
		poolSize = _poolSize;
		maxPoolSize = _maxPoolSize;
		SetUp();
	}


	void SetUp () {
		isActiveArray = new bool[poolSize];
		objectPool = new T[poolSize];

		for (int i = 0; i < objectPool.Length; i++) {
			objectPool[i] = new T();
			objectPool[i].myId = i;

			isActiveArray[i] = false;
		}
	}

	bool ExpandPool () {
		if (poolSize == maxPoolSize) {
			Debug.LogError("Max Pool size reached");
			return false;
		}

		int newPoolSize = poolSize * 2;
		if (newPoolSize > maxPoolSize)
			newPoolSize = maxPoolSize;


		bool[] temp = isActiveArray;
		isActiveArray = new bool[newPoolSize];
		temp.CopyTo(isActiveArray, 0);

		T[] tempArray = objectPool;
		objectPool = new T[newPoolSize];
		tempArray.CopyTo(objectPool, 0);


		for (int i = poolSize; i < newPoolSize; i++) {

			objectPool[i] = new T();
			objectPool[i].myId = i;

			isActiveArray[i] = false;
	
		}
		poolSize = newPoolSize;

		return true;
	}


	T _Spawn () {
		for (int i = 0; i < isActiveArray.Length; i++) {
			if (!isActiveArray[i]) {
				isActiveArray[i] = true;

				if (!autoExpand)
					activeIds.Enqueue(i);

				return objectPool[i];
			}
		}
		Debug.Log("Not enough pooled objects detected");

		//there is no free object left
		if (autoExpand) {
			if (ExpandPool())
				return _Spawn();
			else
				return default;
		} else {
			int toReuse = activeIds.Dequeue();
			activeIds.Enqueue(toReuse);

			return objectPool[toReuse];
		}
	}





	public T Spawn () {
		return _Spawn();
	}


	public T GetObject (int id) {
		return objectPool[id];
	}

	public T[] GetObjectPool () {
		return objectPool;
	}

	/*public T[] GetActiveObjects () {

	}*/

	public void DestroyPooledObject (int id) {
		if (isActiveArray[id] != false) {
			isActiveArray[id] = false;
		} else {
			Debug.LogError("Pooled object with wrong id detected");
		}
	}
}


// Should start disabled!
public interface IPoolableClass {
	int myId { get; set; }
}