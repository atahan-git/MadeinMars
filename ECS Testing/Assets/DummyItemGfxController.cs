using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyItemGfxController : MonoBehaviour
{
    // This declares a new kind of job, which is a unit of work to do.
    // The job is declared as an IJobForEach<Translation, Rotation>,
    // meaning it will process all entities in the world that have both
    // Translation and Rotation components. Change it to process the component
    // types you want.
    //
    // The job is also tagged with the BurstCompile attribute, which means
    // that the Burst compiler will optimize it for the best performance.
    [BurstCompile]
    struct ItemDrawSystemJob : IJobParallelFor {
        // Add fields here that your job needs to do its work.
        // For example,
        //    public float deltaTime;

        [ReadOnly]
        public float2 camPos;
        [ReadOnly]
        public float renderDistance;
        [WriteOnly]
        public NativeArray<bool> pooledItemsToDestroy;
        [WriteOnly]
        public NativeArray<bool> pooledItemsToCreate;
        [ReadOnly]
        public NativeArray<float2> itemPositions;
        [ReadOnly]
        public NativeArray<int> itemEntityIds;

        public void Execute (int index) {
            // Implement the work to perform for each entity here.
            // You should only access data that is local or that is a
            // field on this job. Note that the 'rotation' parameter is
            // marked as [ReadOnly], which means it cannot be modified,
            // but allows this job to run in parallel with other jobs
            // that want to read Rotation component data.
            // For example,
            //     translation.Value += mul(rotation.Value, new float3(0, 0, 1)) * deltaTime;



            float xDiff = abs(itemPositions[index].x - camPos.x);
            float yDiff = abs(itemPositions[index].y - camPos.y);
            //float xDiff = 0;
            //float yDiff = 0;

            if (itemEntityIds[index] != -1) {
                if (xDiff > renderDistance || yDiff > renderDistance) {
                    pooledItemsToDestroy[index] = true;
                }
            } else {
                if (xDiff < renderDistance && yDiff < renderDistance) {
                    pooledItemsToCreate[index] = true;
                }
            }
            
        }
    }

    public Transform myCam;
    public float renderDistance = 20f;
    public ObjectPool myEntityPool;
    public DummyItemCreator creator;
    public DummyItemMover mover;
    public ObjectPool entityPool;

    public NativeArray<float2> itemPositions;
    public NativeArray<int> itemEntityIds;

    public float updatePerSecond = 4;

    public void Start () {
        itemPositions = new NativeArray<float2>(creator.maxItemCount, Allocator.Persistent);
        itemEntityIds = new NativeArray<int>(creator.maxItemCount, Allocator.Persistent);

        //StartCoroutine(UpdateItems());
    }

    IEnumerator UpdateItems () {
        while (true) {

            /*NativeArray<bool> pooledItemsToDestroy = new NativeArray<bool>(DummyItemMover.allItems.Count, Allocator.TempJob);
            NativeArray<bool> pooledItemsToCreate = new NativeArray<bool>(DummyItemMover.allItems.Count, Allocator.TempJob);

            var job = new ItemDrawSystemJob() {
                renderDistance = renderDistance,
                camPos = new float2(myCam.position.x, myCam.position.y),
                pooledItemsToCreate = pooledItemsToCreate,
                pooledItemsToDestroy = pooledItemsToDestroy,
                itemPositions = itemPositions,
                itemEntityIds = itemEntityIds
            };


            // Now that the job is set up, schedule it to be run. 
            JobHandle jobHandle = job.Schedule(DummyItemMover.allItems.Count, 64);

            //yield return new WaitForSeconds((1f / updatePerSecond) / 4f);

            jobHandle.Complete();

            for (int index = 0; index < DummyItemMover.allItems.Count; index++) {
                if (pooledItemsToCreate[index] == true) {
                    Vector3 targetPosWithOffset = DummyItemMover.allItems[index].targetPos + DummyItemMover.allItems[index].offset;
                    DummyItemMover.allItems[index].myEntityId = entityPool.Spawn(targetPosWithOffset, targetPosWithOffset);
                    itemEntityIds[index] = DummyItemMover.allItems[index].myEntityId;
                } else if (pooledItemsToDestroy[index] == true) {
                    entityPool.DestroyPooledObject(DummyItemMover.allItems[index].myEntityId);
                    DummyItemMover.allItems[index].myEntityId = -1;
                    itemEntityIds[index] = -1;
                }
            }

            pooledItemsToDestroy.Dispose();
            pooledItemsToCreate.Dispose();*/

            /*yield return new WaitForSeconds((1f / updatePerSecond)/2f);
            int halfPoint = DummyItemMover.allItems.Count / 2;
            mover.UpdateItems(0, halfPoint);

            yield return new WaitForSeconds((1f / updatePerSecond) / 2f);

            mover.UpdateItems(halfPoint+1, DummyItemMover.allItems.Count);*/

            yield return new WaitForSeconds((1f / updatePerSecond) / 4f);
        }
    }


    public void OnDestroy () {
        itemPositions.Dispose();
        itemEntityIds.Dispose();
    }
}