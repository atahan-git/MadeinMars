using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class FactorySystemBuildingTester {


	[SetUp]
	public void Setup() {
		Grid.s = new Grid();
		Grid.s.DummySetup(25);
		FactoryMaster.s = new FactoryMaster();
	}
	
	

	[TearDown]
	public void Teardown() {
		Grid.s = null;
		FactoryMaster.s = null;
	}


	[Test]
	public void PlaceSingleBelt() {
		// Arrange
		var positions = new[] {
			new Position(0, 0),
			new Position(5, 5),
			new Position(10, 10),
			new Position(15,15),
			new Position(20,20)
		};

		var directions = new[] {
			1, 1, 2, 3, 4
		};
		
		
		// Act
		for (int i = 0; i < positions.Length; i++) {
			FactoryBuilder.CreateBelt(positions[i], directions[i]);
		}
		
		
		// Assert
		Assert.AreEqual(FactoryMaster.s.GetBelts().Count, 5);
		
		for (int i = 0; i < positions.Length; i++) {
			Assert.AreEqual(FactoryMaster.s.GetBelts()[i].startPos, positions[i]);
			Assert.AreEqual(FactoryMaster.s.GetBelts()[i].endPos, positions[i]);
			Assert.AreEqual(FactoryMaster.s.GetBelts()[i].direction, directions[i]);
			Assert.AreEqual(FactoryMaster.SlotPerSegment, FactoryMaster.s.GetBelts()[i].items[0].count);
		}
	}

	[Test]
	public void PlaceBeltAtTheEndOfBelts() {
		// Arrange
		int count = 0;
		
		// Act
		// 1 continuous belt of length 2
		FactoryBuilder.CreateBelt(new Position(0, 0), 1);
		FactoryBuilder.CreateBelt(new Position(0, 1), 1);
		count += 1;
		Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
		Assert.AreEqual(2*FactoryMaster.SlotPerSegment, FactoryMaster.s.GetBelts()[0].items[0].count);

		
		// 1 continuous belt of length 3
		FactoryBuilder.CreateBelt(new Position(5, 0), 2);
		FactoryBuilder.CreateBelt(new Position(6, 0), 2);
		FactoryBuilder.CreateBelt(new Position(7, 0), 2);
		count += 1;
		Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
		Assert.AreEqual(3*FactoryMaster.SlotPerSegment, FactoryMaster.s.GetBelts()[1].items[0].count);
		
		
		// 1 continuous belt of length 1
		// 1 separate belt
		FactoryBuilder.CreateBelt(new Position(5, 5), 2);
		FactoryBuilder.CreateBelt(new Position(6, 5), 2);
		FactoryBuilder.CreateBelt(new Position(5, 6), 1);
		count += 1 + 1;
		Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
		
		// 3 separate belt
		FactoryBuilder.CreateBelt(new Position(10, 10), 1);
		FactoryBuilder.CreateBelt(new Position(12, 10), 1);
		FactoryBuilder.CreateBelt(new Position(11, 10), 1);
		count += 3;
		Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);


		// Assert
		Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
	}

	[Test]
	public void PlaceBeltAtTheStartOfBelts() {
		// Arrange
		int count = 0;
		
		
		// Act
		// 1 continuous belt of length 2
		FactoryBuilder.CreateBelt(new Position(0, 1), 1);
		FactoryBuilder.CreateBelt(new Position(0, 0), 1);
		count += 1;
		Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
		Assert.AreEqual(2*FactoryMaster.SlotPerSegment, FactoryMaster.s.GetBelts()[0].items[0].count);

		
		// 1 continuous belt of length 3
		FactoryBuilder.CreateBelt(new Position(7, 0), 2);
		FactoryBuilder.CreateBelt(new Position(6, 0), 2);
		FactoryBuilder.CreateBelt(new Position(5, 0), 2);
		count += 1;
		Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
		Assert.AreEqual(3*FactoryMaster.SlotPerSegment, FactoryMaster.s.GetBelts()[1].items[0].count);
		
		
		// 1 continuous belt of length 1
		// 1 separate belt
		FactoryBuilder.CreateBelt(new Position(6, 5), 2);
		FactoryBuilder.CreateBelt(new Position(5, 5), 2);
		FactoryBuilder.CreateBelt(new Position(5, 6), 2);
		count += 1 + 1;
		Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
		
		// 3 separate belt
		FactoryBuilder.CreateBelt(new Position(10, 10), 2);
		FactoryBuilder.CreateBelt(new Position(11, 10), 1);
		FactoryBuilder.CreateBelt(new Position(12, 10), 2);
		count += 3;
		Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);


		// Assert
		Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
	}

	[Test]
	public void PlaceBeltAtTheMiddleOfBelts() {
		// Arrange
		int count = 0;
		
		
		// Act
		// 1 continuous belt of length 3
		FactoryBuilder.CreateBelt(new Position(5, 0), 2);
		FactoryBuilder.CreateBelt(new Position(7, 0), 2);
		FactoryBuilder.CreateBelt(new Position(6, 0), 2);
		count += 1;
		Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
		Assert.AreEqual(3*FactoryMaster.SlotPerSegment, FactoryMaster.s.GetBelts()[0].items[0].count);
		
		
		// 1 continuous belt of length 3
		FactoryBuilder.CreateBelt(new Position(0, 5), 3);
		FactoryBuilder.CreateBelt(new Position(0, 7), 3);
		FactoryBuilder.CreateBelt(new Position(0, 6), 3);
		count += 1;
		Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
		Assert.AreEqual(3*FactoryMaster.SlotPerSegment, FactoryMaster.s.GetBelts()[1].items[0].count);
		
		// 3 separate belts
		FactoryBuilder.CreateBelt(new Position(0, 10), 1);
		FactoryBuilder.CreateBelt(new Position(0, 12), 1);
		FactoryBuilder.CreateBelt(new Position(0, 11), 3);
		count += 3;
		Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
		Assert.AreEqual(FactoryMaster.SlotPerSegment, FactoryMaster.s.GetBelts()[2].items[0].count);


		// Assert
		Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
	}

	
	
	
	// -------------------------------------------------------------------------------- connectors
	
	
	
	[Test]
	public void PlaceSingleConnector() {
		// Arrange
		
		
		// Act
		var positions = new[] {
			new Position(0, 0),
			new Position(5, 5),
			new Position(10, 10),
			new Position(15,15),
			new Position(20,20)
		};

		var directions = new[] {
			0, 1, 2, 3, 4
		};


		for (int i = 0; i < positions.Length; i++) {
			FactoryBuilder.CreateConnector(positions[i], directions[i]);
		}
		
		
		// Assert
		Assert.AreEqual(FactoryMaster.s.GetConnectors().Count, 5);
		
		for (int i = 0; i < positions.Length; i++) {
			Assert.AreEqual(FactoryMaster.s.GetConnectors()[i].startPos, positions[i]);
			Assert.AreEqual(FactoryMaster.s.GetConnectors()[i].endPos, positions[i]);
			Assert.AreEqual(FactoryMaster.s.GetConnectors()[i].direction, directions[i]);
		}
	}

	[Test]
	public void PlaceConnectorAtTheEndOfConnectors() {
		// Arrange
		int count = 0;
		
		// Act
		// 1 continuous belt of length 2
		FactoryBuilder.CreateConnector(new Position(0, 0), 1);
		FactoryBuilder.CreateConnector(new Position(0, 1), 1);
		count += 1;
		Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);
		
		
		// 1 continuous belt of length 2
		FactoryBuilder.CreateConnector(new Position(10, 0), 1);
		FactoryBuilder.CreateConnector(new Position(10, 1), 0);
		count += 1;
		Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);
		
		
		// 1 continuous belt of length 2
		FactoryBuilder.CreateConnector(new Position(15, 0), 0);
		FactoryBuilder.CreateConnector(new Position(15, 1), 0);
		count += 1;
		Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);

		
		// 1 continuous belt of length 3
		FactoryBuilder.CreateConnector(new Position(5, 0), 2);
		FactoryBuilder.CreateConnector(new Position(6, 0), 2);
		FactoryBuilder.CreateConnector(new Position(7, 0), 2);
		count += 1;
		Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);
		
		
		FactoryBuilder.CreateConnector(new Position(5, 10), 2);
		FactoryBuilder.CreateConnector(new Position(6, 10), 2);
		FactoryBuilder.CreateConnector(new Position(7, 10), 0);
		count += 1;
		Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);
		
		
		FactoryBuilder.CreateConnector(new Position(5, 15), 0);
		FactoryBuilder.CreateConnector(new Position(6, 15), 0);
		FactoryBuilder.CreateConnector(new Position(7, 15), 0);
		count += 1;
		Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);
		
		
		// 1 continuous belt of length 1
		// 1 separate belt
		FactoryBuilder.CreateConnector(new Position(5, 5), 2);
		FactoryBuilder.CreateConnector(new Position(6, 5), 2);
		FactoryBuilder.CreateConnector(new Position(5, 6), 1);
		count += 1 + 1;
		Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);
		
		// 3 separate belt
		FactoryBuilder.CreateConnector(new Position(10, 10), 1);
		FactoryBuilder.CreateConnector(new Position(12, 10), 1);
		FactoryBuilder.CreateConnector(new Position(11, 10), 1);
		count += 3;
		Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);


		// Assert
		Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);
	}

	[Test]
	public void PlaceConnectorAtTheStartOfConnectors() {
		
		// Act
		// 1 continuous belt of length 2
		FactoryBuilder.CreateConnector(new Position(0, 1), 1);
		FactoryBuilder.CreateConnector(new Position(0, 0), 1);

		
		// 1 continuous belt of length 2
		FactoryBuilder.CreateConnector(new Position(10, 1), 1);
		FactoryBuilder.CreateConnector(new Position(10, 0), 0);
		
		
		// 1 continuous belt of length 3
		FactoryBuilder.CreateConnector(new Position(7, 0), 2);
		FactoryBuilder.CreateConnector(new Position(6, 0), 2);
		FactoryBuilder.CreateConnector(new Position(5, 0), 2);
		
		
		// 1 continuous belt of length 3
		FactoryBuilder.CreateConnector(new Position(7, 10), 2);
		FactoryBuilder.CreateConnector(new Position(6, 10), 2);
		FactoryBuilder.CreateConnector(new Position(5, 10), 0);
		
		
		// 1 continuous belt of length 1
		// 1 separate belt
		FactoryBuilder.CreateConnector(new Position(6, 5), 1);
		FactoryBuilder.CreateConnector(new Position(5, 5), 0);
		FactoryBuilder.CreateConnector(new Position(5, 6), 2);
		
		// 3 separate belt
		FactoryBuilder.CreateConnector(new Position(10, 10), 2);
		FactoryBuilder.CreateConnector(new Position(12, 10), 1);
		FactoryBuilder.CreateConnector(new Position(11, 10), 2);
		


		// Assert
		Assert.AreEqual(FactoryMaster.s.GetConnectors().Count, 1+1+1+ 1 + 1+1 + 3);
	}

	[Test]
	public void PlaceConnectorAtTheMiddleOfConnectors() {
		// Arrange
		int count = 0;
		
		
		// Act
		// 1 continuous belt of length 3
		FactoryBuilder.CreateConnector(new Position(5, 0), 2);
		FactoryBuilder.CreateConnector(new Position(7, 0), 2);
		FactoryBuilder.CreateConnector(new Position(6, 0), 2);
		count += 1;
		Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);
		
		
		// 1 continuous belt of length 3
		FactoryBuilder.CreateConnector(new Position(0, 5), 1);
		FactoryBuilder.CreateConnector(new Position(0, 7), 1);
		FactoryBuilder.CreateConnector(new Position(0, 6), 1);
		count += 1;
		Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);
		
		// 3 separate belts
		FactoryBuilder.CreateConnector(new Position(0, 10), 1);
		FactoryBuilder.CreateConnector(new Position(0, 12), 1);
		FactoryBuilder.CreateConnector(new Position(0, 11), 2);
		count += 3;
		Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);


		// Assert
		Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);
	}
	
	
}
