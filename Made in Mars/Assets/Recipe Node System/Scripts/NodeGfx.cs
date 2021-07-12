using System;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


/// <summary>
/// The base class the other two (CraftingNodeGfx and ItemNodeGfx) extend upon
/// Holds the basic functionality shared across both
/// </summary>
public abstract class NodeGfx : MonoBehaviour {
    public RecipeTreeViewer myViewer;
    public Node myNode;
    
    public Transform leftPortParent;
    public Transform rightPortParent;

    public List<NodePortGfx> allPorts = new List<NodePortGfx>();

    
    
    /// <summary>
    /// Should be used to redraw specific node parts like the name or crafting type
    /// </summary>
    protected void ReDrawnNode(RecipeTreeViewer master, Node node) {
        myViewer = master;
        myNode = node;
        ReDrawPorts();
    }

    public void BeginClickConnect(NodePortGfx port) {
        if (NodeItemTreeMakerMaster.s != null) {
            NodeItemTreeMakerMaster.s.BeginClickConnect(this, port);
        }
    }

    public const float snapMult = 0.0515f/2f;
    public static float snapMultUI;
    public void PositionUpdated() {
        //Snapping is done by DragPanel.cs
        /*myNode.x = ((int) (transform.position.x*snapMult))/snapMult;
        myNode.y = ((int) (transform.position.y*snapMult))/snapMult;
        transform.position = new Vector3(myNode.x,myNode.y,0);*/
        myViewer.RescaleNodeArea();
        //myNode.pos = (transform as RectTransform).anchoredPosition;
    }


    public void OnDraggingNode() {
        foreach (var port in allPorts) {
            port.OnPositionUpdated();
            port.connectedPortGfx?.OnPositionUpdated();
        }

        if (NodeItemTreeMakerMaster.s) {
            NodeItemTreeMakerMaster.s.UpdateNodePosition(this, (transform as RectTransform).anchoredPosition);
        }
    }

    public void DeleteNode() {
        if (NodeItemTreeMakerMaster.s != null) {
            NodeItemTreeMakerMaster.s.DeleteNode(this);
        }
    }



    void ReDrawPorts() {
        int childCount = allPorts.Count;
        for (int i = 0; i < childCount; i++) {
            allPorts[i].GetComponent<PooledGameObject>().DestroyPooledObject();
        }

        allPorts.Clear();
        
        for (int i = 0; i < myNode.myAdapters.Count; i++) {
            var myAdapter = myNode.myAdapters[i];
            for (int n = 0; n < myAdapter.connections.Count; n++) {
                var connection = myAdapter.connections[n];

                var myPort = myAdapter.isLeftAdapter ? myViewer.leftPortPool.Spawn() : myViewer.rightPortPool.Spawn();
                myPort.transform.SetParent(myAdapter.isLeftAdapter? leftPortParent : rightPortParent);
                
                allPorts.Add(myPort.GetComponent<NodePortGfx>().Setup(this, myAdapter, connection, connection.count));
            }
            
            // add an extra port so that the user can generate extra connections
            var myPortExtra = myAdapter.isLeftAdapter ? myViewer.leftPortPool.Spawn() : myViewer.rightPortPool.Spawn();
            myPortExtra.transform.SetParent(myAdapter.isLeftAdapter? leftPortParent : rightPortParent);
                
            allPorts.Add(myPortExtra.GetComponent<NodePortGfx>().Setup(this, myAdapter, null, 0));
        }
        RebuildLayout();
    }

    public void ReDrawConnections() {
        foreach (var port in allPorts) {
            if (port.myConnection != null) {
                var connectedNode = myViewer.GetNodeWithId(port.myConnection.nodeId);
                if (connectedNode != null) {
                    foreach (var connectedNodePort in connectedNode.allPorts) {
                        if (connectedNodePort.myConnection != null) {
                            if (connectedNodePort.myConnection.nodeId == myNode.id) {
                                port.SetConnection(connectedNodePort);
                                break;
                            }
                        }
                    }
                } 
            }
        }
    }
    
    
    public void AdapterConnectionValueUpdated(AdapterGroup.AdapterConnection connection, int value) {
        if(connection != null)
            connection.count = value;
    }

    void RebuildLayout() {
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }

    private Color RealColor;
    public void ShowDeleteColors(bool isDelete) {
        Image bg = GetComponent<Image>();
        if (isDelete) {
            if (bg.color != Color.red) {
                RealColor = bg.color;
            }
            
            bg.color = Color.red;
        } else {
            if (bg.color != Color.red) {
                RealColor = bg.color;
            }
            bg.color = RealColor;
        }
    }
}
