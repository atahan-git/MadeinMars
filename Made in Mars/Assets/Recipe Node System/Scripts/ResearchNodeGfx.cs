using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// The graphics for the researcch node node
/// </summary>
public class ResearchNodeGfx : NodeGfx {

    public Image background;

    public InputField researchName;
    public InputField researchDescription;

    public void ReDrawnNode(RecipeTreeViewer master, ResearchNode node) {
        base.ReDrawnNode(master, node);

        myNode = node;

        researchName.text = node.researchName;
        researchDescription.text = node.researchDescription;
    }

    public void ResearchNameValueUpdated() {
        (myNode as ResearchNode).researchName = researchName.text;
    }
    
    public void ResearchDescriptionValueUpdated() {
        (myNode as ResearchNode).researchDescription = researchDescription.text;
    }
    
}