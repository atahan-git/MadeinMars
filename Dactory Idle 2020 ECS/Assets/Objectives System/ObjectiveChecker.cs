using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is supposed to be the class that holds all the objectives and check when they are done or not
/// Functionality is not there yet
/// </summary>
public class ObjectiveChecker {
	private readonly ObjectiveHolder[] allObjectives;
	private readonly List<InventoryItemSlot> allInventory;
	
	public ObjectiveChecker(ObjectiveHolder[] _allObjectives, List<InventoryItemSlot> _allInventory) {
		allObjectives = _allObjectives;
		allInventory = _allInventory;
	}

	public bool IsComplete(ObjectiveHolder obj) {
		for (int i = 0; i < obj.completeReqs.Length; i++) {
			if (!IsComplete(obj.completeReqs[i]))
				return false;
		}

		return true;
	}

	/*
	 
	 Codewords:
	 
	 [have x itemUniqueName]			Have x amounts of items available in your inventory
	 [craft x itemUniqueName]			Craft x amounts of items total							NOT IMPLEMENTED
	 [complete objectiveUniqueName]		Complete Objective
	 
	 
	 */
	 bool IsComplete(string o) {
		var commands = o.Split(' ');

		switch (commands[0]) {
			case "have":
				foreach (var itemSlot in allInventory) {
					if (itemSlot.myItem.uniqueName == commands[2]) {
						if (itemSlot.count >= int.Parse( commands[1])) {
							return true;
						}
					}
				}
				break;
			case "craft":
				Debug.LogError("Command not implemented: " + commands[0]);
				break;
			case "complete":
				foreach (var obj in allObjectives) {
					if (obj.uniqueName == commands[1]) {
						if (obj.isComplete) {
							return true;
						}
					}
				}
				break;
			default:
				Debug.LogError("Unknown command in objective: " + commands[0]);
				break;
		}

		return false;
	}

	public  bool IsUnlocked(ObjectiveHolder obj) {
		for (int i = 0; i < obj.unlockReqs.Length; i++) {
			if (!IsComplete(obj.unlockReqs[i]))
				return false;
		}
		return false;
	}
}
