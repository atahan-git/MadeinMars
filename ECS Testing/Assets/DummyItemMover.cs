using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;     // Translation
using Unity.Collections;    // NativeArray
using Unity.Rendering;

public class DummyItemMover : MonoBehaviour {
    public static DummyItemMover s;
    public static List<DummyItem> allItems = new List<DummyItem>();
    public static bool moveItems = true;

    public ObjectPool myPool;

    public float updatePerSecond = 4f;

    public float movementMultiplier = 0.1f;

    public DummyItemGfxController gfxs;

    // Start is called before the first frame update
    void Start () {
        s = this;

        
        cycleMoveVectors = new Vector3[] {
            Vector3.right * movementMultiplier,
            Vector3.right * movementMultiplier
            //Vector3.up* movementMultiplier,
            //-Vector3.up* movementMultiplier
        };

        StartCoroutine(UpdateItems());
    }

    public Transform camTransform;

    public int renderDistance = 20;
    int cycle = 0;
    Vector3[] cycleMoveVectors;

    // 100ms
    IEnumerator UpdateItems () {
        yield return new WaitForSeconds((1f / updatePerSecond)/2f);
        
        while (moveItems) {

            yield return new WaitForSeconds((1f / updatePerSecond) / 2f);

            int halfPoint = allItems.Count / 2;

            UpdateItems(0, halfPoint);

            yield return new WaitForSeconds((1f / updatePerSecond) / 2f);

            UpdateItems(halfPoint + 1, allItems.Count);
        }

        //Debug.Log("Item movement stopped");
    }

    void UpdateItems (int startIndex, int endIndex) {
        cycle = Random.Range(0, cycleMoveVectors.Length);
        for (int index = startIndex; index < endIndex; index++) {
            if (allItems[index] != null) {
                allItems[index].targetPos += cycleMoveVectors[cycle];
                gfxs.itemPositions[index] = new Unity.Mathematics.float2(allItems[index].targetPos.x, allItems[index].targetPos.y);


                /*float xDiff = System.Math.Abs(allItems[index].targetPos.x - camTransform.position.x);
                float yDiff = System.Math.Abs(allItems[index].targetPos.y - camTransform.position.y);*/

                if (allItems[index].myEntityId != -1) {

                    myPool.EditPooledObject(allItems[index].myEntityId, allItems[index].targetPos + allItems[index].offset);

                    /*if (xDiff > renderDistance || yDiff > renderDistance) {
                        myPool.DestroyPooledObject(allItems[index].myEntityId);
                        allItems[index].myEntityId = -1;
                    }*/
                } else {
                    /*if (xDiff < renderDistance && yDiff < renderDistance) {
                        Vector3 targetPosWithOffset = allItems[index].targetPos + allItems[index].offset;
                        allItems[index].myEntityId = myPool.Spawn(targetPosWithOffset, targetPosWithOffset);
                    }*/
                }

                cycle += 1;
                cycle %= cycleMoveVectors.Length;
            }
        }
    }
}
