using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;
using System;

public class BeltMasterMatrix : MonoBehaviour {

    private float[,,] beltMatrix;
    private GameObject[,] Items;
    private Vector3[,] ItemsTargetPos;

    public static float beltDistance = 10f;
    public static int beltUpdatePerSecond = 4;
    //index - z vals
    /* 0 - dir
     * 1 - target dest x
     * 2 - target dest y
     * 3 - itemid
     * 4 - isprocessedmovedflag
     */

    // Start is called before the first frame update
    void Start() {
        beltMatrix = new float[3,3,5];

        for (int x = 0; x < 3; x++) {
            for (int y = 0; y < 3; y++) {
                if (x == 0 && y == 1)
                    beltMatrix[x, y, 0] = 2;
                if (x == 1 && y == 0)
                    beltMatrix[x, y, 0] = 2;
                if (x == 1 && y == 1)
                    beltMatrix[x, y, 0] = 1;
                if (x == 2 && y == 0)
                    beltMatrix[x, y, 0] = 3;
                if (x == 2 && y == 1)
                    beltMatrix[x, y, 0] = 3;
            }
        }
        Items = new GameObject[beltMatrix.GetLength(0),beltMatrix.GetLength(1)];
        ItemsTargetPos = new Vector3[beltMatrix.GetLength(0),beltMatrix.GetLength(1)];

        InitializeViewMatrix();
    }

    public GameObject CellPrefab;
    public GameObject TextPrefab;
    public Transform CellParent;

    public GridLayoutGroup grid;

    private Text[,,] myTexts;
    void InitializeViewMatrix() {
        myTexts=new Text[beltMatrix.GetLength(0),beltMatrix.GetLength(1),beltMatrix.GetLength(2)];
        grid.constraintCount = beltMatrix.GetLength(0);
        grid.cellSize = Vector2.one*500f / beltMatrix.GetLength(0);
        
        for (int y = 0; y < beltMatrix.GetLength(1); y++) {
            for (int x = 0; x < beltMatrix.GetLength(0); x++) {
                var obj = Instantiate(CellPrefab, CellParent);
                beltMatrix[x, y, 1] = obj.transform.position.x;
                beltMatrix[x, y, 2] = obj.transform.position.y;
                for (int i = 0; i < beltMatrix.GetLength(2); i++) {
                    var text = Instantiate(TextPrefab, obj.transform);
                    myTexts[x, y, i] = text.GetComponent<Text>();
                }
            }
        }
    }

    // Update is called once per frame
    void Update() {
        for (int x = 0; x < beltMatrix.GetLength(0); x++) {
            for (int y = 0; y < beltMatrix.GetLength(1); y++) {
                for (int i = 0; i < beltMatrix.GetLength(2); i++) {
                    myTexts[x, y, i].text = beltMatrix[x, y, i].ToString("F2");
                }
            }
        }
    }

    void ProcessMatrixUpdate() {
        for (int x = 0; x < beltMatrix.GetLength(0); x++) {
            for (int y = 0; y < beltMatrix.GetLength(1); y++) {
                if (beltMatrix[x, y, 3] > 0) {
                    ShiftCoordinates(x,y,beltMatrix[x,y,0], out int x_out, out int y_out);
                    if (beltMatrix[x_out, y_out, 3] == 0) {
                        beltMatrix[x_out, y_out, 3] = beltMatrix[x, y, 3];
                        beltMatrix[x, y, 3] = 0;
                        beltMatrix[x_out, y_out, 4] = 1f;
                        Items[x_out, y_out] = Items[x, y];
                        Items[x, y] = null;
                    }
                }
            }
        }
    }

    void ShiftCoordinates(int x, int y, float dir, out int x_out, out int y_out) {
        switch (dir) {
            case 1:
                x_out = x;
                y_out = y - 1;
                break;
            case 2:
                x_out = x + 1;
                y_out = y;
                break;
            case 3:
                x_out = x;
                y_out = y + 1;
                break;
            case 4:
                x_out = x - 1;
                y_out = y;
                break;
            default:
                x_out = x;
                y_out = y;
                break;
        }
    }


    void ProcessGfxUpdate() {
        
    }
}

/*
[Serializable]
public struct ItemMatrixPos : IComponentData {
    // the target we want to move towards (offset included)
    public int x,y;
}

/*[Serializable]
public struct ItemID : IComponentData {
    public ushort myItemId;
}*


public class MatrixMovementProcessor : JobComponentSystem {

    // This declares a new kind of job, which is a unit of work to do.
    // The job is declared as an IJobForEach<Translation, Rotation>,
    // meaning it will process all entities in the world that have both
    // Translation and Rotation components. Change it to process the component
    // types you want.
    //
    // The job is also tagged with the BurstCompile attribute, which means
    // that the Burst compiler will optimize it for the best performance.
    [BurstCompile]
    struct MovementSystemJob : IJobForEach<Translation, ItemMatrixPos> {

        // the delta we want to move - calculated with slot distance + Time.deltatime
        [ReadOnly]
        public float deltaMovementWithDeltaTime;
        [ReadOnly]
        public float[,,] beltMatrix;
        
        public void Execute(ref Translation translation, [ReadOnly] ref ItemMatrixPos posData) {
            // Implement the work to perform for each entity here.
            // You should only access data that is local or that is a
            // field on this job. Note that the 'rotation' parameter is
            // marked as [ReadOnly], which means it cannot be modified,
            // but allows this job to run in parallel with other jobs
            // that want to read Rotation component data.
            // For example,
            //     translation.Value += mul(rotation.Value, new float3(0, 0, 1)) * deltaTime;

            // Calculate Vector3.MoveTowards ourselves:
            float3 direction = float3(beltMatrix[posData.x,posData.y,3],beltMatrix[posData.x,posData.y,4],0)  - translation.Value;
            float magnitude = length(direction);
            if (magnitude <= deltaMovementWithDeltaTime || magnitude == 0f) {
                translation.Value = float3(beltMatrix[posData.x,posData.y,3],beltMatrix[posData.x,posData.y,4],0);
            } else {
                translation.Value = translation.Value + direction / magnitude * deltaMovementWithDeltaTime;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies) {

        var job = new MovementSystemJob();

        // Assign values to the fields on your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        //     job.deltaTime = UnityEngine.Time.deltaTime;

        job.deltaMovementWithDeltaTime = BeltMasterMatrix.beltDistance * BeltMasterMatrix.beltUpdatePerSecond * Time.DeltaTime;
        job.beltMatrix = BeltMasterMatrix.beltMatrix;


        // Now that the job is set up, schedule it to be run. 
        return job.Schedule(this, inputDependencies);
    }
}*/
