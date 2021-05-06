using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemDrawSystem : MonoBehaviour {

    public static ItemDrawSystem s;

    private ObjectPool myPool;
    
    private void Awake() {
        s = this;
        myPool = GetComponent<ObjectPool>();
    }

    //public float randomScale = 0.1f;
    public void SpawnBeltItem(Item item, Vector3 position, int direction, bool isMoving) {
        //var randomness = new Vector3(Random.Range(-randomScale, randomScale), Random.Range(-randomScale, randomScale), Random.Range(-randomScale, randomScale));
        Vector3 pos;
        if (direction == 1) {
            pos = position + new Vector3( 0.5f, -0.5f / FactorySystem.SlotPerSegment, 0);
        }else if (direction == 2 ) {
            pos = position + new Vector3(-0.5f / FactorySystem.SlotPerSegment, 0.5f, 0);
        }else if (direction == 3) {
            pos = position+ new Vector3(0.5f, 1f + 0.5f / FactorySystem.SlotPerSegment, 0);
        } else if (direction == 4) {
            pos = position+ new Vector3(1+0.5f / FactorySystem.SlotPerSegment, 0.5f, 0);
        } else{
            pos = position;
        }
        
        
        var myItem = myPool.Spawn(pos, item.GetSprite(), isMoving? direction:0, true);
    }


    public Vector3[] connectorOffsets = new[] {
        new Vector3( 0.5f, 0, 0),
        new Vector3(1, 0.5f, 0),
        new Vector3(0.5f, 1f, 0),
        new Vector3(1, 0.5f, 0)
    };
    public void SpawnConnectorItem(Item item, Vector3 position, int direction, bool isMoving) {
        //var randomness = new Vector3(Random.Range(-randomScale, randomScale), Random.Range(-randomScale, randomScale), Random.Range(-randomScale, randomScale));
        Vector3 pos =position + new Vector3(0.5f,0.5f);

        var myItem = myPool.Spawn(pos, item.GetSprite(), isMoving? direction:0,false);
    }


    private void Update() {
        for (int i = 0; i < myPool.objs.Length; i++) {

            var obj = myPool.objs[i];
            if (obj != null) {
                if (obj.isActive) {
                    if (obj.lifetime >= 1f / FactoryMaster.SimUpdatePerSecond) {
                        obj.DestroyPooledObject();
                        continue;
                    }

                    if (obj.isBeltObject) {
                        obj.transform.position += Position.GetCardinalDirection(obj.direction).Vector3(0f) 
                            * (FactoryMaster.BeltLengthToMovePerSecond * Time.deltaTime) 
                            / FactorySystem.SlotPerSegment;
                    } else {
                        obj.transform.position += Position.GetCardinalDirection(obj.direction).Vector3(0f) 
                                                  * (FactoryMaster.BeltLengthToMovePerSecond * Time.deltaTime);
                    }

                    obj.lifetime += Time.deltaTime;
                }
            }
        }
    }
}
