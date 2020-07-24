using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNodeEditor;
using XNode;


[CustomNodeEditor(typeof(CraftingProcessNode))]
public class CraftingProcessNodeEditor : NodeEditor {

	public const float portFieldVerticalSize = 20f;
	public const float startFromTopDist = 96f;

	public override void OnHeaderGUI () {
		//GUI.backgroundColor = Color.white;
		CraftingProcessNode node = target as CraftingProcessNode;
		RecipeSet graph = node.graph as RecipeSet;

		string myColor = "#FFFFFF";
		switch (node.CraftingType) {
		case CraftingProcessNode.cTypes.Miner:
			myColor = "#E1AAFF";
			break;
		case CraftingProcessNode.cTypes.Furnace:
			myColor = "#FFAAAA";
			break;
		case CraftingProcessNode.cTypes.ProcessorSingle:
			myColor = "#AAB1FF";
			break;
		case CraftingProcessNode.cTypes.ProcessorDouble:
			myColor = "#AAFFAA";
			break;
		}

		Color color;
		ColorUtility.TryParseHtmlString(myColor, out color);
		GUI.color = color;
		//if (graph.current == node) GUI.color = Color.blue;
		string title = target.name +" Tier "+ node.CraftingTier.ToString();
		GUILayout.Label(title, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));

		GUI.color = Color.white;
	}

	public override void OnBodyGUI () {
		CraftingProcessNode node = target as CraftingProcessNode;
		RecipeSet graph = node.graph as RecipeSet;
		// Limiting crafting counts stuff
		int maxInputCount = -1;
		switch (node.CraftingType) {
		case CraftingProcessNode.cTypes.Miner:
			maxInputCount = 0;
			break;
		case CraftingProcessNode.cTypes.Furnace:
		case CraftingProcessNode.cTypes.ProcessorSingle:
		case CraftingProcessNode.cTypes.Cutter:
			maxInputCount = 1;
			break;
		case CraftingProcessNode.cTypes.ProcessorDouble:
			maxInputCount = 2;
			break;
		}

		if (maxInputCount != -1) {
			if (node.inputItemUniqueNames.Count - 1 < maxInputCount) {
				GUI.color = Color.yellow;
			} else if (node.inputItemUniqueNames.Count - 1 > maxInputCount) {
				GUI.color = Color.red;
			} else {
				GUI.color = Color.white;
			}
		} else {
			GUI.color = Color.white;
		}



		base.OnBodyGUI();
		bool inputPortAdded = false;

		// --------------------------------------------------------- Input fields
		if (Event.current.type == EventType.Repaint) {
			if (node.inputItemUniqueNames.Count == 0) { // if there is none start by adding the first
				node.inputItemUniqueNames.Add("Empty");
				node.inputItemCounts.Add(1);
				NodeEditorGUILayout.AddPortField(node.AddDynamicInput(typeof(ItemInput), fieldName: "Input " + node.inputItemUniqueNames.Count.ToString()));
				inputPortAdded = true;
			}

			// If there are not enough ports keep adding them until we reach the list length
			while (node.GetInputPort("Input " + node.inputItemUniqueNames.Count.ToString()) == null) {
				NodeEditorGUILayout.AddPortField(node.AddDynamicInput(typeof(ItemInput), fieldName: "Input " + (node.inputItemUniqueNames.Count+1).ToString()));
				inputPortAdded = true;
			}

			// if the last port has more than 0 connections then add another empty port
			if (node.GetInputPort("Input " + node.inputItemUniqueNames.Count.ToString()).ConnectionCount > 0) {
				node.inputItemUniqueNames.Add("Empty");
				node.inputItemCounts.Add(1);
				NodeEditorGUILayout.AddPortField(node.AddDynamicInput(typeof(ItemInput), fieldName: "Input " + node.inputItemUniqueNames.Count.ToString()));
				inputPortAdded = true;
			}

			// for all of the existing ports
			for (int i = 0; i < node.inputItemUniqueNames.Count-1; i++) {
				// if any of them have 0 connections now (user disconnected one) we remove that one
				if (node.GetInputPort("Input " + (i + 1).ToString()).ConnectionCount == 0) {
					// shift the below ports up
					for (int m = i+1; m < node.inputItemUniqueNames.Count-1; m++) {
						node.GetPort("Input " + (m + 1).ToString()).MoveConnections(node.GetPort("Input " + (m).ToString()));
					}

					// remove the last port
					node.RemoveDynamicPort(node.GetPort("Input " + (node.inputItemUniqueNames.Count).ToString()));
					// remove the data about the specific port we removed
					node.inputItemUniqueNames.RemoveAt(i);
					node.inputItemCounts.RemoveAt(i);
				}

				// if any of the ports have more than 1 connection we remove the first one added
				if (node.GetInputPort("Input " + (i + 1).ToString()).ConnectionCount > 1) {
					node.GetInputPort("Input " + (i + 1).ToString()).Disconnect(node.GetInputPort("Input " + (i + 1).ToString()).GetConnection(0));
				}
			}

			// these two are just to match the length of 'unique names' list and 'item counts' list
			if (node.inputItemUniqueNames.Count-1 > node.inputItemCounts.Count) {
				node.inputItemCounts.Add(1);
			}
			if (node.inputItemUniqueNames.Count-1 < node.inputItemCounts.Count) {
				node.inputItemCounts.RemoveAt(node.inputItemCounts.Count - 1);
			}
		}

		for (int i = 0; i < node.inputItemCounts.Count; i++) {
			if (GUI.Button(new Rect(100, startFromTopDist + (i * portFieldVerticalSize), 17, 17), "-")) {
				node.inputItemCounts[i]--;
			}
			if (GUI.Button(new Rect(100+ portFieldVerticalSize, startFromTopDist + (i * portFieldVerticalSize), 17, 17), "+")) {
				node.inputItemCounts[i]++;
			}

			node.inputItemCounts[i] = GUIIntFieldInput(node.inputItemCounts[i], i);

			node.inputItemCounts[i] = Mathf.Clamp(node.inputItemCounts[i], 1, int.MaxValue);

			if (node.GetInputPort("Input " + (i + 1).ToString()).ConnectionCount > 0) {
				ItemNode connectedItemNode = node.GetInputPort("Input " + (i + 1).ToString()).GetConnection(0).node as ItemNode;
				if (connectedItemNode != null)
					node.inputItemUniqueNames[i] = connectedItemNode.myUniqueName;
			}
		}

		// --------------------------------------------------------- Output Fields
		if (Event.current.type == EventType.Repaint) {
			if (node.outputItemUniqueNames.Count == 0) { // if there is none start by adding the first
				node.outputItemUniqueNames.Add("Empty");
				node.outputItemCounts.Add(1);
				NodeEditorGUILayout.AddPortField(node.AddDynamicOutput(typeof(ItemOutput), fieldName: "Output " + node.outputItemUniqueNames.Count.ToString()));
			}

			// If there are not enough ports keep adding them until we reach the list length
			while (node.GetOutputPort("Output " + node.outputItemUniqueNames.Count.ToString()) == null) {
				NodeEditorGUILayout.AddPortField(node.AddDynamicOutput(typeof(ItemOutput), fieldName: "Output " + (node.outputItemUniqueNames.Count + 1).ToString()));
			}

			// if the last port has more than 0 connections then add another empty port
			if (node.GetOutputPort("Output " + node.outputItemUniqueNames.Count.ToString()).ConnectionCount > 0) {
				node.outputItemUniqueNames.Add("Empty");
				node.outputItemCounts.Add(1);
				NodeEditorGUILayout.AddPortField(node.AddDynamicOutput(typeof(ItemOutput), fieldName: "Output " + node.outputItemUniqueNames.Count.ToString()));
			}

			// for all of the existing ports
			for (int i = 0; i < node.outputItemUniqueNames.Count - 1; i++) {
				// if any of them have 0 connections now (user disconnected one) we remove that one
				if (node.GetOutputPort("Output " + (i + 1).ToString()).ConnectionCount == 0) {
					// shift the below ports up
					for (int m = i + 1; m < node.outputItemUniqueNames.Count - 1; m++) {
						node.GetPort("Output " + (m + 1).ToString()).MoveConnections(node.GetPort("Output " + (m).ToString()));
					}

					// remove the last port
					node.RemoveDynamicPort(node.GetPort("Output " + (node.outputItemUniqueNames.Count).ToString()));
					// remove the data about the specific port we removed
					node.outputItemUniqueNames.RemoveAt(i);
					node.outputItemCounts.RemoveAt(i);
				}

				// if any of the ports have more than 1 connection we remove the first one added
				if (node.GetOutputPort("Output " + (i + 1).ToString()).ConnectionCount > 1) {
					node.GetOutputPort("Output " + (i + 1).ToString()).Disconnect(node.GetOutputPort("Output " + (i + 1).ToString()).GetConnection(0));
				}
			}

			// these two are just to match the length of 'unique names' list and 'item counts' list
			if (node.outputItemUniqueNames.Count - 1 > node.outputItemCounts.Count) {
				node.outputItemCounts.Add(1);
			}
			if (node.outputItemUniqueNames.Count - 1 < node.outputItemCounts.Count) {
				node.outputItemCounts.RemoveAt(node.outputItemCounts.Count - 1);
			}
		}

		for (int i = 0; i < node.outputItemCounts.Count; i++) {
			// the offset we need to draw output items
			float inputOffset = node.inputItemCounts.Count * portFieldVerticalSize + 20;

			if (GUI.Button(new Rect(65, startFromTopDist + (i * portFieldVerticalSize) + inputOffset, 17, 17), "-")) {
				node.outputItemCounts[i]--;
			}
			if (GUI.Button(new Rect(65 + portFieldVerticalSize, startFromTopDist + (i * portFieldVerticalSize) + inputOffset, 17, 17), "+")) {
				node.outputItemCounts[i]++;
			}

			node.outputItemCounts[i] = GUIIntFieldOutput(node.outputItemCounts[i], i, inputOffset);

			node.outputItemCounts[i] = Mathf.Clamp(node.outputItemCounts[i], 1, int.MaxValue);

			if (node.GetOutputPort("Output " + (i + 1).ToString()).ConnectionCount > 0) {
				ItemNode connectedItemNode = node.GetOutputPort("Output " + (i + 1).ToString()).GetConnection(0).node as ItemNode;
				if (connectedItemNode != null)
					node.outputItemUniqueNames[i] = connectedItemNode.myUniqueName;

				BuildingNode connectedBuildingNode = node.GetOutputPort("Output " + (i + 1).ToString()).GetConnection(0).node as BuildingNode;
				if (connectedBuildingNode != null)
					node.outputItemUniqueNames[i] = connectedBuildingNode.myUniqueName;
			}
		}
	}

	public int GUIIntFieldInput (int input, int offset) {
		string s = GUI.TextField(new Rect(65, startFromTopDist + (offset* portFieldVerticalSize), 30, 17), input.ToString());
		int output = 1;
		if (int.TryParse(s, out output)) {
			return output;
		} else {
			return 1;
		}
	}

	public int GUIIntFieldOutput (int input, int offset, float inputOffset) {
		string s = GUI.TextField(new Rect(105, startFromTopDist + (offset * portFieldVerticalSize) + inputOffset, 30, 17), input.ToString());
		int output = 1;
		if (int.TryParse(s, out output)) {
			return output;
		} else {
			return 1;
		}
	}
}
