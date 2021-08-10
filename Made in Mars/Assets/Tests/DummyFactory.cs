using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static Tests.DummyFactory;

namespace Tests
{
	public class DummyFactory {
		
		public static string dummyInputName = "dummy input";
		public static  string dummyOutputName = "dummy output";

		[SetUp]
		public void SetUp() {
			SetUpDataHolder();
		}
		
		
		[TearDown]
		public void TearDown() {
			TearDownDataHolder();
		}
		

		public static Item GetDummyInputItem() {
			return ScriptableObject.CreateInstance<Item>().MakeDummyItem(dummyInputName);
		}
		
		public static Item GetDummyOutputItem() {
			return ScriptableObject.CreateInstance<Item>().MakeDummyItem(dummyOutputName);
		}

		public static Item MakeDummyItem(string itemUniqueName) {
			return ScriptableObject.CreateInstance<Item>().MakeDummyItem(itemUniqueName);
		}
		

		public static void SetUpDataHolder() {
			DataHolder.s = new DataHolder();
			var itemSet = ScriptableObject.CreateInstance<ItemSet>();
			DataHolder.s.allItemSets = new[] {itemSet};
			itemSet.items = new[] {GetDummyInputItem(), GetDummyOutputItem()};
			var recipeSet = ScriptableObject.CreateInstance<RecipeSet>();
			DataHolder.s.allRecipeSets = new[] {recipeSet};
			DataHolder.s.Setup();
		}

		public static void TearDownDataHolder() {
			DataHolder.s = null;
		}

		public static void SetUpGridAndFactoryMaster() {
			Grid.s = new Grid();
			Grid.s.DummySetup(25);
			FactoryMaster.s = new FactoryMaster();
		}
		
		public static void TearDownGridAndFactoryMaster() {
			Grid.s = null;
			FactoryMaster.s = null;
		}
		
		public static Belt GetFullBelt() {
			return new Belt(new Position(0, 0), new Position(0, 0)) {
				items = new List<Belt.BeltSegment>() {
					new Belt.BeltSegment() {count = 1, item = MakeDummyItem(1.ToString())},
				}
			};
		}

		public static Belt GetEmptyBelt() {
			return new Belt(new Position(0, 0), new Position(0, 0)) {
				items = new List<Belt.BeltSegment>() {
					new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
				}
			};
		}

		public static bool CheckBeltEmptyness(Connector.Connection connection) {
			return (connection.myObj as Belt).items[0].item.isEmpty();
		}

		[Test]
		public void TestBeltGenerator() {
			var beltfull = GetFullBelt();
			var beltempty = GetEmptyBelt();

			Assert.IsFalse(CheckBeltEmptyness(new Connector.Connection(beltfull, new Position(0, 0), 0)));
			Assert.IsTrue(CheckBeltEmptyness(new Connector.Connection(beltempty, new Position(0, 0), 0)));
		}

		public static Building GetFullBuilding() {
			var inventory = new List<InventoryItemSlot>();
			inventory.Add(new InventoryItemSlot(MakeDummyItem(1.ToString()), 1, 1, InventoryItemSlot.SlotType.output));
			inventory.Add(new InventoryItemSlot(MakeDummyItem(1.ToString()), 1, 1, InventoryItemSlot.SlotType.input));
			var building = new Building(new Position(0, 0), GetDummyBuildingData(), inventory);
			building.inv = InventoryFactory.CreateBuildingInventory(null, building);
			building.inv.SetInventory(inventory);
			return building;
		}

		public static Building GetEmptyBuilding() {
			var inventory = new List<InventoryItemSlot>();
			inventory.Add(new InventoryItemSlot(MakeDummyItem(1.ToString()), 0, 1, InventoryItemSlot.SlotType.output));
			inventory.Add(new InventoryItemSlot(MakeDummyItem(1.ToString()), 0, 1, InventoryItemSlot.SlotType.input));
			var building = new Building(new Position(0, 0), GetDummyBuildingData(), inventory);
			building.inv = InventoryFactory.CreateBuildingInventory(null, building);
			building.inv.SetInventory(inventory);
			return building;
		}

		public static int GetBuildingInputCount(Connector.Connection connection) {
			return (connection.myObj as Building).inv.GetTotalAmountOfItems(false);
		}

		public static int GetBuildingOutputCount(Connector.Connection connection) {
			return (connection.myObj as Building).inv.GetTotalAmountOfItems(true);
		}

		[Test]
		public void TestBuildingGenerator() {
			var buildingFull = GetFullBuilding();
			var buildingEmpty = GetEmptyBuilding();

			Assert.AreEqual(GetBuildingInputCount(new Connector.Connection(buildingFull, new Position(0, 0), 0)), 1);
			Assert.AreEqual(GetBuildingOutputCount(new Connector.Connection(buildingFull, new Position(0, 0), 0)), 1);
			Assert.AreEqual(GetBuildingInputCount(new Connector.Connection(buildingEmpty, new Position(0, 0), 0)), 0);
			Assert.AreEqual(GetBuildingOutputCount(new Connector.Connection(buildingEmpty, new Position(0, 0), 0)), 0);
		}
		
		public static bool BeltSegmentEqualityChecker(List<Belt.BeltSegment> first, List<Belt.BeltSegment> second) {

            if (first.Count == second.Count) {
                for (int i = 0; i < first.Count; i++) {
                    if (first[i].item != second[i].item) {
                        return false;
                    }

                    if (first[i].count != second[i].count) {
                        return false;
                    }
                }
            } else {
                return false;
            }

            return true;
        }

        [Test]
        public void BeltSegmentEqualityCheckerTest() {
            var beltsegment1 = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 3, item = Item.GetEmpty()}
            };
            var beltsegment2 = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 3, item = Item.GetEmpty()}
            };
            var beltsegment3 = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 3, item = Item.GetEmpty()},
                new Belt.BeltSegment() {count = 2, item = MakeDummyItem(1.ToString())},
            };
            var beltsegment4 = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 3, item = Item.GetEmpty()},
                new Belt.BeltSegment() {count = 2, item = MakeDummyItem(1.ToString())}
            };
            var beltsegment5 = new List<Belt.BeltSegment>() { };
            var beltsegment6 = new List<Belt.BeltSegment>() { };

            Assert.IsTrue(BeltSegmentEqualityChecker(beltsegment1, beltsegment2));
            Assert.IsTrue(BeltSegmentEqualityChecker(beltsegment3, beltsegment4));
            Assert.IsTrue(BeltSegmentEqualityChecker(beltsegment5, beltsegment6));
            Assert.IsFalse(BeltSegmentEqualityChecker(beltsegment1, beltsegment3));
            Assert.IsFalse(BeltSegmentEqualityChecker(beltsegment2, beltsegment4));
            Assert.IsFalse(BeltSegmentEqualityChecker(beltsegment1, beltsegment5));
        }

        public static BuildingData GetDummyBuildingDataForConstruction(List<DataHolder.CountedItem> materials) {
	        if (DataHolder.s == null) {
		        throw new Exception("You need to setup a DataHolder for this operation!");
	        }

	        DataHolder.s.allItemSets[0].items = new Item[materials.Count];
	        for(int i = 0; i < materials.Count; i++) {
		        DataHolder.s.allItemSets[0].items[i] = MakeDummyItem(materials[i].itemUniqueName);
	        }

	        var buildingData = ScriptableObject.CreateInstance<BuildingData>();
	        buildingData.uniqueName = "dummy Building";
	        buildingData.myType = BuildingData.ItemType.Furnace;
	        buildingData.shape = new ArrayLayout();
	        buildingData.shape.column[3].row[3] = true;
	        DataHolder.s.allBuildings = new [] {buildingData};

	        var recipeSet = ScriptableObject.CreateInstance<RecipeSet>();
	        DataHolder.s.allRecipeSets = new[] {recipeSet};

	        recipeSet.myBuildings = new[] {buildingData};

	        recipeSet.myItemSets = DataHolder.s.allItemSets;
	        for(int i = 0; i < DataHolder.s.allItemSets[0].items.Length; i++) {
		        recipeSet.AddItemNode(Vector3.zero, DataHolder.s.allItemSets[0].items[i]);
	        }

	        recipeSet.AddCraftingNode(Vector3.zero);
	        var craftingNode = recipeSet.GetCraftingNodes()[0];
	        craftingNode.CraftingType = CraftingNode.cTypes.Building;
	        foreach (var itemNode in recipeSet.GetItemNodes()) {
		        itemNode.SetupAdapters();
		        RecipeSet.ConnectAdapters(itemNode.myAdapters[1], craftingNode.myAdapters[0]);
	        }

	        var buildingNode = recipeSet.AddItemNode(Vector3.zero,MakeDummyItem(buildingData.uniqueName));
	        RecipeSet.ConnectAdapters(buildingNode.myAdapters[0], craftingNode.myAdapters[1]);
	        
	        DataHolder.s.Setup();
	        
	        return buildingData;
        }
        
        public static BuildingData GetDummyBuildingData() {
	        if (DataHolder.s == null) {
		        throw new Exception("You need to setup a DataHolder for this operation!");
	        }
	        
	        var buildingData = ScriptableObject.CreateInstance<BuildingData>();
	        buildingData.uniqueName = "dummy Building";
	        buildingData.myType = BuildingData.ItemType.Furnace;
	        buildingData.shape = new ArrayLayout();
	        buildingData.shape.column[3].row[3] = true;
	        DataHolder.s.allBuildings = new [] {buildingData};

	        return buildingData;
        }


        public static Building CreateStorage(int slots, bool isInfinite) {
	        var buildingData = ScriptableObject.CreateInstance<BuildingData>();
	        buildingData.myType = BuildingData.ItemType.Storage;
	        buildingData.myTier = slots;
	        buildingData.shape = new ArrayLayout();
	        buildingData.shape.column[3].row[3] = true;
	        var building = FactoryBuilder.CreateBuilding(buildingData, new Position(0, 0), null);
	        building.inv.isCheatInventory = isInfinite;
	        return building;
        }
	}
}
