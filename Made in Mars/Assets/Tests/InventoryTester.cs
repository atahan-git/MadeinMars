using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static Tests.DummyFactory;

namespace Tests
{
    public class InventoryTester {
        
        [SetUp]
        public void SetUp() {
            SetUpDataHolder();
        }

        [TearDown]
        public void TearDown() {
            TearDownDataHolder();
        }

        [Test]
        public void TestEmptyBuildingInventory() {
            var fakeBuilding = new Building(Position.InvalidPosition(), GetDummyBuildingData(), null);
            var inventory = InventoryFactory.CreateBuildingInventory(null, fakeBuilding);
            
            Assert.IsNotNull(inventory);
        }


        [Test]
        public void TestBuildingInventoryWithRecipes() {
            var fakeBuilding = new Building(Position.InvalidPosition(), GetDummyBuildingData(), null);
            fakeBuilding.craftController.myCraftingProcesses = new CraftingProcess[] {
                new CraftingProcess(
                    new List<DataHolder.CountedItem>() {new DataHolder.CountedItem(dummyInputName, 1)},
                    new List<DataHolder.CountedItem>() {new DataHolder.CountedItem(dummyOutputName, 1)},
                    1)
            };
            var inventory = InventoryFactory.CreateBuildingInventory(null, fakeBuilding);
            
            Assert.IsNotNull(inventory.inventoryItemSlots);
            Assert.AreEqual(inventory.inventoryItemSlots.Count, 2);
            Assert.AreEqual(inventory.inventoryItemSlots[0].myItem.uniqueName, dummyInputName);
            Assert.AreEqual(inventory.inventoryItemSlots[1].myItem.uniqueName, dummyOutputName);
        }
    }
}
