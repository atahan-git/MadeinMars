using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static Tests.DummyFactory;

namespace Tests {
    public class BuildingTester {

        [SetUp]
        public void SetUp() {
            SetUpDataHolder();
            SetUpGridAndFactoryMaster();
        }


        [TearDown]
        public void TearDown() {
            TearDownDataHolder();
            TearDownGridAndFactoryMaster();
        }
        
        
        [Test]
        public void TestStorageCreation() {
            var storage = CreateStorage(10, false);
            
            Assert.IsNotNull(storage);
            Assert.AreEqual(storage.inv.inventoryItemSlots.Count, 10);
            Assert.AreEqual(storage.inv.inventoryItemSlots[0].mySlotType, InventoryItemSlot.SlotType.storage);
        }
        
        [Test]
        public void TestInfiniteStorage() {
            var storage = CreateStorage(10,true);
            
            Assert.IsNotNull(storage);
            Assert.IsTrue(storage.inv.TryAndTakeItem(new Item().MakeDummyItem("iron"),5,true));
            Assert.IsTrue(storage.inv.TryAndTakeItem(new Item().MakeDummyItem("iron"),5,true));
        }
        
        [Test]
        public void TestPuttingItemInInfiniteStorage() {
            var storage = CreateStorage(10,true);
            
            Assert.IsNotNull(storage);
            var item = new Item().MakeDummyItem("iron");
            var amount = 5;
            Assert.AreEqual(storage.inv.GetAmountOfItems(item), 0);
            Assert.IsTrue(storage.inv.TryAndAddItem(item,amount,true));
            Assert.AreEqual(storage.inv.GetAmountOfItems(item), amount);
        }

        [Test]
        public void TestMinerInventoryCreation() {
            var miner = CreateMiner();
            
            Assert.IsNotNull(miner);
            Assert.AreEqual(1, miner.inv.inventoryItemSlots.Count);
            Assert.AreEqual( "dummyOre", miner.inv.inventoryItemSlots[0].myItem.uniqueName);
        }
        
        Building CreateMiner() {
            var buildingData = GetDummyBuildingData();
            buildingData.myType = BuildingData.ItemType.Miner;

            Grid.s.GetTile(new Position(0, 0)).oreType = 1;
            Grid.s.GetTile(new Position(0, 0)).oreAmount = 10;

            var oreSpawnSettings = ScriptableObject.CreateInstance<OreSpawnSettings>();
            oreSpawnSettings.oreUniqueName = "dummyOre";
            var recipeSet = ScriptableObject.CreateInstance<RecipeSet>();
            recipeSet.myOres = new[] {oreSpawnSettings};
            DataHolder.s.myItemSets[0].items = new[] {new Item() {uniqueName = "dummyOre"}};
            DataHolder.s.myRecipeSets = new[] {recipeSet};
            
            var craftingNode = recipeSet.AddCraftingNode(Vector3.zero);
            var oreNode = recipeSet.AddItemNode(Vector3.zero, DataHolder.s.OreIdToItem(1));
            craftingNode.SetupAdapters();
            oreNode.SetupAdapters();
            RecipeSet.ConnectAdapters(craftingNode.myAdapters[1], oreNode.myAdapters[0]);
            craftingNode.timeCost = 0;
            craftingNode.CraftingType = CraftingNode.cTypes.Miner;
            
            DataHolder.s.Setup();
            
            var miner = FactoryBuilder.CreateBuilding(buildingData, new Position(0, 0), null);
            return miner;
        }


        [Test]
        public void TestMinerCraftingOres() {
            var miner = CreateMiner();

            Assert.AreEqual(0, miner.inv.GetTotalAmountOfItems());
            
            FactorySimulator.UpdateBuilding(miner, 1);
            
            Assert.AreEqual(1, miner.inv.GetTotalAmountOfItems());
            
        }
    }
}