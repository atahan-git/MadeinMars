using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ObjectPoolSimple<T> where T : IPoolableClass, new() {

	T[] _objectPool;

	public T[] objectPool{get{ return _objectPool; }}

	public int poolSize = 50;
	public int maxPoolSize = 10000;

	bool[] isActiveArray;

	Queue<int> activeIds = new Queue<int>();

	public ObjectPoolSimple (int _poolSize, int _maxPoolSize) {
		poolSize = _poolSize;
		maxPoolSize = _maxPoolSize;
	}


	public void SetUp () {
		isActiveArray = new bool[poolSize];
		_objectPool = new T[poolSize];

		for (int i = 0; i < _objectPool.Length; i++) {
			_objectPool[i] = new T();
			_objectPool[i].myId = i;
			_objectPool[i].Setup();

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

		T[] tempArray = _objectPool;
		_objectPool = new T[newPoolSize];
		tempArray.CopyTo(_objectPool, 0);


		for (int i = poolSize; i < newPoolSize; i++) {

			_objectPool[i] = new T();
			_objectPool[i].myId = i;

			isActiveArray[i] = false;

		}
		poolSize = newPoolSize;

		return true;
	}


	T _Spawn () {
		for (int i = 0; i < isActiveArray.Length; i++) {
			if (!isActiveArray[i]) {
				isActiveArray[i] = true;

				return _objectPool[i];
			}
		}
		Debug.Log("Not enough pooled objects detected");

		//there is no free object left
		if (ExpandPool())
			return _Spawn();
		else {
			Debug.LogError("Can't Spawn more objects!");
			return default;
		}
	}





	public T Spawn () {
		return _Spawn();
	}


	/*public T[] GetActiveObjects () {

	}*/

	public void DestroyPooledObject (T item) {
		DestroyPooledObject(item.myId);
	}

	public void DestroyPooledObject (int id) {
		if (isActiveArray[id] != false) {
			isActiveArray[id] = false;
		} else {
			Debug.LogError("Pooled object with wrong id detected");
		}
	}
}


public interface IPoolableClass {
	int myId { get; set; }
	void Setup ();
}