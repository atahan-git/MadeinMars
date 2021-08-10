using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static Tests.DummyFactory;

namespace Tests {
	public class FactoryBuilderBeltsTester {


		[SetUp]
		public void Setup() {
			SetUpGridAndFactoryMaster();
		}



		[TearDown]
		public void Teardown() {
			TearDownGridAndFactoryMaster();
		}


		[Test]
		public void PlaceSingleBelt() {
			// Arrange
			var positions = new[] {
				new Position(0, 0),
				new Position(5, 5),
				new Position(10, 10),
				new Position(15, 15),
				new Position(20, 20)
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
			Assert.AreEqual(2 * FactoryMaster.SlotPerSegment, FactoryMaster.s.GetBelts()[0].items[0].count);


			// 1 continuous belt of length 3
			FactoryBuilder.CreateBelt(new Position(5, 0), 2);
			FactoryBuilder.CreateBelt(new Position(6, 0), 2);
			FactoryBuilder.CreateBelt(new Position(7, 0), 2);
			count += 1;
			Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
			Assert.AreEqual(3 * FactoryMaster.SlotPerSegment, FactoryMaster.s.GetBelts()[1].items[0].count);


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
			Assert.AreEqual(2 * FactoryMaster.SlotPerSegment, FactoryMaster.s.GetBelts()[0].items[0].count);


			// 1 continuous belt of length 3
			FactoryBuilder.CreateBelt(new Position(7, 0), 2);
			FactoryBuilder.CreateBelt(new Position(6, 0), 2);
			FactoryBuilder.CreateBelt(new Position(5, 0), 2);
			count += 1;
			Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
			Assert.AreEqual(3 * FactoryMaster.SlotPerSegment, FactoryMaster.s.GetBelts()[1].items[0].count);


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
			Assert.AreEqual(3 * FactoryMaster.SlotPerSegment, FactoryMaster.s.GetBelts()[0].items[0].count);


			// 1 continuous belt of length 3
			FactoryBuilder.CreateBelt(new Position(0, 5), 3);
			FactoryBuilder.CreateBelt(new Position(0, 7), 3);
			FactoryBuilder.CreateBelt(new Position(0, 6), 3);
			count += 1;
			Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
			Assert.AreEqual(3 * FactoryMaster.SlotPerSegment, FactoryMaster.s.GetBelts()[1].items[0].count);

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
		
		
		// -------------------------------------------------------------------- Removing



		int CountBeltSlots(Belt belt) {
			int count = 0;
			foreach (var segment in belt.items) {
				count += segment.count;
			}

			return count;
		}
		
		void AssertBeltLengths() {
			foreach (var belt in FactoryMaster.s.GetBelts()) {
				var distance = Position.Distance(belt.startPos, belt.endPos);
				Assert.AreEqual(distance+1, belt.length);
			}
		}

		void AssertBeltSlotCounts() {
			AssertBeltLengths();
			foreach (var belt in FactoryMaster.s.GetBelts()) {
				Assert.AreEqual(belt.length*4, CountBeltSlots(belt));
			}
		}


		[Test]
		public void RemoveSingleBelt() {
			// Arrange
			var positions = new[] {
				new Position(0, 0),
				new Position(5, 5),
				new Position(10, 10),
				new Position(15, 15),
				new Position(20, 20)
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
			Assert.AreEqual(0, FactoryMaster.s.GetBelts().Count);

			for (int i = 0; i < positions.Length; i++) {
				Assert.IsFalse(Grid.s.GetTile(positions[i]).simObject is Belt);
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

			FactoryBuilder.RemoveBelt(new Position(0, 1));
			count += 1;
			Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
			AssertBeltSlotCounts();

			// 1 continuous belt of length 2
			FactoryBuilder.CreateBelt(new Position(5, 0), 2);
			FactoryBuilder.CreateBelt(new Position(6, 0), 2);
			FactoryBuilder.CreateBelt(new Position(7, 0), 2);

			FactoryBuilder.RemoveBelt(new Position(7, 0));

			count += 1;
			Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
			AssertBeltSlotCounts();


			// 3 belt of lenght 1
			FactoryBuilder.CreateBelt(new Position(10, 10), 1);
			FactoryBuilder.CreateBelt(new Position(12, 10), 1);
			FactoryBuilder.CreateBelt(new Position(11, 10), 1);
			count += 3;
			Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
			AssertBeltSlotCounts();


			// 2 belts of length 1
			FactoryBuilder.RemoveBelt(new Position(12, 10));
			count -= 1;

			Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
			AssertBeltSlotCounts();
		}

		[Test]
		public void RemoveBeltAtTheStartOfBelts() {
			// Arrange
			int count = 0;

			// Act
			// 1 belt remaining
			FactoryBuilder.CreateBelt(new Position(0, 0), 1);
			FactoryBuilder.CreateBelt(new Position(0, 1), 1);

			FactoryBuilder.RemoveBelt(new Position(0, 0));
			count += 1;
			AssertBeltSlotCounts();


			// 1 continuous belt of length 2
			FactoryBuilder.CreateBelt(new Position(5, 0), 2);
			FactoryBuilder.CreateBelt(new Position(6, 0), 2);
			FactoryBuilder.CreateBelt(new Position(7, 0), 2);

			FactoryBuilder.RemoveBelt(new Position(5, 0));

			count += 1;
			Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
			AssertBeltSlotCounts();
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

			FactoryBuilder.RemoveBelt(new Position(6, 0));

			count += 2;
			Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
			AssertBeltSlotCounts();


			// 1 belts of length 5
			FactoryBuilder.CreateBelt(new Position(5, 2), 2);
			FactoryBuilder.CreateBelt(new Position(6, 2), 2);
			FactoryBuilder.CreateBelt(new Position(7, 2), 2);
			FactoryBuilder.CreateBelt(new Position(8, 2), 2);
			FactoryBuilder.CreateBelt(new Position(9, 2), 2);


			count += 1;
			Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
			AssertBeltSlotCounts();

			FactoryBuilder.RemoveBelt(new Position(7, 2));
			// 2 belts of length 2 (only one additional belt)
			count += 1;

			Assert.AreEqual(count, FactoryMaster.s.GetBelts().Count);
			AssertBeltSlotCounts();
		}


		[Test]
		public void RemoveBeltSegmentWithItems() {
			FactoryBuilder.CreateBelt(new Position(4, 0), 2);
			FactoryBuilder.CreateBelt(new Position(5, 0), 2);
			FactoryBuilder.CreateBelt(new Position(6, 0), 2);
			FactoryBuilder.CreateBelt(new Position(7, 0), 2);
			FactoryBuilder.CreateBelt(new Position(8, 0), 2);
			FactoryBuilder.CreateBelt(new Position(9, 0), 2);

			Assert.AreEqual(1, FactoryMaster.s.GetBelts().Count);

			// insert items to the belt
			var dummy1 = new Item().MakeDummyItem(1);
			var dummy2 = new Item().MakeDummyItem(2);

			var belt = FactoryMaster.s.GetBelts()[0];
			for (int i = 0; i < 16; i++) {
				// add items in chunks
				if (i < 3 || (i > 7 && i < 10) || i > 15) {
					belt.TryAndInsertItem(dummy1);
				} else if (i < 5) {
					belt.TryAndInsertItem(dummy2);
				}


				AssertBeltSlotCounts();
			}

			// Belt at the end of the line
			FactoryBuilder.RemoveBelt(new Position(9, 0));
			AssertBeltSlotCounts();
			
			// Belt at the start of the line
			FactoryBuilder.RemoveBelt(new Position(4, 0));
			AssertBeltSlotCounts();
			
			// Belt in the middle
			FactoryBuilder.RemoveBelt(new Position(6, 0));
 			AssertBeltSlotCounts();
            
            AssertBeltSlotCounts();

		}

	}
}