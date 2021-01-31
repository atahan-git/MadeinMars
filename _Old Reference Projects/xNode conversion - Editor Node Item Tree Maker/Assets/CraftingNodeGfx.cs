using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingNodeGfx : NodeGfx {
    
    public void SetUp(NodeItemTreeMakerMaster master/*, CraftingNode node*/) {
        /*base.SetUp(master, node);
        
        myNode = node;
        
        foreach (var input in node.GetInputs()) {
            inputPorts.Add(
                Instantiate(inputPortPrefab,inputParent).GetComponent<NodePortGfx>()
                .Setup(
                    this, 
                    NodePortGfx.PortType.craftInput, 
                    //master.GetNodeGfxFromNode(input.itemNode as Node).outputPorts[0]
                    )
                );
        }

        foreach (var output in node.GetOutputs()) {
            outputPorts.Add(
                Instantiate(outputPortPrefab,outputParent).GetComponent<NodePortGfx>()
                    .Setup(
                        this, 
                        NodePortGfx.PortType.craftOutput, 
                        //master.GetNodeGfxFromNode(output.itemNode as Node).inputPorts[0]
                    )
            );
            
        }

        inputPorts.Add(Instantiate(inputPortPrefab,inputParent).GetComponent<NodePortGfx>()
            .Setup(this, NodePortGfx.PortType.craftInput));
        outputPorts.Add(Instantiate(outputPortPrefab,outputParent).GetComponent<NodePortGfx>()
            .Setup(this,NodePortGfx.PortType.craftOutput));
        
        LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);*/
    }
}