using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class BeltItemGfxUpdateProcessor : JobComponentSystem {
    // This declares a new kind of job, which is a unit of work to do.
    // The job is declared as an IJobForEach<Translation, Rotation>,
    // meaning it will process all entities in the world that have both
    // Translation and Rotation components. Change it to process the component
    // types you want.
    //
    // The job is also tagged with the BurstCompile attribute, which means
    // that the Burst compiler will optimize it for the best performance.
    [BurstCompile]
    struct MovementSystemJob : IJobForEach<Translation, ItemMovement> {

        // the delta we want to move - calculated with slot distance + Time.deltatime
        public float deltaMovementWithDeltaTime;



        public void Execute (ref Translation translation, [ReadOnly] ref ItemMovement movData) {
            // Implement the work to perform for each entity here.
            // You should only access data that is local or that is a
            // field on this job. Note that the 'rotation' parameter is
            // marked as [ReadOnly], which means it cannot be modified,
            // but allows this job to run in parallel with other jobs
            // that want to read Rotation component data.
            // For example,
            //     translation.Value += mul(rotation.Value, new float3(0, 0, 1)) * deltaTime;

            // Calculate Vector3.MoveTowards ourselves:
            float3 direction = movData.targetWithOffset - translation.Value;
            float magnitude = length(direction);
            if (magnitude <= deltaMovementWithDeltaTime || magnitude == 0f) {
                translation.Value = movData.targetWithOffset;
            } else {
                translation.Value = translation.Value + direction / magnitude * deltaMovementWithDeltaTime;
            }
        }
    }

    protected override JobHandle OnUpdate (JobHandle inputDependencies) {

        var job = new MovementSystemJob();

        // Assign values to the fields on your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        //     job.deltaTime = UnityEngine.Time.deltaTime;

        job.deltaMovementWithDeltaTime = BeltObject.beltItemSlotDistance * BeltMaster.beltUpdatePerSecond * Time.DeltaTime;


        // Now that the job is set up, schedule it to be run. 
        return job.Schedule(this, inputDependencies);
    }

    /*[BurstCompile]
    struct ApplyItemPosToEntitiesJob : IJobForEach<ItemMovement> {

        [ReadOnly] public NativeArray<float3> positions;

        public EntityCommandBuffer.Concurrent EntityCommandBuffer;

        public void Execute (ref ItemMovement c0) {
            throw new System.NotImplementedException();
        }
    }

    public static void UpdateBeltItemPositions () {
        // Create a native array of a single float to store the result. This example waits for the job to complete for illustration purposes
        NativeArray<float3> positions = new NativeArray<float3>(BeltMaster.s.itemPool.poolSize, Allocator.TempJob);
        for (int i = 0; i < BeltMaster.s.itemPool.poolSize; i++) {
            positions[i] = new float3(
                BeltMaster.s.itemPool.objectPool[i].myRandomOffset.x + BeltMaster.s.itemPool.objectPool[i].mySlot.position.x,
                BeltMaster.s.itemPool.objectPool[i].myRandomOffset.y + BeltMaster.s.itemPool.objectPool[i].mySlot.position.y,
                0);
        }

        // Set up the job data
        ApplyItemPosToEntitiesJob jobData = new ApplyItemPosToEntitiesJob();
        jobData.positions = positions;

        // Schedule the job
        jobData.Schedule(BeltMaster.s.itemPool.poolSize, 64);

        // Free the memory allocated by the result array
        positions.Dispose();
    }*/
}