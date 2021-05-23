using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// The graphics for the crafting node
/// </summary>
public class CraftingNodeGfx : NodeGfx {

    public Image background;
    public Text craftingName;
    
    public Color[] craftingTypeColors = new Color[11];
    public float nonLegalBrightness = 0.5f;
    
    public ValueRegion craftingTypeRegion;
    public ValueRegion craftingTierRegion;
    public ValueRegion timeCostRegion;

    private static string[] dropDownOptions = null;
    private static CraftingNode.cTypes[] craftingTypes;
    public void ReDrawnNode(RecipeTreeViewer master, CraftingNode node) {
        base.ReDrawnNode(master, node);

        myNode = node;

        if (dropDownOptions == null) {
            craftingTypes = (CraftingNode.cTypes[]) Enum.GetValues(typeof(CraftingNode.cTypes));
            dropDownOptions = new string[craftingTypes.Length];
            for (int i = 0; i < craftingTypes.Length; i++) {
                dropDownOptions[i] = craftingTypes[i].ToString();
            }
            
        }
        craftingTypeRegion.SetUp(ValueRegion.type.craftingType,true, "Crafting Type", dropDownOptions,this, (int)node.CraftingType);
        craftingTierRegion.SetUp(ValueRegion.type.craftingTier,true, "Crafting Tier", null,this, node.tier);
        timeCostRegion.SetUp(ValueRegion.type.timeCost,true, "Time Cost",  null, this, node.timeCost);
        
        UpdateVisuals();
    }

    
    public void ValueUpdated(ValueRegion.type type, int value) {
        //print(type.ToString() + " - " + value.ToString() + " - " + index.ToString());
        switch (type) {
            case ValueRegion.type.port:
                Debug.LogError("Port types should not use this update!");
                break;
            case ValueRegion.type.craftingType:
                (myNode as CraftingNode).CraftingType = craftingTypes[value];
                break;
            case ValueRegion.type.craftingTier:
                (myNode as CraftingNode).tier = value;
                break;
            case ValueRegion.type.timeCost:
                (myNode as CraftingNode).timeCost = value;
                break;
        }

        UpdateVisuals();
    }


    public void UpdateVisuals() {
        bool isLegal = false;
        CraftingNode node = (myNode as CraftingNode);

        var inputCount = node.GetAdapterConnectionCount(true);
        var outputCount = node.GetAdapterConnectionCount(false);
        
        switch (node.CraftingType) {
            case CraftingNode.cTypes.Miner:
                // for miner we want zero inputs and a single ore as output
                if (inputCount == 0 && outputCount == 1)
                    isLegal = true;
                background.color = craftingTypeColors[0];
                break;
            case CraftingNode.cTypes.Furnace:
                if (true)
                    isLegal = true;
                background.color = craftingTypeColors[1];
                break;
            case CraftingNode.cTypes.ProcessorSingle:
                if (inputCount == 1 && outputCount > 0)
                    isLegal = true;
                background.color = craftingTypeColors[2];
                break;
            case CraftingNode.cTypes.ProcessorDouble:
                if (inputCount == 2 && outputCount > 0)
                    isLegal = true;
                background.color = craftingTypeColors[3];
                break;
            case CraftingNode.cTypes.Press:
                if (inputCount == 1 && outputCount == 1)
                    isLegal = true;
                background.color = craftingTypeColors[4];
                break;
            case CraftingNode.cTypes.Coiler:
                if (inputCount == 1 && outputCount == 1)
                    isLegal = true;
                background.color = craftingTypeColors[5];
                break;
            case CraftingNode.cTypes.Cutter:
                if (inputCount == 1 && outputCount== 1)
                    isLegal = true;
                background.color = craftingTypeColors[6];
                break;
            case CraftingNode.cTypes.Lab:
                if (true)
                    isLegal = true;
                background.color = craftingTypeColors[7];
                break;
            case CraftingNode.cTypes.Building:
                if (outputCount  == 1)
                    isLegal = true;
                background.color = craftingTypeColors[8];
                break;
            case CraftingNode.cTypes.House:
                if (inputCount > 0 && outputCount == 0)
                    isLegal = true;
                background.color = craftingTypeColors[9];
                break;
            case CraftingNode.cTypes.Farm:
                if (outputCount > 0)
                    isLegal = true;
                background.color = craftingTypeColors[10];
                break;
        }

        if(!isLegal)
            SetNonLegalColor();

        craftingName.text = node.CraftingType.ToString() + " Process";
    }

    void SetNonLegalColor() {
        Color.RGBToHSV(background.color, out float h, out float s, out float v);
        background.color = Color.HSVToRGB(h,s,nonLegalBrightness);
    }
}