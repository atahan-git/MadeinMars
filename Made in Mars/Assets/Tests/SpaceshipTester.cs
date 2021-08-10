using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static Tests.DummyFactory;


namespace Tests {
	public class SpaceshipTester {

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

		public Building MakeSpaceship() {
			var spaceshipData = GetDummyBuildingData();
			spaceshipData.uniqueName = "spaceship";
			spaceshipData.myType = BuildingData.ItemType.Spaceship;

			return FactoryBuilder.CreateBuilding(spaceshipData, new Position(0, 0), null);
		}


		[Test]
		public void TestCreateSpaceship() {
			var ship = MakeSpaceship();

			Assert.IsNotNull(ship);
		}
		
	}
}
 