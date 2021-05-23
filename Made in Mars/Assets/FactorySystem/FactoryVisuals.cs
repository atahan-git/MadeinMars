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

    
    public GameObjectObjectPool buidingWorldObjectPool;
    public GameObjectObjectPool beltWorldObjectPool;
    public GameObjectObjectPool connectorWorldObjectPool;
    public GameObjectObjectPool droneWorldObjectPool;
    
    private void Awake() {
        s = this;
        FactoryBuilder.ObjectsUpdated += ObjectsCreationDeletionUpdate;
    }

    private void OnDestroy() {
        FactoryBuilder.ObjectsUpdated -= ObjectsCreationDeletionUpdate;
        s = null;
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
                if (tile.areThereWorldObject) {
                    // We assume that if there is a world object, then it must be a belt world object.
                    // This is because world object deletions are handled through tile.objectUpdatedCallback
                    Assert.IsNotNull(tile.worldObject.GetComponent<BeltWorldObject>());
                    
                    var beltWorldObject = tile.worldObject.GetComponent<BeltWorldObject>();
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
                if (tile.areThereWorldObject) {
                    // We assume that if there is a world object, then it must be a belt world object.
                    // This is because world object deletions are handled through tile.objectUpdatedCallback
                    try {
                        Assert.IsNotNull(tile.worldObject.GetComponent<ConnectorWorldObject>());
                    } catch {
                        print(tile.location);
                    }
                    
                    var connectorWorldObject = tile.worldObject.GetComponent<ConnectorWorldObject>();
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
            if (tile.areThereWorldObject) {
                // We assume that if there is a world object, then it must be a belt world object.
                // This is because world object deletions are handled through tile.objectUpdatedCallback
                try {
                    Assert.IsNotNull(tile.worldObject.GetComponent<BuildingWorldObject>());
                } catch {
                    print(tile.location);
                }

                var buildingWorldObject = tile.worldObject.GetComponent<BuildingWorldObject>();
                buildingWorldObject.UpdateSelf(building);
            } else {
                buidingWorldObjectPool.Spawn().GetComponent<BuildingWorldObject>().UpdateSelf(building);
            }
        }
        
        
        var constructions = FactoryMaster.s.GetConstructions();
        for (int i = 0; i < constructions.Count; i++) {
            var construction = constructions[i];
            var tile = Grid.s.GetTile(construction.center);
            switch (construction.myData.myType) {
                case BuildingData.ItemType.Belt:
                    if (tile.areThereWorldObject) {
                        Assert.IsNotNull(tile.worldObject.GetComponent<BeltWorldObject>());

                        var beltWorldObject = tile.worldObject.GetComponent<BeltWorldObject>();
                        beltWorldObject.UpdateSelf(construction.center, construction);
                    } else {
                        beltWorldObjectPool.Spawn().GetComponent<BeltWorldObject>().UpdateSelf(construction.center, construction);
                    }

                    break;
                case BuildingData.ItemType.Connector:
                    if (tile.areThereWorldObject) {
                        // We assume that if there is a world object, then it must be a belt world object.
                        // This is because world object deletions are handled through tile.objectUpdatedCallback
                        Assert.IsNotNull(tile.worldObject.GetComponent<ConnectorWorldObject>());
                    
                        var connectorWorldObject = tile.worldObject.GetComponent<ConnectorWorldObject>();
                        connectorWorldObject.UpdateSelf(construction.center, construction);
                    } else {
                        connectorWorldObjectPool.Spawn().GetComponent<ConnectorWorldObject>().UpdateSelf(construction.center, construction);
                    }
                    break;
                default:
                    if (tile.areThereWorldObject) {
                        Assert.IsNotNull(tile.worldObject.GetComponent<BuildingWorldObject>());

                        var buildingWorldObject = tile.worldObject.GetComponent<BuildingWorldObject>();
                        buildingWorldObject.UpdateSelf(construction);
                    } else {
                        buidingWorldObjectPool.Spawn().GetComponent<BuildingWorldObject>().UpdateSelf(construction);
                    }

                    break;
            }
        }
        
        // Currently there are no ways to create or destroy drones, so we handle drone visuals creation by using CreateDroneVisuals()
        /*var drones = FactoryMaster.s.GetDrones();
        for (int i = 0; i < drones.Count; i++) {
            var drone = drones[i];
            if (tile.areThereWorldObject) {
                // We assume that if there is a world object, then it must be a belt world object.
                // This is because world object deletions are handled through tile.objectUpdatedCallback
                Assert.IsNotNull(tile.worldObject.GetComponent<BuildingWorldObject>());
                
                var buildingWorldObject = tile.worldObject.GetComponent<BuildingWorldObject>();
                buildingWorldObject.UpdateSelf(building);
            } else {
                buidingWorldObjectPool.Spawn().GetComponent<BuildingWorldObject>().UpdateSelf(building);
            }
        }*/
    }

    public void CreateDroneVisuals(Drone drone) {
        droneWorldObjectPool.Spawn().GetComponent<DroneWorldObject>().SetUp(drone);
    }


    public int lastBeltItemCount = 0;
    public void BeltVisualsUpdate () {
        int skipCount = Mathf.CeilToInt((float)lastBeltItemCount/ItemDrawSystem.s.maxBeltItems);
        skipCount = Mathf.Max(1, skipCount);
        lastBeltItemCount = 0;
        
        for (int i = 0; i < FactoryMaster.s.GetBelts().Count; i++) {
            var curBelt = FactoryMaster.s.GetBelts()[i];
            if (curBelt != null) {

                int n = 0;
                bool canMove = false;
                var dir = (new Position(0,0)-Position.GetCardinalDirection(curBelt.direction)).Vector3(Position.Type.item);
                //var start = (curBelt.endPos + Position.GetCardinalDirection(curBelt.direction)).Vector3(Position.Type.item);
                var start = (curBelt.endPos).Vector3(Position.Type.item) - dir *0.5f;
                
                for (int m = curBelt.items.Count-1; m >= 0; m--) {

                    if (curBelt.items[m].item.isEmpty()) {
                        n += curBelt.items[m].count;
                        canMove = true;
                    } else {
                        for (int k = 0; k < curBelt.items[m].count; k++) {
                            //if (lastBeltItemCount % skipCount == 0) {
                                var pos = start + (dir * (((float) n) / ((float) FactoryMaster.SlotPerSegment+1)));
                                pos = new Vector3(pos.x, pos.y, DataHolder.itemLayer);
                                ItemDrawSystem.s.SpawnBeltItem(curBelt.items[m].item, pos, curBelt.direction, canMove);
                            //}

                            lastBeltItemCount += 1;
                            //Debug.DrawLine(pos, pos+Vector3.up, Color.red, 1f/FactoryMaster.SimUpdatePerSecond);
                            n += 1;
                        }
                    }
                }
            }
        }
    }
    
    
    public void ConnectorVisualUpdate () {
        for (int i = 0; i < FactoryMaster.s.GetConnectors().Count; i++) {
            var curConnector = FactoryMaster.s.GetConnectors()[i];
            if (curConnector != null) {
                for (int job = 0; job < curConnector.itemTransportJobs.Length; job++) {
                    var curJob = curConnector.itemTransportJobs[job];
                    if (curJob.isMovingItem) {
                        ItemDrawSystem.s.SpawnConnectorItem(curJob.movingItem, curJob.itemCurPos.Vector3(Position.Type.item), curJob.itemDir, true);
                    }
                }
            }
        }
    }

}
