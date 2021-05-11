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
		FactoryMaster.s = new FactoryMaster();
	}
	

	[TearDown]
	public void Teardown() {
		Grid.s = null;
		FactoryMaster.s = null;
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
			FactoryBuilder.CreateBelt(positions[i], directions[i]);
		}
		
		for (int i = 0; i < positions.Length; i++) {
			FactoryBuilder.RemoveBelt(positions[i]);
		}
		
		
		// Assert
		Assert.AreEqual(0,FactoryMaster.s.GetBelts().Count);
		
		for (int i = 0; i < positions.Length; i++) {
			Assert.IsFalse(Grid.s.GetTile(positions[i]).areThereBelt);
		}
	}

	[Test]
	public void RemoveBeltAtTheEndOfBelts() {
		// Arrange
		int count = 0;
		
		// Act
		// 1 belt remaining
		FactoryBuilder.CreateBelt(new Position(0, 0), 1);
		FactoryBuilder.CreateBelt(new Position(0, 1), 1);
		
		FactoryBuilder.RemoveBelt(new Position(0,1));
		count += 1;
		
		// 1 continuous belt of length 2
		FactoryBuilder.CreateBelt(new Position(5, 0), 2);
		FactoryBuilder.CreateBelt(new Position(6, 0), 2);
		FactoryBuilder.CreateBelt(new Position(7, 0), 2);
		
		FactoryBuilder.RemoveBelt(new Position(7,0));
		
		count += 1;
		Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
		
		
		// 3 belt of lenght 1
		FactoryBuilder.CreateBelt(new Position(10, 10), 1);
		FactoryBuilder.CreateBelt(new Position(12, 10), 1);
		FactoryBuilder.CreateBelt(new Position(11, 10), 1);
		count += 3;
		Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
		
		
		// 2 belts of length 1
		FactoryBuilder.RemoveBelt(new Position(12, 10));
		count -= 1;
		
		Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
	}

	[Test]
	public void RemoveBeltAtTheStartOfBelts() {
		// Arrange
		int count = 0;
		
		// Act
		// 1 belt remaining
		FactoryBuilder.CreateBelt(new Position(0, 0), 1);
		FactoryBuilder.CreateBelt(new Position(0, 1), 1);
		
		FactoryBuilder.RemoveBelt(new Position(0,0));
		count += 1;

		
		// 1 continuous belt of length 2
		FactoryBuilder.CreateBelt(new Position(5, 0), 2);
		FactoryBuilder.CreateBelt(new Position(6, 0), 2);
		FactoryBuilder.CreateBelt(new Position(7, 0), 2);
		
		FactoryBuilder.RemoveBelt(new Position(5,0));
		
		count += 1;
		Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
	}

	[Test]
	public void RemoveBeltAtTheMiddleOfBelts() {
		// Arrange
		int count = 0;
		
		// Act
		// 2 belts of length 1
		FactoryBuilder.CreateBelt(new Position(5, 0), 2);
		FactoryBuilder.CreateBelt(new Position(6, 0), 2);
		FactoryBuilder.CreateBelt(new Position(7, 0), 2);
		
		FactoryBuilder.RemoveBelt(new Position(6,0));
		
		count += 2;
		Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
		
		
		// 1 belts of length 5
		FactoryBuilder.CreateBelt(new Position(5, 0), 2);
		FactoryBuilder.CreateBelt(new Position(6, 0), 2);
		FactoryBuilder.CreateBelt(new Position(7, 0), 2);
		FactoryBuilder.CreateBelt(new Position(8, 0), 2);
		FactoryBuilder.CreateBelt(new Position(9, 0), 2);
		
		
		count += 1;
		Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
		
		FactoryBuilder.RemoveBelt(new Position(7,0));
		// 2 belts of length 2 (only one additional belt)
		count += 1;
		
		Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
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
			FactoryBuilder.CreateConnector(positions[i], directions[i]);
		}
		
		for (int i = 0; i < positions.Length; i++) {
			FactoryBuilder.RemoveConnector(positions[i]);
		}
		
		
		// Assert
		Assert.AreEqual(FactoryMaster.s.GetBelts().Count, 0);
		
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
		FactoryBuilder.CreateConnector(new Position(0, 0), 1);
		FactoryBuilder.CreateConnector(new Position(0, 1), 1);
		
		FactoryBuilder.RemoveConnector(new Position(0,1));
		
		count += 1;
		Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);

		
		// 1 continuous connector of length 2
		FactoryBuilder.CreateConnector(new Position(5, 0), 2);
		FactoryBuilder.CreateConnector(new Position(6, 0), 2);
		FactoryBuilder.CreateConnector(new Position(7, 0), 2);
		
		FactoryBuilder.RemoveConnector( new Position(7,0));
		
		count += 1;
		Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);
		
		
		// 2 separate belt
		FactoryBuilder.CreateConnector(new Position(10, 10), 1);
		FactoryBuilder.CreateConnector(new Position(12, 10), 1);
		FactoryBuilder.CreateConnector(new Position(11, 10), 1);
		
		FactoryBuilder.RemoveConnector(new Position(12, 10));
		
		
		count += 2;
		Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);
	}

	[Test]
	public void RemoveConnectorAtTheStartOfConnectors() {
		// Arrange
		int count = 0;
		
		// Act
		// 1 connector
		FactoryBuilder.CreateConnector(new Position(0, 0), 1);
		FactoryBuilder.CreateConnector(new Position(0, 1), 1);
		
		FactoryBuilder.RemoveConnector(new Position(0,0));
		
		count += 1;
		Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);

		
		// 1 continuous belt of length 2
		FactoryBuilder.CreateConnector(new Position(5, 0), 2);
		FactoryBuilder.CreateConnector(new Position(6, 0), 2);
		FactoryBuilder.CreateConnector(new Position(7, 0), 2);
		
		FactoryBuilder.RemoveConnector(new Position(5,0));
		
		count += 1;
		Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);
	}

	[Test]
	public void RemoveConnectorAtTheMiddleOfConnectors() {
		// Arrange
		int count = 0;
		
		// Act
		// 2 connectors
		FactoryBuilder.CreateConnector(new Position(0, 5), 2);
		FactoryBuilder.CreateConnector(new Position(0, 6), 2);
		FactoryBuilder.CreateConnector(new Position(0, 7), 2);
		
		FactoryBuilder.RemoveConnector(new Position(0,6));
		
		count += 2;
		Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);
		
		
		// two connectors of length 2
		FactoryBuilder.CreateConnector(new Position(5, 0), 2);
		FactoryBuilder.CreateConnector(new Position(6, 0), 2);
		FactoryBuilder.CreateConnector(new Position(7, 0), 2);
		FactoryBuilder.CreateConnector(new Position(8, 0), 2);
		FactoryBuilder.CreateConnector(new Position(9, 0), 2);
		
		FactoryBuilder.RemoveConnector(new Position(7,0));
		
		count += 2;
		Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);
	}
	
	
}
