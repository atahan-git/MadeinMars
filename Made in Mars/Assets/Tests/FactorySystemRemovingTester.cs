using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class FactorySystemRemovingTester {


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
	public void RemoveSingleBelt() {
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
		
		for (int i = 0; i < positions.Length; i++) {
			FactorySystem.s.RemoveBelt(positions[i]);
		}
		
		
		// Assert
		Assert.AreEqual(FactorySystem.s.belts.Count, 0);
		
		for (int i = 0; i < positions.Length; i++) {
			Assert.IsFalse(Grid.s.GetTile(positions[i]).areThereBelt);
		}
	}

	[Test]
	public void RemoveBeltAtTheEndOfBelts() {
		// Arrange
		int count = 0;
		
		// Act
		// no belt remaining
		FactorySystem.s.CreateBelt(new Position(0, 0), 1);
		FactorySystem.s.CreateBelt(new Position(0, 1), 1);
		
		FactorySystem.s.RemoveBelt(new Position(0,1));

		
		// 1 continuous belt of length 2
		FactorySystem.s.CreateBelt(new Position(5, 0), 2);
		FactorySystem.s.CreateBelt(new Position(6, 0), 2);
		FactorySystem.s.CreateBelt(new Position(7, 0), 2);
		
		FactorySystem.s.RemoveBelt(new Position(7,0));
		
		count += 1;
		Assert.AreEqual(count, FactorySystem.s.belts.Count);
		
		
		// 2 belts of lenght 1
		FactorySystem.s.CreateBelt(new Position(10, 10), 1);
		FactorySystem.s.CreateBelt(new Position(12, 10), 1);
		FactorySystem.s.CreateBelt(new Position(11, 10), 1);
		
		FactorySystem.s.RemoveBelt(new Position(12, 10));
		
		count += 2;
		Assert.AreEqual(count, FactorySystem.s.belts.Count);
	}

	[Test]
	public void RemoveBeltAtTheStartOfBelts() {
		// Arrange
		int count = 0;
		
		// Act
		// no belt remaining
		FactorySystem.s.CreateBelt(new Position(0, 0), 1);
		FactorySystem.s.CreateBelt(new Position(0, 1), 1);
		
		FactorySystem.s.RemoveBelt(new Position(0,0));

		
		// 1 continuous belt of length 2
		FactorySystem.s.CreateBelt(new Position(5, 0), 2);
		FactorySystem.s.CreateBelt(new Position(6, 0), 2);
		FactorySystem.s.CreateBelt(new Position(7, 0), 2);
		
		FactorySystem.s.RemoveBelt(new Position(5,0));
		
		count += 1;
		Assert.AreEqual(count, FactorySystem.s.belts.Count);
	}

	[Test]
	public void RemoveBeltAtTheMiddleOfBelts() {
		// Arrange
		int count = 0;
		
		// Act
		// no belts remaining
		FactorySystem.s.CreateBelt(new Position(5, 0), 2);
		FactorySystem.s.CreateBelt(new Position(6, 0), 2);
		FactorySystem.s.CreateBelt(new Position(7, 0), 2);
		
		FactorySystem.s.RemoveBelt(new Position(6,0));
		
		count += 0;
		Assert.AreEqual(count, FactorySystem.s.belts.Count);
		
		
		// two belts of length 2
		FactorySystem.s.CreateBelt(new Position(5, 0), 2);
		FactorySystem.s.CreateBelt(new Position(6, 0), 2);
		FactorySystem.s.CreateBelt(new Position(7, 0), 2);
		FactorySystem.s.CreateBelt(new Position(8, 0), 2);
		FactorySystem.s.CreateBelt(new Position(9, 0), 2);
		
		
		count += 1;
		Assert.AreEqual(count, FactorySystem.s.belts.Count);
		
		FactorySystem.s.RemoveBelt(new Position(7,0));
		count += 1;
		
		Assert.AreEqual(count, FactorySystem.s.belts.Count);
	}

	
	
	
	// -------------------------------------------------------------------------------- connectors
	
	
	
	[Test]
	public void RemoveSingleConnector() {
		// Arrange
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
		
		
		// Act
		for (int i = 0; i < positions.Length; i++) {
			FactorySystem.s.CreateConnector(positions[i], directions[i]);
		}
		
		for (int i = 0; i < positions.Length; i++) {
			FactorySystem.s.RemoveConnector(positions[i]);
		}
		
		
		// Assert
		Assert.AreEqual(FactorySystem.s.belts.Count, 0);
		
		for (int i = 0; i < positions.Length; i++) {
			Assert.IsFalse(Grid.s.GetTile(positions[i]).areThereBelt);
		}
	}

	[Test]
	public void RemoveConnectorAtTheEndOfConnectors() {
		// Arrange
		int count = 0;
		
		// Act
		// 1 connector remains
		FactorySystem.s.CreateConnector(new Position(0, 0), 1);
		FactorySystem.s.CreateConnector(new Position(0, 1), 1);
		
		FactorySystem.s.RemoveConnector(new Position(0,1));
		
		count += 1;
		Assert.AreEqual(count, FactorySystem.s.connectors.Count);

		
		// 1 continuous connector of length 2
		FactorySystem.s.CreateConnector(new Position(5, 0), 2);
		FactorySystem.s.CreateConnector(new Position(6, 0), 2);
		FactorySystem.s.CreateConnector(new Position(7, 0), 2);
		
		FactorySystem.s.RemoveConnector( new Position(7,0));
		
		count += 1;
		Assert.AreEqual(count, FactorySystem.s.connectors.Count);
		
		
		// 2 separate belt
		FactorySystem.s.CreateConnector(new Position(10, 10), 1);
		FactorySystem.s.CreateConnector(new Position(12, 10), 1);
		FactorySystem.s.CreateConnector(new Position(11, 10), 1);
		
		FactorySystem.s.RemoveConnector(new Position(12, 10));
		
		
		count += 2;
		Assert.AreEqual(count, FactorySystem.s.connectors.Count);
	}

	[Test]
	public void RemoveConnectorAtTheStartOfConnectors() {
		// Arrange
		int count = 0;
		
		// Act
		// 1 connector
		FactorySystem.s.CreateConnector(new Position(0, 0), 1);
		FactorySystem.s.CreateConnector(new Position(0, 1), 1);
		
		FactorySystem.s.RemoveConnector(new Position(0,0));
		
		count += 1;
		Assert.AreEqual(count, FactorySystem.s.connectors.Count);

		
		// 1 continuous belt of length 2
		FactorySystem.s.CreateConnector(new Position(5, 0), 2);
		FactorySystem.s.CreateConnector(new Position(6, 0), 2);
		FactorySystem.s.CreateConnector(new Position(7, 0), 2);
		
		FactorySystem.s.RemoveConnector(new Position(5,0));
		
		count += 1;
		Assert.AreEqual(count, FactorySystem.s.connectors.Count);
	}

	[Test]
	public void RemoveConnectorAtTheMiddleOfConnectors() {
		// Arrange
		int count = 0;
		
		// Act
		// 2 connectors
		FactorySystem.s.CreateConnector(new Position(0, 5), 2);
		FactorySystem.s.CreateConnector(new Position(0, 6), 2);
		FactorySystem.s.CreateConnector(new Position(0, 7), 2);
		
		FactorySystem.s.RemoveConnector(new Position(0,6));
		
		count += 2;
		Assert.AreEqual(count, FactorySystem.s.connectors.Count);
		
		
		// two connectors of length 2
		FactorySystem.s.CreateConnector(new Position(5, 0), 2);
		FactorySystem.s.CreateConnector(new Position(6, 0), 2);
		FactorySystem.s.CreateConnector(new Position(7, 0), 2);
		FactorySystem.s.CreateConnector(new Position(8, 0), 2);
		FactorySystem.s.CreateConnector(new Position(9, 0), 2);
		
		FactorySystem.s.RemoveConnector(new Position(7,0));
		
		count += 2;
		Assert.AreEqual(count, FactorySystem.s.connectors.Count);
	}
	
	
}
