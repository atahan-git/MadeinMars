using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;     // Translation
using Unity.Collections;    // NativeArray
using Unity.Rendering;
using Unity.Mathematics;


public class ObjectPool : MonoBehaviour {

	[SerializeField] private Mesh mesh;
	[SerializeField] private Material material;
	public int poolSize = 50;
	public int maxPoolSize = 10000;
	[Space]
	public int ActiveObjectsCount;

	EntityManager entityManager;

	EntityArchetype entityArchetype;

	NativeArray<Entity> allEntitiesArray;
	public bool[] isActiveArray;


	Queue<int> activeIds = new Queue<int>();

	void Start (){
		entityManager = World.Active.EntityManager;

		entityArchetype = entityManager.CreateArchetype(
			typeof(Translation), typeof(RenderMesh),  // Rendering
			typeof(LocalToWorld), // Coordinate conversion
			typeof(Disabled), // So that our pooled objects start disabled
			typeof(ItemMovement),
			//typeof(ItemID),
			typeof(Scale)
			) ;

		SetUp();
	}

	void SetUp () {
		isActiveArray = new bool[poolSize];
		allEntitiesArray = new NativeArray<Entity>(poolSize, Allocator.Persistent);
		entityManager.CreateEntity(entityArchetype, allEntitiesArray);

		for (int i = 0; i < allEntitiesArray.Length; i++) {
			Entity entity = allEntitiesArray[i];

			entityManager.SetSharedComponentData(entity, new RenderMesh {
				mesh = mesh,
				material = material,
			});

			entityManager.SetComponentData(entity, new Scale { Value = 0.45f });

			isActiveArray[i] = false;
		}
	}

	bool ExpandEntityArray () {
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

		NativeArray<Entity> tempArray1 = allEntitiesArray;
		NativeArray<Entity> tempArray2 = new NativeArray<Entity>(newPoolSize-poolSize, Allocator.Temp);
		entityManager.CreateEntity(entityArchetype, tempArray2);

		allEntitiesArray = new NativeArray<Entity>(newPoolSize, Allocator.Persistent);



		for (int i = 0; i < tempArray1.Length; i++) {
			allEntitiesArray[i] = tempArray1[i];

			if (i < tempArray2.Length) {
				Entity entity = tempArray2[i];

				entityManager.SetSharedComponentData(entity, new RenderMesh {
					mesh = mesh,
					material = material,
				});

				allEntitiesArray[i + poolSize] = entity;
				isActiveArray[i + poolSize] = false;
			}
		}

		tempArray1.Dispose();
		tempArray2.Dispose();
		poolSize = newPoolSize;

		return true;
	}


	public int Spawn (Vector3 pos, Vector3 componentData, Item item) {
		for (int i = 0; i < isActiveArray.Length; i++) {
			if (!isActiveArray[i]) {
				isActiveArray[i] = true;
				Material mat = item.GetMaterial();
				entityManager.SetComponentData(allEntitiesArray[i], new Translation { Value = pos });
				entityManager.SetComponentData(allEntitiesArray[i], new ItemMovement { targetWithOffset = componentData });
				entityManager.RemoveComponent<Disabled>(allEntitiesArray[i]);
				entityManager.SetSharedComponentData(allEntitiesArray[i], new RenderMesh {
					mesh = mesh,
					material = mat,
				});
				ActiveObjectsCount++;

				return i;
			}
		}
		
		print ("Not enough pooled objects detected");

		//there is no free object left
		if (ExpandEntityArray())
			return Spawn(pos, componentData, item);
		else {
			Debug.LogError("Can't Spawn more objects!");
			return -1;
		}
		
	}
		

	/*public GameObject Spawn (){
		return Spawn (myObject.transform.position, myObject.transform.rotation);
	}*/

	/// <summary>
	/// You shouldn't use this method unless absolutely necessary. Decoupling!
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public Entity GetEntity (int id) {
		return allEntitiesArray[id];
	}

	public void DestroyPooledObject (int id){
		if (isActiveArray[id] != false) {
			isActiveArray[id] = false;
			ActiveObjectsCount--;
			entityManager.AddComponent<Disabled>(allEntitiesArray[id]);
		} else {
			Debug.LogError("Pooled object with wrong id detected");
		}
	}

	private void OnDestroy () {
		allEntitiesArray.Dispose();
	}
}