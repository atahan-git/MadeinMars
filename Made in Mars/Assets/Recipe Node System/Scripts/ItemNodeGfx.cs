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
        Item item = master.GetItemOfNode((ItemNode) myNode);
        var isEmptyItem = item == null || item.uniqueName.Length == 0;
        if (!isEmptyItem) {
            icon.sprite = item.mySprite;
            uniqueName.text = item.uniqueName;
        } else {
            uniqueName.text = "error";
        }
    }
}



