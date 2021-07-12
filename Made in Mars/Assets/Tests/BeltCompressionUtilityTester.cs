using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static Tests.DummyFactory;

namespace Tests {
	public class BeltCompressionUtilityTester {

		public static bool BeltSegmentEqualityChecker(List<Belt.BeltSegment> first, List<Belt.BeltSegment> second) {

			if (first.Count == second.Count) {
				for (int i = 0; i < first.Count; i++) {
					if (first[i].item != second[i].item) {
						return false;
					}

					if (first[i].count != second[i].count) {
						return false;
					}
				}
			} else {
				return false;
			}

			return true;
		}
		

		[Test]
		public void TestDecompressBelt() {
			var item1 = new Item().MakeDummyItem(0);
			var item2 = new Item().MakeDummyItem(1);
			var empty = Item.GetEmpty();

			var input = new List<Belt.BeltSegment>() {
				new Belt.BeltSegment() {count = 2, item = empty},
				new Belt.BeltSegment() {count = 1, item = item1},
				new Belt.BeltSegment() {count = 3, item = item2},
				new Belt.BeltSegment() {count = 2, item = empty},
			};

			var output = new List<Item>() {
				empty, empty,
				item1,
				item2, item2, item2,
				empty, empty
			};
			
			Assert.AreEqual(output, BeltCompressionUtility.DecompressBelt(input));
		}
		
		[Test]
		public void TestCompressItemsToBelt() {
			var item1 = new Item().MakeDummyItem(0);
			var item2 = new Item().MakeDummyItem(1);
			var empty = Item.GetEmpty();

			var output = new List<Belt.BeltSegment>() {
				new Belt.BeltSegment() {count = 2, item = empty},
				new Belt.BeltSegment() {count = 1, item = item1},
				new Belt.BeltSegment() {count = 3, item = item2},
				new Belt.BeltSegment() {count = 2, item = empty},
			};

			var input = new List<Item>() {
				empty, empty,
				item1,
				item2, item2, item2,
				empty, empty
			};

			Assert.IsTrue(BeltSegmentEqualityChecker(output, BeltCompressionUtility.CompressItemsToBelt(input)));
		}
		
		[Test]
		public void TestCompressItemsToBeltWithSkip() {
			var item1 = new Item().MakeDummyItem(0);
			var item2 = new Item().MakeDummyItem(1);
			var empty = Item.GetEmpty();

			// skip the first 4, do 8, and skip the rest
			var output = new List<Belt.BeltSegment>() {
				new Belt.BeltSegment() {count = 2, item = item2},
				new Belt.BeltSegment() {count = 2, item = empty},
				new Belt.BeltSegment() {count = 3, item = item1},
				new Belt.BeltSegment() {count = 1, item = empty},
			};

			var input = new List<Item>() {
				empty, empty,
				item1,
				item2, item2, item2,
				empty, empty,
				item1, item1, item1,
				empty,empty,empty,empty,empty,
				item2,
				item1,
			};
			
			Assert.IsTrue(BeltSegmentEqualityChecker(output, BeltCompressionUtility.CompressItemsToBelt(input, 1, 3)));
		}
		
		
		/*[Test]
		public void TestCompressItemsToInventorySlots() {
			var item1 = new Item().MakeDummyItem(0);
			var item2 = new Item().MakeDummyItem(1);
			var empty = Item.GetEmpty();

			var output = new List<InventoryItemSlot>() {
				new InventoryItemSlot()
			};

			var input = new List<Item>() {
				empty, empty,
				item1,
				item2, item2, item2,
				empty, empty
			};
			
			Assert.AreEqual(output, BeltCompressionUtility.CompressItemsToInventorySlots(input));
		}*/
	}
}