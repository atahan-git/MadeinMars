using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using XNodeEditor;


[CustomNodeEditor(typeof(BuildingNode))]
public class BuildingNodeEditor : NodeEditor {

	BuildingData myBuilding;
	string lastUniqueName;

	public override void OnHeaderGUI () {
		GUI.backgroundColor = Color.white;
		GUI.color = Color.white;
		BuildingNode node = target as BuildingNode;
		RecipeSet graph = node.graph as RecipeSet;

		string title = node.myUniqueName;

		if (lastUniqueName != node.myUniqueName) {
			lastUniqueName = node.myUniqueName;
			myBuilding = graph.GetBuilding(lastUniqueName);
		}

		if (myBuilding != null) {
			title = myBuilding.name;
			GUI.color = Color.white;
		} else {
			GUI.color = Color.red;
		}

		GUILayout.Label(title, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
	}

	public override void OnBodyGUI () {
		BuildingNode node = target as BuildingNode;
		RecipeSet graph = node.graph as RecipeSet;

		if (lastUniqueName != node.myUniqueName) {
			lastUniqueName = node.myUniqueName;
			myBuilding = graph.GetBuilding(lastUniqueName);
		}
		if (myBuilding != null) {
			GUI.color = Color.white;
			if (myBuilding.BuildingSprite != null) {
				GUIDrawSprite(new Rect(10, 30, 60, 60), myBuilding.BuildingSprite);
			}
		} else {
			GUI.color = new Color(1, .5f, .5f);
		}
		GUILayout.Space(55);
		GUI.Label(new Rect(70, 40, 120, 20), "Unique Item Name");
		node.myUniqueName = GUI.TextField(new Rect(70, 60, 120, 20), node.myUniqueName);


		base.OnBodyGUI();
	}

	public static void GUIDrawSprite (Rect rect, Sprite sprite) {
		Rect spriteRect = sprite.rect;
		Texture2D tex = sprite.texture;
		GUI.DrawTextureWithTexCoords(rect, tex, new Rect(spriteRect.x / tex.width, spriteRect.y / tex.height, spriteRect.width / tex.width, spriteRect.height / tex.height));
	}
}