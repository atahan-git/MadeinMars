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

        researchName.text = node.researchUniqueName;
        researchDescription.text = node.researchDescription;
    }

    public void ResearchNameValueUpdated() {
        (myNode as ResearchNode).researchUniqueName = researchName.text;
    }
    
    public void ResearchDescriptionValueUpdated() {
        (myNode as ResearchNode).researchDescription = researchDescription.text;
    }
    
}