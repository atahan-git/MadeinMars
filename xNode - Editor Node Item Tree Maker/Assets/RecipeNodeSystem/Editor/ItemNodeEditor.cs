using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNodeEditor;


[CustomNodeEditor(typeof(ItemNode))]
public class ItemNodeEditor : NodeEditor {

	Item myItem;
	string lastUniqueName;

	public override void OnHeaderGUI () {
		GUI.color = Color.white;
		ItemNode node = target as ItemNode;
		RecipeSet graph = node.graph as RecipeSet;

		string title = node.myUniqueName;

		if (lastUniqueName != node.myUniqueName) {
			lastUniqueName = node.myUniqueName;
			myItem = graph.GetItem(lastUniqueName);
		}

		if (myItem != null) {
			title = myItem.name;
			GUI.color = Color.white;
		} else {
			GUI.color = Color.red;
		}

		GUILayout.Label(title, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
	}

	public override void OnBodyGUI () {
		ItemNode node = target as ItemNode;
		RecipeSet graph = node.graph as RecipeSet;

		if (lastUniqueName != node.myUniqueName) {
			lastUniqueName = node.myUniqueName;
			myItem = graph.GetItem(lastUniqueName);
		}
		if (myItem != null) {
			GUI.color = Color.white;
			if (myItem.myMat != null) {
				GUI.DrawTexture(new Rect(20, 40, 40, 40), myItem.myMat.mainTexture, ScaleMode.ScaleToFit);
			}
		} else {
			GUI.color = new Color(1, .5f, .5f);
		}
		GUILayout.Space(55);
		GUI.Label(new Rect(70, 40, 120, 20), "Unique Item Name");
		node.myUniqueName = GUI.TextField(new Rect(70, 60, 120, 20), node.myUniqueName);


		base.OnBodyGUI();
	}
}