using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static Tests.DummyFactory;


namespace Tests {
	public class FactoryBuilderConnectorsTester {

		[SetUp]
		public void Setup() {
			SetUpGridAndFactoryMaster();
		}



		[TearDown]
		public void Teardown() {
			TearDownGridAndFactoryMaster();
		}


		[Test]
		public void PlaceSingleConnector() {
			// Arrange


			// Act
			var positions = new[] {
				new Position(0, 0),
				new Position(5, 5),
				new Position(10, 10),
				new Position(15, 15),
				new Position(20, 20)
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
			Assert.AreEqual(FactoryMaster.s.GetConnectors().Count, 1 + 1 + 1 + 1 + 1 + 1 + 3);
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



		// -------------------------------------------------------------------- Removing


		[Test]
		public void RemoveSingleConnector() {
			// Arrange
			var positions = new[] {
				new Position(0, 0),
				new Position(5, 5),
				new Position(10, 10),
				new Position(15, 15),
				new Position(20, 20)
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
				Assert.IsFalse(Grid.s.GetTile(positions[i]).simObject is Belt);
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

			FactoryBuilder.RemoveConnector(new Position(0, 1));

			count += 1;
			Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);


			// 1 continuous connector of length 2
			FactoryBuilder.CreateConnector(new Position(5, 0), 2);
			FactoryBuilder.CreateConnector(new Position(6, 0), 2);
			FactoryBuilder.CreateConnector(new Position(7, 0), 2);

			FactoryBuilder.RemoveConnector(new Position(7, 0));

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

			FactoryBuilder.RemoveConnector(new Position(0, 0));

			count += 1;
			Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);


			// 1 continuous belt of length 2
			FactoryBuilder.CreateConnector(new Position(5, 0), 2);
			FactoryBuilder.CreateConnector(new Position(6, 0), 2);
			FactoryBuilder.CreateConnector(new Position(7, 0), 2);

			FactoryBuilder.RemoveConnector(new Position(5, 0));

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

			FactoryBuilder.RemoveConnector(new Position(0, 6));

			count += 2;
			Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);


			// two connectors of length 2
			FactoryBuilder.CreateConnector(new Position(5, 0), 2);
			FactoryBuilder.CreateConnector(new Position(6, 0), 2);
			FactoryBuilder.CreateConnector(new Position(7, 0), 2);
			FactoryBuilder.CreateConnector(new Position(8, 0), 2);
			FactoryBuilder.CreateConnector(new Position(9, 0), 2);

			FactoryBuilder.RemoveConnector(new Position(7, 0));

			count += 2;
			Assert.AreEqual(count, FactoryMaster.s.GetConnectors().Count);
		}

	}
}