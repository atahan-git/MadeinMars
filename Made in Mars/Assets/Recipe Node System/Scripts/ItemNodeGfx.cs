using System;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The graphics for the item node
/// </summary>
public class ItemNodeGfx : NodeGfx {
    public Image icon;
    public Text uniqueName;


    public void SetUp(RecipeTreeMaster master, ItemNode node) {
        base.SetUp(master, node);
        
        myNode = node;
        icon.sprite = ((ItemNode)myNode).GetItem(master).mySprite;
        uniqueName.text = ((ItemNode)myNode).GetItem(master).uniqueName;
    }

    public override void SetupPorts(){
        ItemNode node = myNode as ItemNode;
        for(int i =0 ; i < node.inputIds.Count ; i ++) {
            inputPorts.Add(
                Instantiate(inputPortPrefab,inputParent).GetComponent<NodePortGfx>()
                    .Setup(
                        this, 
                        NodePortGfx.PortType.itemInput,
                        -1
                    )
            );
        }

        for(int i =0 ; i < node.outputIds.Count ; i ++) {
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
        
        for(int i =0 ; i < node.inputIds.Count ; i ++) {
            inputPorts[i].AddConnection(myMaster.GetNodeGfxFromNode(node.inputIds[i]).GetNextEmptyNode(false));
        }

        for(int i =0 ; i < node.outputIds.Count ; i ++) {
            outputPorts[i].AddConnection(myMaster.GetNodeGfxFromNode(node.outputIds[i]).GetNextEmptyNode(true));
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



