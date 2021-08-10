using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static Tests.DummyFactory;

namespace Tests
{
    public class DronesAndConstructionTester
    {
        [SetUp]
        public void Setup() {
            SetUpGridAndFactoryMaster();
            SetUpDataHolder();
        }



        [TearDown]
        public void Teardown() {
            TearDownGridAndFactoryMaster();
            TearDownDataHolder();
        }


        [Test]
        public void TestDroneCreation() {
            FactoryBuilder.CreateDrone(new Position(0,0));
            Assert.AreEqual( 1,FactoryMaster.s.GetDrones().Count);
            Assert.IsNotNull(FactoryMaster.s.GetDrones()[0]);
            Assert.AreEqual( new Position(0,0),FactoryMaster.s.GetDrones()[0].curPosition);
        }

        [Test]
        public void TestConstructionCreation() {
            var dummyBuildingData = GetDummyBuildingDataForConstruction(new List<DataHolder.CountedItem>(){new DataHolder.CountedItem(GetDummyInputItem(),1)});
            FactoryBuilder.StartConstruction(dummyBuildingData, 0, new Position(0,1));
            
            Assert.AreEqual( 1,FactoryMaster.s.GetConstructions().Count);
        }
        
        [Test]
        public void TestConstructionMaterials() {
            var dummyBuildingData = GetDummyBuildingDataForConstruction(new List<DataHolder.CountedItem>() {new DataHolder.CountedItem("iron", 1)});
            FactoryBuilder.StartConstruction(dummyBuildingData, 0, new Position(0,0));
            
            Assert.AreEqual( 1,FactoryMaster.s.GetConstructions()[0].constructionInventory.inventoryItemSlots.Count);
            Assert.AreEqual( "iron", FactoryMaster.s.GetConstructions()[0].constructionInventory.inventoryItemSlots[0].myItem.uniqueName);
            Assert.AreEqual( 1,FactoryMaster.s.GetConstructions()[0].constructionInventory.inventoryItemSlots[0].maxCount);
        }

        [Test]
        public void TestBuildingBuildingByCode() {
            var dummyBuildingData = GetDummyBuildingDataForConstruction(new List<DataHolder.CountedItem>() {new DataHolder.CountedItem("iron", 1)});
            var position = new Position(0, 0);
            var construction = FactoryBuilder.StartConstruction(dummyBuildingData, 0, position);
            
            Assert.IsTrue(Grid.s.GetTile(position).simObject is Construction);
            Assert.IsFalse(Grid.s.GetTile(position).simObject is Building);
            
            FactoryBuilder.CompleteConstruction(construction);
            
            Assert.IsFalse(Grid.s.GetTile(position).simObject is Construction);
            Assert.IsTrue(Grid.s.GetTile(position).simObject is Building);
        }
        
        [Test]
        public void TestBuildingBuildingByDrone() {
            var dummyBuildingData = GetDummyBuildingDataForConstruction(new List<DataHolder.CountedItem>() {new DataHolder.CountedItem("iron", 1)});
            var position = new Position(0, 0);
            var construction = FactoryBuilder.StartConstruction(dummyBuildingData, 0, position);
            
            Assert.IsTrue(Grid.s.GetTile(position).simObject is Construction);
            Assert.IsFalse(Grid.s.GetTile(position).simObject is Building);
            
            var storage = CreateStorage(10,true);
            var drone = FactoryBuilder.CreateDrone(new Position(0, 0));
            
            FactoryDrones.UpdateTasks();
            
            FactoryDrones.UpdateDrone(drone);
            FactoryDrones.UpdateDrone(drone);
            FactoryDrones.UpdateDrone(drone);
            FactoryDrones.UpdateDrone(drone);
            FactoryDrones.UpdateDrone(drone);
            FactoryDrones.UpdateDrone(drone);
            FactoryDrones.UpdateDrone(drone);
            FactoryDrones.UpdateDrone(drone);
            FactoryDrones.UpdateDrone(drone);
            FactoryDrones.UpdateDrone(drone);
            FactoryDrones.UpdateDrone(drone);
            FactoryDrones.UpdateDrone(drone);
            
            Assert.IsFalse(Grid.s.GetTile(position).simObject is Construction);
            Assert.IsTrue(Grid.s.GetTile(position).simObject is Building);
        }
        
        
        [Test]
        public void TestDroneAcceptingTask() {
            var dummyBuildingData = GetDummyBuildingDataForConstruction(new List<DataHolder.CountedItem>() {new DataHolder.CountedItem("iron", 1)});
            var position = new Position(0, 0);
            var construction = FactoryBuilder.StartConstruction(dummyBuildingData, 0, position);
            
            Assert.IsTrue(Grid.s.GetTile(position).simObject is Construction);
            Assert.IsFalse(Grid.s.GetTile(position).simObject is Building);
            
            var storage = CreateStorage(10,true);
            var drone = FactoryBuilder.CreateDrone(new Position(0, 0));
            
            Assert.IsNull(drone.currentTask);
            
            FactoryDrones.UpdateTasks();
            
            Assert.IsNotNull(drone.currentTask);
        }
        
        [Test]
        public void TestDronePickingUpMaterialFromStorage() {
            var dummyBuildingData = GetDummyBuildingDataForConstruction(new List<DataHolder.CountedItem>() {new DataHolder.CountedItem("iron", 1)});
            var position = new Position(0, 0);
            var construction = FactoryBuilder.StartConstruction(dummyBuildingData, 0, position);
            
            Assert.IsTrue(Grid.s.GetTile(position).simObject is Construction);
            Assert.IsFalse(Grid.s.GetTile(position).simObject is Building);
            
            var storage = CreateStorage(10,true);
            var drone = FactoryBuilder.CreateDrone(new Position(0, 0));
            
            Assert.IsNull(drone.currentTask);
            Assert.AreEqual("DroneIdleState",drone.myState.ToString());
            
            FactoryDrones.UpdateTasks();
            
            Assert.IsNotNull(drone.currentTask);
            Assert.IsTrue(drone.isBusy);
            
            Assert.AreEqual("DroneBeginConstruction",drone.myState.ToString());
            FactoryDrones.UpdateDrone(drone);
            Assert.AreEqual("DroneSearchItemForConstruction" ,drone.myState.ToString());
            FactoryDrones.UpdateDrone(drone);
            Assert.AreEqual("DroneTravelToItemStorage",drone.myState.ToString());
            FactoryDrones.UpdateDrone(drone);
            Assert.AreEqual("DroneTakeItemFromStorage",drone.myState.ToString());
            FactoryDrones.UpdateDrone(drone); 
            // We do this twice, once for getting the item once for checking if we have all the items
            Assert.AreEqual("DroneTakeItemFromStorage",drone.myState.ToString());
            FactoryDrones.UpdateDrone(drone);
            // We also 'search for item' again to make sure we have all the items needed
            Assert.AreEqual("DroneSearchItemForConstruction",drone.myState.ToString());
            FactoryDrones.UpdateDrone(drone);
            Assert.AreEqual("DroneTravelToConstruction",drone.myState.ToString());
            FactoryDrones.UpdateDrone(drone);
            Assert.AreEqual("DroneConstruct",drone.myState.ToString());
            FactoryDrones.UpdateDrone(drone);
            Assert.AreEqual("DroneConstruct",drone.myState.ToString());
            FactoryDrones.UpdateDrone(drone);
            Assert.AreEqual("DroneIdleState",drone.myState.ToString());
            
            Assert.IsFalse(Grid.s.GetTile(position).simObject is Construction);
            Assert.IsTrue(Grid.s.GetTile(position).simObject is Building);
        }
        
        
        
    }
}
