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
		FactorySystem.s = new FactorySystem();
	}
	
	

	[TearDown]
	public void Teardown() {
		Grid.s = null;
		FactorySystem.s = null;
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
			FactorySystem.s.CreateBelt(positions[i], directions[i]);
		}
		
		
		// Assert
		Assert.AreEqual(FactorySystem.s.belts.Count, 5);
		
		for (int i = 0; i < positions.Length; i++) {
			Assert.AreEqual(FactorySystem.s.belts[i].startPos, positions[i]);
			Assert.AreEqual(FactorySystem.s.belts[i].endPos, positions[i]);
			Assert.AreEqual(FactorySystem.s.belts[i].direction, directions[i]);
			Assert.AreEqual(FactorySystem.SlotPerSegment, FactorySystem.s.belts[i].items[0].count);
		}
	}

	[Test]
	public void PlaceBeltAtTheEndOfBelts() {
		// Arrange
		int count = 0;
		
		// Act
		// 1 continuous belt of length 2
		FactorySystem.s.CreateBelt(new Position(0, 0), 1);
		FactorySystem.s.CreateBelt(new Position(0, 1), 1);
		count += 1;
		Assert.AreEqual(count, FactorySystem.s.belts.Count);
		Assert.AreEqual(2*FactorySystem.SlotPerSegment, FactorySystem.s.belts[0].items[0].count);

		
		// 1 continuous belt of length 3
		FactorySystem.s.CreateBelt(new Position(5, 0), 2);
		FactorySystem.s.CreateBelt(new Position(6, 0), 2);
		FactorySystem.s.CreateBelt(new Position(7, 0), 2);
		count += 1;
		Assert.AreEqual(count, FactorySystem.s.belts.Count);
		Assert.AreEqual(3*FactorySystem.SlotPerSegment, FactorySystem.s.belts[1].items[0].count);
		
		
		// 1 continuous belt of length 1
		// 1 separate belt
		FactorySystem.s.CreateBelt(new Position(5, 5), 2);
		FactorySystem.s.CreateBelt(new Position(6, 5), 2);
		FactorySystem.s.CreateBelt(new Position(5, 6), 1);
		count += 1 + 1;
		Assert.AreEqual(count, FactorySystem.s.belts.Count);
		
		// 3 separate belt
		FactorySystem.s.CreateBelt(new Position(10, 10), 1);
		FactorySystem.s.CreateBelt(new Position(12, 10), 1);
		FactorySystem.s.CreateBelt(new Position(11, 10), 1);
		count += 3;
		Assert.AreEqual(count, FactorySystem.s.belts.Count);


		// Assert
		Assert.AreEqual(count, FactorySystem.s.belts.Count);
	}

	[Test]
	public void PlaceBeltAtTheStartOfBelts() {
		// Arrange
		int count = 0;
		
		
		// Act
		// 1 continuous belt of length 2
		FactorySystem.s.CreateBelt(new Position(0, 1), 1);
		FactorySystem.s.CreateBelt(new Position(0, 0), 1);
		count += 1;
		Assert.AreEqual(count, FactorySystem.s.belts.Count);
		Assert.AreEqual(2*FactorySystem.SlotPerSegment, FactorySystem.s.belts[0].items[0].count);

		
		// 1 continuous belt of length 3
		FactorySystem.s.CreateBelt(new Position(7, 0), 2);
		FactorySystem.s.CreateBelt(new Position(6, 0), 2);
		FactorySystem.s.CreateBelt(new Position(5, 0), 2);
		count += 1;
		Assert.AreEqual(count, FactorySystem.s.belts.Count);
		Assert.AreEqual(3*FactorySystem.SlotPerSegment, FactorySystem.s.belts[1].items[0].count);
		
		
		// 1 continuous belt of length 1
		// 1 separate belt
		FactorySystem.s.CreateBelt(new Position(6, 5), 2);
		FactorySystem.s.CreateBelt(new Position(5, 5), 2);
		FactorySystem.s.CreateBelt(new Position(5, 6), 2);
		count += 1 + 1;
		Assert.AreEqual(count, FactorySystem.s.belts.Count);
		
		// 3 separate belt
		FactorySystem.s.CreateBelt(new Position(10, 10), 2);
		FactorySystem.s.CreateBelt(new Position(11, 10), 1);
		FactorySystem.s.CreateBelt(new Position(12, 10), 2);
		count += 3;
		Assert.AreEqual(count, FactorySystem.s.belts.Count);


		// Assert
		Assert.AreEqual(count, FactorySystem.s.belts.Count);
	}

	[Test]
	public void PlaceBeltAtTheMiddleOfBelts() {
		// Arrange
		int count = 0;
		
		
		// Act
		// 1 continuous belt of length 3
		FactorySystem.s.CreateBelt(new Position(5, 0), 2);
		FactorySystem.s.CreateBelt(new Position(7, 0), 2);
		FactorySystem.s.CreateBelt(new Position(6, 0), 2);
		count += 1;
		Assert.AreEqual(count, FactorySystem.s.belts.Count);
		Assert.AreEqual(3*FactorySystem.SlotPerSegment, FactorySystem.s.belts[0].items[0].count);
		
		
		// 1 continuous belt of length 3
		FactorySystem.s.CreateBelt(new Position(0, 5), 3);
		FactorySystem.s.CreateBelt(new Position(0, 7), 3);
		FactorySystem.s.CreateBelt(new Position(0, 6), 3);
		count += 1;
		Assert.AreEqual(count, FactorySystem.s.belts.Count);
		Assert.AreEqual(3*FactorySystem.SlotPerSegment, FactorySystem.s.belts[1].items[0].count);
		
		// 3 separate belts
		FactorySystem.s.CreateBelt(new Position(0, 10), 1);
		FactorySystem.s.CreateBelt(new Position(0, 12), 1);
		FactorySystem.s.CreateBelt(new Position(0, 11), 3);
		count += 3;
		Assert.AreEqual(count, FactorySystem.s.belts.Count);
		Assert.AreEqual(FactorySystem.SlotPerSegment, FactorySystem.s.belts[2].items[0].count);


		// Assert
		Assert.AreEqual(count, FactorySystem.s.belts.Count);
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
			FactorySystem.s.CreateConnector(positions[i], directions[i]);
		}
		
		
		// Assert
		Assert.AreEqual(FactorySystem.s.connectors.Count, 5);
		
		for (int i = 0; i < positions.Length; i++) {
			Assert.AreEqual(FactorySystem.s.connectors[i].startPos, positions[i]);
			Assert.AreEqual(FactorySystem.s.connectors[i].endPos, positions[i]);
			Assert.AreEqual(FactorySystem.s.connectors[i].direction, directions[i]);
		}
	}

	[Test]
	public void PlaceConnectorAtTheEndOfConnectors() {
		// Arrange
		int count = 0;
		
		// Act
		// 1 continuous belt of length 2
		FactorySystem.s.CreateConnector(new Position(0, 0), 1);
		FactorySystem.s.CreateConnector(new Position(0, 1), 1);
		count += 1;
		Assert.AreEqual(count, FactorySystem.s.connectors.Count);
		
		
		// 1 continuous belt of length 2
		FactorySystem.s.CreateConnector(new Position(10, 0), 1);
		FactorySystem.s.CreateConnector(new Position(10, 1), 0);
		count += 1;
		Assert.AreEqual(count, FactorySystem.s.connectors.Count);
		
		
		// 1 continuous belt of length 2
		FactorySystem.s.CreateConnector(new Position(15, 0), 0);
		FactorySystem.s.CreateConnector(new Position(15, 1), 0);
		count += 1;
		Assert.AreEqual(count, FactorySystem.s.connectors.Count);

		
		// 1 continuous belt of length 3
		FactorySystem.s.CreateConnector(new Position(5, 0), 2);
		FactorySystem.s.CreateConnector(new Position(6, 0), 2);
		FactorySystem.s.CreateConnector(new Position(7, 0), 2);
		count += 1;
		Assert.AreEqual(count, FactorySystem.s.connectors.Count);
		
		
		FactorySystem.s.CreateConnector(new Position(5, 10), 2);
		FactorySystem.s.CreateConnector(new Position(6, 10), 2);
		FactorySystem.s.CreateConnector(new Position(7, 10), 0);
		count += 1;
		Assert.AreEqual(count, FactorySystem.s.connectors.Count);
		
		
		FactorySystem.s.CreateConnector(new Position(5, 15), 0);
		FactorySystem.s.CreateConnector(new Position(6, 15), 0);
		FactorySystem.s.CreateConnector(new Position(7, 15), 0);
		count += 1;
		Assert.AreEqual(count, FactorySystem.s.connectors.Count);
		
		
		// 1 continuous belt of length 1
		// 1 separate belt
		FactorySystem.s.CreateConnector(new Position(5, 5), 2);
		FactorySystem.s.CreateConnector(new Position(6, 5), 2);
		FactorySystem.s.CreateConnector(new Position(5, 6), 1);
		count += 1 + 1;
		Assert.AreEqual(count, FactorySystem.s.connectors.Count);
		
		// 3 separate belt
		FactorySystem.s.CreateConnector(new Position(10, 10), 1);
		FactorySystem.s.CreateConnector(new Position(12, 10), 1);
		FactorySystem.s.CreateConnector(new Position(11, 10), 1);
		count += 3;
		Assert.AreEqual(count, FactorySystem.s.connectors.Count);


		// Assert
		Assert.AreEqual(count, FactorySystem.s.connectors.Count);
	}

	[Test]
	public void PlaceConnectorAtTheStartOfConnectors() {
		
		// Act
		// 1 continuous belt of length 2
		FactorySystem.s.CreateConnector(new Position(0, 1), 1);
		FactorySystem.s.CreateConnector(new Position(0, 0), 1);

		
		// 1 continuous belt of length 2
		FactorySystem.s.CreateConnector(new Position(10, 1), 1);
		FactorySystem.s.CreateConnector(new Position(10, 0), 0);
		
		
		// 1 continuous belt of length 3
		FactorySystem.s.CreateConnector(new Position(7, 0), 2);
		FactorySystem.s.CreateConnector(new Position(6, 0), 2);
		FactorySystem.s.CreateConnector(new Position(5, 0), 2);
		
		
		// 1 continuous belt of length 3
		FactorySystem.s.CreateConnector(new Position(7, 10), 2);
		FactorySystem.s.CreateConnector(new Position(6, 10), 2);
		FactorySystem.s.CreateConnector(new Position(5, 10), 0);
		
		
		// 1 continuous belt of length 1
		// 1 separate belt
		FactorySystem.s.CreateConnector(new Position(6, 5), 1);
		FactorySystem.s.CreateConnector(new Position(5, 5), 0);
		FactorySystem.s.CreateConnector(new Position(5, 6), 2);
		
		// 3 separate belt
		FactorySystem.s.CreateConnector(new Position(10, 10), 2);
		FactorySystem.s.CreateConnector(new Position(12, 10), 1);
		FactorySystem.s.CreateConnector(new Position(11, 10), 2);
		


		// Assert
		Assert.AreEqual(FactorySystem.s.connectors.Count, 1+1+1+ 1 + 1+1 + 3);
	}

	[Test]
	public void PlaceConnectorAtTheMiddleOfConnectors() {
		// Arrange
		int count = 0;
		
		
		// Act
		// 1 continuous belt of length 3
		FactorySystem.s.CreateConnector(new Position(5, 0), 2);
		FactorySystem.s.CreateConnector(new Position(7, 0), 2);
		FactorySystem.s.CreateConnector(new Position(6, 0), 2);
		count += 1;
		Assert.AreEqual(count, FactorySystem.s.connectors.Count);
		
		
		// 1 continuous belt of length 3
		FactorySystem.s.CreateConnector(new Position(0, 5), 1);
		FactorySystem.s.CreateConnector(new Position(0, 7), 1);
		FactorySystem.s.CreateConnector(new Position(0, 6), 1);
		count += 1;
		Assert.AreEqual(count, FactorySystem.s.connectors.Count);
		
		// 3 separate belts
		FactorySystem.s.CreateConnector(new Position(0, 10), 1);
		FactorySystem.s.CreateConnector(new Position(0, 12), 1);
		FactorySystem.s.CreateConnector(new Position(0, 11), 2);
		count += 3;
		Assert.AreEqual(count, FactorySystem.s.connectors.Count);


		// Assert
		Assert.AreEqual(count, FactorySystem.s.connectors.Count);
	}
	
	
}
