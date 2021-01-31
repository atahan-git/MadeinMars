using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingNodeGfx : NodeGfx {

    public Image background;
    public Text craftingName;
    
    public Color[] craftingTypeColors = new Color[9];
    public float nonLegalBrightness = 0.5f;
    
    public ValueRegion craftingTypeRegion;
    public ValueRegion craftingTierRegion;
    public ValueRegion timeCostRegion;

    private static string[] dropDownOptions = null;
    private static CraftingNode.CraftingTypes[] craftingTypes;
    public void SetUp(RecipeTreeMaster master, CraftingNode node) {
        base.SetUp(master, node);

        myNode = node;

        if (dropDownOptions == null) {
            craftingTypes = (CraftingNode.CraftingTypes[]) Enum.GetValues(typeof(CraftingNode.CraftingTypes));
            dropDownOptions = new string[craftingTypes.Length];
            for (int i = 0; i < craftingTypes.Length; i++) {
                dropDownOptions[i] = craftingTypes[i].ToString();
            }
            
        }
        craftingTypeRegion.SetUp(ValueRegion.type.craftingType, dropDownOptions,this, (int)node.myCraftingType);
        craftingTierRegion.SetUp(ValueRegion.type.craftingTier, null,this, node.tier);
        timeCostRegion.SetUp(ValueRegion.type.timeCost, null, this, node.craftingTime);
    }

    public override void SetupPorts() {
        CraftingNode node = myNode as CraftingNode;
        
        for(int i =0 ; i < node.inputs.Count ; i ++) {
            inputPorts.Add(
                Instantiate(inputPortPrefab,inputParent).GetComponent<NodePortGfx>()
                    .Setup(
                        this, 
                        NodePortGfx.PortType.craftInput,
                        node.inputs[i].count
                    )
            );
        }

        for(int i =0 ; i < node.outputs.Count ; i ++) {
            outputPorts.Add(
                Instantiate(outputPortPrefab,outputParent).GetComponent<NodePortGfx>()
                    .Setup(
                        this, 
                        NodePortGfx.PortType.craftOutput,
                        node.outputs[i].count
                    )
            );
            
        }

        AddPort(true);
        AddPort(false);
    }
    
    public override void SetupConnections() {
        CraftingNode node = myNode as CraftingNode;
        
        for(int i =0 ; i < node.inputs.Count ; i ++) {
            inputPorts[i].AddConnection(myMaster.GetNodeGfxFromNode(node.inputs[i].itemNode as Node).GetNextEmptyNode(false));
        }

        for(int i =0 ; i < node.outputs.Count ; i ++) {
            outputPorts[i].AddConnection(myMaster.GetNodeGfxFromNode(node.outputs[i].itemNode as Node).GetNextEmptyNode(true));
        }
        
        RebuildLayout();
    }

    public void AddPort(bool isInput) {
        if (isInput) {
            inputPorts.Add(Instantiate(inputPortPrefab,inputParent).GetComponent<NodePortGfx>()
                .Setup(this, NodePortGfx.PortType.craftInput, 1));
        } else {
            outputPorts.Add(Instantiate(outputPortPrefab,outputParent).GetComponent<NodePortGfx>()
                .Setup(this, NodePortGfx.PortType.craftOutput, 1));
        }
        
        UpdateVisuals();
    }

    public void ValueUpdated(ValueRegion.type type, int value, int index) {
        //print(type.ToString() + " - " + value.ToString() + " - " + index.ToString());
        switch (type) {
            case ValueRegion.type.craftInput:
                if (index < (myNode as CraftingNode).inputs.Count)
                (myNode as CraftingNode).inputs[index].count = value;
                break;
            case ValueRegion.type.craftOutput:
                if (index < (myNode as CraftingNode).outputs.Count)
                (myNode as CraftingNode).outputs[index].count = value;
                break;
            case ValueRegion.type.craftingType:
                (myNode as CraftingNode).myCraftingType = craftingTypes[value];
                break;
            case ValueRegion.type.craftingTier:
                (myNode as CraftingNode).tier = value;
                break;
            case ValueRegion.type.timeCost:
                (myNode as CraftingNode).craftingTime = value;
                break;
        }

        UpdateVisuals();
    }
    
    
    public void UpdateVisuals() {
        bool isLegal = false;
        CraftingNode node = (myNode as CraftingNode);
        switch (node.myCraftingType) {
            case CraftingNode.CraftingTypes.Miner:
                if (node.inputs.Count == 0 && node.outputs.Count == 1)
                    isLegal = true;
                background.color = craftingTypeColors[0];
                break;
            case CraftingNode.CraftingTypes.Furnace:
                if (true)
                    isLegal = true;
                background.color = craftingTypeColors[1];
                break;
            case CraftingNode.CraftingTypes.ProcessorSingle:
                if (node.inputs.Count == 1)
                    isLegal = true;
                background.color = craftingTypeColors[2];
                break;
            case CraftingNode.CraftingTypes.ProcessorDouble:
                if (node.inputs.Count == 2)
                    isLegal = true;
                background.color = craftingTypeColors[3];
                break;
            case CraftingNode.CraftingTypes.Press:
                if (node.inputs.Count == 1 && node.outputs.Count == 1)
                    isLegal = true;
                background.color = craftingTypeColors[4];
                break;
            case CraftingNode.CraftingTypes.Coiler:
                if (node.inputs.Count == 1 && node.outputs.Count == 1)
                    isLegal = true;
                background.color = craftingTypeColors[5];
                break;
            case CraftingNode.CraftingTypes.Cutter:
                if (node.inputs.Count == 1 && node.outputs.Count == 1)
                    isLegal = true;
                background.color = craftingTypeColors[6];
                break;
            case CraftingNode.CraftingTypes.Lab:
                if (true)
                    isLegal = true;
                background.color = craftingTypeColors[7];
                break;
            case CraftingNode.CraftingTypes.Building:
                if (node.outputs.Count == 1)
                    isLegal = true;
                background.color = craftingTypeColors[8];
                break;
        }

        if(!isLegal)
            SetNonLegalColor();

        craftingName.text = node.myCraftingType.ToString() + " Process";
    }

    void SetNonLegalColor() {
        Color.RGBToHSV(background.color, out float h, out float s, out float v);
        background.color = Color.HSVToRGB(h,s,nonLegalBrightness);
    }
}