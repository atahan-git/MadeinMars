using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryVisuals : MonoBehaviour {

    public static FactoryVisuals s;
    
    public Sprite[] beltSprites;
    public Sprite[] connectorSprites;
    public Sprite[] connectorSpritesBases;
    public Sprite[] connectorPullerSprites;

    public List<VisualBuilding> allBuildings = new List<VisualBuilding>();
    public List<VisualBelt> allBelts = new List<VisualBelt>();
    public List<VisualConnector> allConnectors = new List<VisualConnector>();
    
    
    public class VisualBelt {
        public Belt functionalBelt;
        public List<GameObject> physicalBelts;
    }
    
    public class VisualBuilding {
        public Building functionalBuilding;
        public List<GameObject> physicalBuildings;
    }
    
    public class VisualConnector {
        public Connector functionalConnector;
        public List<GameObject> physicalConnectors;
    }


    private void Awake() {
        s = this;
    }

    
    public void BeltVisualsUpdate () {
        for (int i = 0; i < FactorySystem.s.belts.Count; i++) {
            var curBelt = FactorySystem.s.belts[i];
            if (curBelt != null) {

                int n = 0;
                bool canMove = false;
                var dir = (new Position(0,0)-Position.GetCardinalDirection(curBelt.direction)).Vector3(Position.Type.item);
                var start = (curBelt.endPos + Position.GetCardinalDirection(curBelt.direction)).Vector3(Position.Type.item);
                
                for (int m = curBelt.items.Count-1; m >= 0; m--) {

                    if (curBelt.items[m].item.isEmpty()) {
                        n += curBelt.items[m].count;
                        canMove = true;
                    } else {
                        for (int k = 0; k < curBelt.items[m].count; k++) {
                            var pos = start + (dir *(((float)n)/((float)FactorySystem.SlotPerSegment)));
                            pos = new Vector3(pos.x, pos.y, DataHolder.itemLayer);
                            ItemDrawSystem.s.SpawnBeltItem(curBelt.items[m].item, pos,  curBelt.direction, canMove);
                            //Debug.DrawLine(pos, pos+Vector3.up, Color.red, 1f/FactoryMaster.SimUpdatePerSecond);
                            n += 1;
                        }
                    }
                }
            }
        }
    }
    
    
    public void ConnectorVisualUpdate () {
        for (int i = 0; i < FactorySystem.s.connectors.Count; i++) {
            var curConnector = FactorySystem.s.connectors[i];
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
