using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemNodeGfx : NodeGfx {
    public Image icon;
    public Text uniqueName;
    
    public void SetUp(NodeItemTreeMakerMaster master, ItemNode node) {
        base.SetUp(master, node);
        
        myNode = node;
        icon.sprite = ((ItemNode)myNode).myItem.mySprite;
        uniqueName.text = ((ItemNode)myNode).myItem.uniqueName;
        
        inputPorts.Add(Instantiate(inputPortPrefab,inputParent).GetComponent<NodePortGfx>()
            .Setup(this, NodePortGfx.PortType.itemInput));
        outputPorts.Add(Instantiate(outputPortPrefab,outputParent).GetComponent<NodePortGfx>()
            .Setup(this,NodePortGfx.PortType.itemOutput));
        
        LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
    }
}



public abstract class NodeGfx : MonoBehaviour {
    private NodeItemTreeMakerMaster myMaster;
    public Node myNode;
    
    public GameObject inputPortPrefab;
    public Transform inputParent;
    public GameObject outputPortPrefab;
    public Transform outputParent;

    public List<NodePortGfx> inputPorts = new List<NodePortGfx>();
    public List<NodePortGfx> outputPorts = new List<NodePortGfx>();

    protected void SetUp(NodeItemTreeMakerMaster master, Node node) {
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
        myMaster.DeleteNode(this);
        Destroy(gameObject);
    }
}
