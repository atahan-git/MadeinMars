using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemNodeGfx : NodeGfx {
    public Image icon;
    public Text uniqueName;


    public void SetUp(RecipeTreeMaster master, ItemNode node) {
        base.SetUp(master, node);
        
        myNode = node;
        icon.sprite = ((ItemNode)myNode).myItem.mySprite;
        uniqueName.text = ((ItemNode)myNode).myItem.uniqueName;
    }

    public override void SetupPorts(){
        ItemNode node = myNode as ItemNode;
        for(int i =0 ; i < node.inputs.Count ; i ++) {
            inputPorts.Add(
                Instantiate(inputPortPrefab,inputParent).GetComponent<NodePortGfx>()
                    .Setup(
                        this, 
                        NodePortGfx.PortType.itemInput,
                        -1
                    )
            );
        }

        for(int i =0 ; i < node.outputs.Count ; i ++) {
            outputPorts.Add(
                Instantiate(outputPortPrefab,outputParent).GetComponent<NodePortGfx>()
                    .Setup(
                        this, 
                        NodePortGfx.PortType.itemOutput,
                        -1
                    )
            );
            
        }
        
        AddPort(true);
        AddPort(false);
    }

    public override void SetupConnections() {
        ItemNode node = myNode as ItemNode;
        
        for(int i =0 ; i < node.inputs.Count ; i ++) {
            inputPorts[i].AddConnection(myMaster.GetNodeGfxFromNode(node.inputs[i] as Node).GetNextEmptyNode(false));
        }

        for(int i =0 ; i < node.outputs.Count ; i ++) {
            outputPorts[i].AddConnection(myMaster.GetNodeGfxFromNode(node.outputs[i] as Node).GetNextEmptyNode(true));
        }
    }
    
    public void AddPort(bool isInput) {
        if (!isInput) {
            inputPorts.Add(Instantiate(inputPortPrefab,inputParent).GetComponent<NodePortGfx>()
                .Setup(this, NodePortGfx.PortType.itemInput, -1));
        } else {
            outputPorts.Add(Instantiate(outputPortPrefab,outputParent).GetComponent<NodePortGfx>()
                .Setup(this,NodePortGfx.PortType.itemOutput, -1));
        }
        
        RebuildLayout();
    }
}



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

    public const float snapMult = 5;
    public static float snapMultUI = 5;
    public void PositionUpdated() {
        //Snapping is done by DragPanel.cs
        /*myNode.x = ((int) (transform.position.x*snapMult))/snapMult;
        myNode.y = ((int) (transform.position.y*snapMult))/snapMult;
        transform.position = new Vector3(myNode.x,myNode.y,0);*/
        myMaster.RescaleNodeArea();
        myNode.pos = (transform as RectTransform).anchoredPosition;
    }

    public void DeleteNode() {
        foreach(var input in inputPorts)
            input.DeleteSelf();
        foreach(var output in outputPorts)
            output.DeleteSelf();
        
        myMaster.DeleteNode(this);
        Destroy(gameObject);
    }

    public void RemoveConnectionAtPort(NodePortGfx.PortType portType, int index) {
        try {
            switch (portType) {
                case NodePortGfx.PortType.craftInput:
                    (myNode as CraftingNode).inputs.RemoveAt(index);
                    break;
                case NodePortGfx.PortType.craftOutput:
                    (myNode as CraftingNode).outputs.RemoveAt(index);
                    break;
                case NodePortGfx.PortType.itemInput:
                    (myNode as ItemNode).inputs.RemoveAt(index);
                    break;
                case NodePortGfx.PortType.itemOutput:
                    (myNode as ItemNode).outputs.RemoveAt(index);
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
}
