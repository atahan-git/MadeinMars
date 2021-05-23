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


    public void ReDrawnNode(RecipeTreeViewer master, ItemNode node) {
        base.ReDrawnNode(master, node);
        
        myNode = node;
        icon.sprite = ((ItemNode)myNode).GetItem(master).mySprite;
        uniqueName.text = ((ItemNode)myNode).GetItem(master).uniqueName;
    }
}



