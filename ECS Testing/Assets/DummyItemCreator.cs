using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;     // Translation
using Unity.Collections;    // NativeArray
using Unity.Rendering;
using Unity.Mathematics;


public class DummyItemCreator : MonoBehaviour {


    public ObjectPool myPool;
    public ObjectPoolSimple<DummyItem> myObjPool;

    public DummyItemGfxController gfxs;

    public float itemSpawnPerSecond = 2f;
    public float itemSpawnedEachTime = 1000f;
    // Start is called before the first frame update

    public int maxItemCount = 40000;
    public int curItemCount = 0;
    void Start () {

        myObjPool = new ObjectPoolSimple<DummyItem>(true, 1000, int.MaxValue);

        StartCoroutine(CreateNewItem());
    }

    IEnumerator CreateNewItem () {
        yield return null;
        while (curItemCount < maxItemCount) {
            for (int i = 0; i < itemSpawnedEachTime; i++) {
                DummyItem newItem = myObjPool.Spawn();
                DummyItemMover.allItems.Add(newItem);


                Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), 0);
                newItem.offset = randomOffset;

                newItem.myEntityId =  myPool.Spawn(newItem.targetPos + randomOffset, newItem.targetPos + randomOffset );
                gfxs.itemEntityIds[DummyItemMover.allItems.Count - 1] = newItem.myEntityId;

                curItemCount++;

                if (curItemCount >= maxItemCount) {
                    //Debug.Break(); for showcase purposes lol
                    break;
                }
            }
            yield return new WaitForSeconds(1f / itemSpawnPerSecond);
        }

        //DummyItemMover.moveItems = false;
    }
}


public class DummyItem :IPoolableClass{
    public Vector3 targetPos;
    public Vector3 offset;
    public int myEntityId;

    public int myId { get; set; }
}