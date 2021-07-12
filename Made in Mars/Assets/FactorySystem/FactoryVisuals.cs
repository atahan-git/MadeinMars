using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class FactoryVisuals : MonoBehaviour {
    public static FactoryVisuals s;
    
    public Sprite[] beltSprites;
    public Sprite[] connectorSprites;
    public Sprite[] connectorSpritesBases;
    public Sprite[] connectorPullerSprites;
    
    public GameObjectObjectPool buildingWorldObjectPool;
    public GameObjectObjectPool beltWorldObjectPool;
    public GameObjectObjectPool connectorWorldObjectPool;
    public GameObjectObjectPool droneWorldObjectPool;
    
    private void Awake() {
        s = this;
        FactoryBuilder.ObjectsUpdated += ObjectsCreationDeletionUpdate;
        FactoryBuilder.DronesUpdated += DroneCreationDeletionUpdate;
        GameMaster.CallWhenClearPlanet(ClearAll);
    }

    private void OnDestroy() {
        FactoryBuilder.ObjectsUpdated -= ObjectsCreationDeletionUpdate;
        FactoryBuilder.DronesUpdated -= DroneCreationDeletionUpdate;
        GameMaster.RemoveFromCall(ClearAll);
        s = null;
    }

    void ClearAll() {
        buildingWorldObjectPool.DestroyAllPooledObjects();
        beltWorldObjectPool.DestroyAllPooledObjects();
        connectorWorldObjectPool.DestroyAllPooledObjects();
        droneWorldObjectPool.DestroyAllPooledObjects();
    }


    public void ObjectsCreationDeletionUpdate() {
        if(!FactoryMaster.s.isSimStarted)
            return;


        var belts = FactoryMaster.s.GetBelts();
        for (int i = 0; i < belts.Count; i++) {
            var belt = belts[i];
            for (int n = 0; n < belt.length; n++) {
                var location = Position.MoveTowards(belt.startPos, belt.endPos, n);
                var tile = Grid.s.GetTile(location);
                if (tile.areThereVisualObject) {
                    // We assume that if there is a world object, then it must be a belt world object.
                    // This is because world object deletions are handled through tile.objectUpdatedCallback
                    Assert.IsNotNull(tile.visualObject.GetComponent<BeltWorldObject>());
                    
                    var beltWorldObject = tile.visualObject.GetComponent<BeltWorldObject>();
                    beltWorldObject.UpdateSelf(location, belt);
                } else {
                    beltWorldObjectPool.Spawn().GetComponent<BeltWorldObject>().UpdateSelf(location, belt);
                }
            }
        }
        
        
        
        var connectors = FactoryMaster.s.GetConnectors();
        for (int i = 0; i < connectors.Count; i++) {
            var connector = connectors[i];
            for (int n = 0; n < connector.length; n++) {
                var location = Position.MoveTowards(connector.startPos, connector.endPos, n);
                var tile = Grid.s.GetTile(location);
                if (tile.areThereVisualObject) {
                    // We assume that if there is a world object, then it must be a belt world object.
                    // This is because world object deletions are handled through tile.objectUpdatedCallback
                    try {
                        Assert.IsNotNull(tile.visualObject.GetComponent<ConnectorWorldObject>());
                    } catch {
                        print(tile.location);
                    }
                    
                    var connectorWorldObject = tile.visualObject.GetComponent<ConnectorWorldObject>();
                    connectorWorldObject.UpdateSelf(location, connector);
                } else {
                    connectorWorldObjectPool.Spawn().GetComponent<ConnectorWorldObject>().UpdateSelf(location, connector);
                }
            }
        }
        
        var buildings = FactoryMaster.s.GetBuildings();
        for (int i = 0; i < buildings.Count; i++) {
            var building = buildings[i];
            var tile = Grid.s.GetTile(building.center);
            if (tile.areThereVisualObject) {
                // We assume that if there is a world object, then it must be a belt world object.
                // This is because world object deletions are handled through tile.objectUpdatedCallback
                try {
                    Assert.IsNotNull(tile.visualObject.GetComponent<BuildingWorldObject>());
                } catch {
                    print(tile.location);
                }

                var buildingWorldObject = tile.visualObject.GetComponent<BuildingWorldObject>();
                buildingWorldObject.UpdateSelf(building);
            } else {
                buildingWorldObjectPool.Spawn().GetComponent<BuildingWorldObject>().UpdateSelf(building);
            }
        }
        
        
        var constructions = FactoryMaster.s.GetConstructions();
        for (int i = 0; i < constructions.Count; i++) {
            var construction = constructions[i];
            var tile = Grid.s.GetTile(construction.center);
            switch (construction.myData.myType) {
                case BuildingData.ItemType.Belt:
                    if (tile.areThereVisualObject) {
                        Assert.IsNotNull(tile.visualObject.GetComponent<BeltWorldObject>());

                        var beltWorldObject = tile.visualObject.GetComponent<BeltWorldObject>();
                        beltWorldObject.UpdateSelf(construction.center, construction);
                    } else {
                        beltWorldObjectPool.Spawn().GetComponent<BeltWorldObject>().UpdateSelf(construction.center, construction);
                    }

                    break;
                case BuildingData.ItemType.Connector:
                    if (tile.areThereVisualObject) {
                        // We assume that if there is a world object, then it must be a belt world object.
                        // This is because world object deletions are handled through tile.objectUpdatedCallback
                        Assert.IsNotNull(tile.visualObject.GetComponent<ConnectorWorldObject>());
                    
                        var connectorWorldObject = tile.visualObject.GetComponent<ConnectorWorldObject>();
                        connectorWorldObject.UpdateSelf(construction.center, construction);
                    } else {
                        connectorWorldObjectPool.Spawn().GetComponent<ConnectorWorldObject>().UpdateSelf(construction.center, construction);
                    }
                    break;
                default:
                    if (tile.areThereVisualObject) {
                        Assert.IsNotNull(tile.visualObject.GetComponent<BuildingWorldObject>());

                        var buildingWorldObject = tile.visualObject.GetComponent<BuildingWorldObject>();
                        buildingWorldObject.UpdateSelf(construction);
                    } else {
                        buildingWorldObjectPool.Spawn().GetComponent<BuildingWorldObject>().UpdateSelf(construction);
                    }

                    break;
            }
        }
    }

    public void DroneCreationDeletionUpdate() {
        droneWorldObjectPool.DestroyAllPooledObjects();

        for (int i = 0; i < FactoryMaster.s.GetDrones().Count; i++) {
            var drone = FactoryMaster.s.GetDrones()[i];
            droneWorldObjectPool.Spawn().GetComponent<DroneWorldObject>().SetUp(drone);
        }
    }


    public int lastSpawnedItemCount = 0;
    public void UpdateVisualItems() {
        ItemDrawSystem.s.ResetPoolIndex();
        int skipCount = Mathf.CeilToInt((float)lastSpawnedItemCount/ItemDrawSystem.s.maxBeltItems);
        skipCount = Mathf.Max(1, skipCount);
        if (skipCount > 1) {
            ItemDrawSystem.s.ExpandPool();
        }

        lastSpawnedItemCount = 0;
        BeltVisualsUpdate(skipCount);
        ConnectorVisualUpdate(skipCount);
    }
    public void BeltVisualsUpdate (int skipCount) {
        
        for (int i = 0; i < FactoryMaster.s.GetBelts().Count; i++) {
            var curBelt = FactoryMaster.s.GetBelts()[i];
            if (curBelt != null) {

                int n = 0;
                bool canMove = false;
                var dir = (new Position(0,0)-Position.GetCardinalDirection(curBelt.direction)).Vector3(Position.Type.item);
                var start = (curBelt.endPos).Vector3(Position.Type.item) - dir *0.5f;
                
                for (int m = curBelt.items.Count-1; m >= 0; m--) {

                    if (curBelt.items[m].item.isEmpty()) {
                        n += curBelt.items[m].count;
                        canMove = true;
                    } else {
                        for (int k = 0; k < curBelt.items[m].count; k++) {
                            if (lastSpawnedItemCount % skipCount == 0) {
                                var pos = start + (dir * (((float) n) / ((float) FactoryMaster.SlotPerSegment+1)));
                                pos = new Vector3(pos.x, pos.y, DataHolder.itemLayer);
                                ItemDrawSystem.s.SpawnBeltItem(curBelt.items[m].item, pos, curBelt.direction, canMove);
                            }

                            lastSpawnedItemCount += 1;
                            n += 1;
                        }
                    }
                }
            }
        }
    }
    
    
    public void ConnectorVisualUpdate (int skipCount) {
        int lastConnectorItemCount = 0;
        for (int i = 0; i < FactoryMaster.s.GetConnectors().Count; i++) {
            var curConnector = FactoryMaster.s.GetConnectors()[i];
            if (curConnector != null) {
                for (int job = 0; job < curConnector.itemTransportJobs.Length; job++) {
                    var curJob = curConnector.itemTransportJobs[job];
                    if (curJob.isMovingItem) {
                        if (lastSpawnedItemCount % skipCount == 0) {
                            ItemDrawSystem.s.SpawnConnectorItem(curJob.movingItem, curJob.itemCurPos.Vector3(Position.Type.item), curJob.itemDir, true);
                        }
                        lastSpawnedItemCount++;
                    }
                }
            }
        }
    }
}