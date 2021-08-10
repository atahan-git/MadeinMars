﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public static class InventoryFactory {


	public static Inventory CreateBuildingInventory(List<InventoryItemSlot> existingItems, Building building) {
		var inv = new Inventory();
		if (existingItems != null && existingItems.Count > 0) {
			inv.SetUp(existingItems);
		} else {
			switch (building.buildingData.myType) {
				case BuildingData.ItemType.Storage:
					for (int i = inv.inventoryItemSlots.Count; i < building.buildingData.myTier; i++) {
						inv.AddSlot(Item.GetEmpty(), 99, InventoryItemSlot.SlotType.storage, false);
					}

					break;
				case BuildingData.ItemType.Miner:
					for (int i = 0; i < building.myPositions.Count; i++) {
						var tile = Grid.s.GetTile(building.myPositions[i]);
						if (tile.oreType > 0) {
							inv.AddSlot(DataHolder.s.OreIdToItem(tile.oreType), 99, InventoryItemSlot.SlotType.output, false);
						}
					}


					break;
				case BuildingData.ItemType.Spaceship:
					/*inv.isGrid = true;
					
					for (int i = inv.inventoryItemSlots.Count; i < 30; i++) {
						inv.AddSlot(Item.GetEmpty(), 99, InventoryItemSlot.SlotType.storage, false);
					}
	                
					FactoryDrones.RegisterStorageBuilding(building);*/

					break;
				default:
					var myCrafter = building.craftController;
					for (int i = 0; i < myCrafter.myCraftingProcesses.Length; i++) {
						var inputs = myCrafter.myCraftingProcesses[i].GetInputItems();
						for (int m = 0; m < inputs.Length; m++) {
							inv.AddSlot(
								DataHolder.s.GetItem(inputs[m].itemUniqueName), inputs[m].count * 2,
								InventoryItemSlot.SlotType.input, false);
						}

						var outputs = myCrafter.myCraftingProcesses[i].GetOutputItems();
						for (int m = 0; m < outputs.Length; m++) {
							inv.AddSlot(
								DataHolder.s.GetItem(outputs[m].itemUniqueName), outputs[m].count * 2,
								InventoryItemSlot.SlotType.output, false);
						}
					}

					break;
			}
		}

		if (building.buildingData.myType == BuildingData.ItemType.Storage) {
			FactoryDrones.RegisterStorageBuilding(building);
		}

		inv.ReDrawInventory();
		return inv;
	}

	public static Inventory CreateConstructionInventory(List<InventoryItemSlot> materials) {
		var inv = new Inventory();
		inv.SetUp(null);
		
		inv.SetInventory(materials);
		inv.ReDrawInventory();
		return inv;
	}
	
	public static Inventory CreateDroneInventory() {
		var inv = new Inventory();
		inv.SetUp(null);
		inv.ReDrawInventory();
		return inv;
	}
}
