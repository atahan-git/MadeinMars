using System;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// The base class the other two (CraftingNodeGfx and ItemNodeGfx) extend upon
/// Holds the basic functionality shared across both
/// </summary>
public abstract class NodeGfx : MonoBehaviour {
    protected RecipeTreeMaster myMaster;
    public Node myNode;
    
    public GameObject inputPortPrefab;
    public Transform inputParent;
    public GameObject outputPortPrefab;
    public Transform outputParent;

    public List<NodePortGfx> inputPorts = new List<NodePortGfx>();
    public List<NodePortGfx> outputPorts = new List<NodePortGfx>();

    private int lastInputGiven = -1;
    private int lastOutputGiven = -1;
    public NodePortGfx GetNextEmptyNode(bool isInput) {
        if (isInput) {
            lastInputGiven += 1;
            return inputPorts[lastInputGiven];
        } else {
            lastOutputGiven += 1;
            return outputPorts[lastOutputGiven];
        }
    }
    
    protected void SetUp(RecipeTreeMaster master, Node node) {
        myMaster = master;
        myNode = node;
    }

    public void BeginClickConnect(NodePortGfx port) {
        myMaster.BeginClickConnect(this, port);
    }

    public const float snapMult = 0.0515f/2f;
    public static float snapMultUI;
    public void PositionUpdated() {
        //Snapping is done by DragPanel.cs
        /*myNode.x = ((int) (transform.position.x*snapMult))/snapMult;
        myNode.y = ((int) (transform.position.y*snapMult))/snapMult;
        transform.position = new Vector3(myNode.x,myNode.y,0);*/
        myMaster.RescaleNodeArea();
        myNode.pos = (transform as RectTransform).anchoredPosition;
    }


    public void OnDraggingNode() {
        foreach (var input in inputPorts) {
            input.OnPositionUpdated();
            if(input.myConnection)
                input.myConnection.OnPositionUpdated();
        }

        foreach (var output in outputPorts) {
            output.OnPositionUpdated();
            if(output.myConnection)
                output.myConnection.OnPositionUpdated();
        }
    }

    public void DeleteNode() {

        int count;
        count = inputPorts.Count;
        for (int i = count-1; i >= 0; i --) {
            var port = inputPorts[i];
            port.RemoveConnections();
            port.DeleteSelf();
        }

        count = outputPorts.Count;
        for (int i = count-1; i >= 0; i --) {
            var port = outputPorts[i];
            port.RemoveConnections();
            port.DeleteSelf();
        }

        myMaster.DeleteNode(this);
        Destroy(gameObject);
    }

    public void DeletePort(NodePortGfx.PortType portType, int index) {
        try {
            switch (portType) {
                case NodePortGfx.PortType.itemInput:
                case NodePortGfx.PortType.craftInput:
                    inputPorts.RemoveAt(index);
                    break;
                case NodePortGfx.PortType.itemOutput:
                case NodePortGfx.PortType.craftOutput:
                    outputPorts.RemoveAt(index);
                    break;
            }
        }catch{}

        if(this is CraftingNodeGfx)
            (this as CraftingNodeGfx).UpdateVisuals();
    }
    
    public void RemoveConnectionAtPort(NodePortGfx.PortType portType, int index) {
        try {
            switch (portType) {
                case NodePortGfx.PortType.craftInput:
                    (myNode as CraftingNode).inputs.RemoveAt(index);
                    //inputPorts.RemoveAt(index);
                    break;
                case NodePortGfx.PortType.craftOutput:
                    (myNode as CraftingNode).outputs.RemoveAt(index);
                    //outputPorts.RemoveAt(index);
                    break;
                case NodePortGfx.PortType.itemInput:
                    (myNode as ItemNode).inputIds.RemoveAt(index);
                    //inputPorts.RemoveAt(index);
                    break;
                case NodePortGfx.PortType.itemOutput:
                    (myNode as ItemNode).outputIds.RemoveAt(index);
                    //outputPorts.RemoveAt(index);
                    break;
            }
        }catch{}

        if(this is CraftingNodeGfx)
            (this as CraftingNodeGfx).UpdateVisuals();
    }

    public abstract void SetupPorts();
    public abstract void SetupConnections();

    public void RebuildLayout() {
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
