using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static Tests.DummyFactory;

namespace Tests {
    public class BeltTester {

        [Test]
        public void TestBeltCreation() {
            // Arrange
            var belt1 = new Belt(new Position(0, 0), new Position(2, 0));

            // Assert
            var correct = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 3 * FactoryMaster.SlotPerSegment, item = Item.GetEmpty()}
            };

            Assert.IsTrue(BeltSegmentEqualityChecker(belt1.items, correct));
        }


        [Test]
        public void TestBeltInsertItem() {
            // Arrange
            int numberOfCases = 8;
            var belts = new Belt[numberOfCases];

            belts[0] = new Belt(new Position(0, 0), new Position(2, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 3, item = Item.GetEmpty()}
                }

            };
            belts[1] = new Belt(new Position(0, 0), new Position(2, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                    new Belt.BeltSegment() {count = 2, item = new Item().MakeDummyItem(1)},
                }
            };
            belts[2] = new Belt(new Position(0, 0), new Position(2, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                    new Belt.BeltSegment() {count = 2, item = Item.GetEmpty()},
                }
            };
            belts[3] = new Belt(new Position(0, 0), new Position(2, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                    new Belt.BeltSegment() {count = 2, item = new Item().MakeDummyItem(5)},
                }
            };
            belts[4] = new Belt(new Position(0, 0), new Position(2, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                    new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                }
            };
            belts[5] = new Belt(new Position(0, 0), new Position(2, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                    new Belt.BeltSegment() {count = 2, item = new Item().MakeDummyItem(1)},
                }
            };
            belts[6] = new Belt(new Position(0, 0), new Position(2, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                }
            };
            belts[7] = new Belt(new Position(0, 0), new Position(2, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                }
            };

            // Act
            var results = new bool[numberOfCases];

            results[0] = belts[0].TryInsertItemToBelt(new Item().MakeDummyItem(1));
            results[1] = belts[1].TryInsertItemToBelt(new Item().MakeDummyItem(1));
            results[2] = belts[2].TryInsertItemToBelt(new Item().MakeDummyItem(1));
            results[3] = belts[3].TryInsertItemToBelt(new Item().MakeDummyItem(2));
            results[4] = belts[4].TryInsertItemToBelt(new Item().MakeDummyItem(1));
            results[5] = belts[5].TryInsertItemToBelt(Item.GetEmpty());
            results[6] = belts[6].TryInsertItemToBelt(new Item().MakeDummyItem(1));
            results[7] = belts[7].TryInsertItemToBelt(new Item().MakeDummyItem(1));



            // Assert
            var corrects = new List<Belt.BeltSegment>[numberOfCases];
            corrects[0] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                new Belt.BeltSegment() {count = 2, item = Item.GetEmpty()},
            };
            corrects[1] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 3, item = new Item().MakeDummyItem(1)},
            };
            corrects[2] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                new Belt.BeltSegment() {count = 2, item = Item.GetEmpty()},
            };
            corrects[3] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(2)},
                new Belt.BeltSegment() {count = 2, item = new Item().MakeDummyItem(5)},
            };
            corrects[4] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 2, item = new Item().MakeDummyItem(1)},
                new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
            };
            corrects[5] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                new Belt.BeltSegment() {count = 2, item = new Item().MakeDummyItem(1)},
            };
            corrects[6] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
            };
            corrects[7] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
            };

            Assert.IsTrue(results[0]);
            Assert.IsTrue(BeltSegmentEqualityChecker(belts[0].items, corrects[0]));

            Assert.IsTrue(results[1]);
            Assert.IsTrue(BeltSegmentEqualityChecker(belts[1].items, corrects[1]));

            Assert.IsFalse(results[2]);
            Assert.IsTrue(BeltSegmentEqualityChecker(belts[2].items, corrects[2]));

            Assert.IsTrue(results[3]);
            Assert.IsTrue(BeltSegmentEqualityChecker(belts[3].items, corrects[3]));

            Assert.IsTrue(results[4]);
            Assert.IsTrue(BeltSegmentEqualityChecker(belts[4].items, corrects[4]));

            Assert.IsTrue(results[5]);
            Assert.IsTrue(BeltSegmentEqualityChecker(belts[5].items, corrects[5]));

            Assert.IsTrue(results[6]);
            Assert.IsTrue(BeltSegmentEqualityChecker(belts[6].items, corrects[7]));

            Assert.IsFalse(results[7]);
            Assert.IsTrue(BeltSegmentEqualityChecker(belts[6].items, corrects[7]));
        }

        [Test]
        public void TestBeltRemoveLastItem() {
            // Arrange
            int numberOfCases = 6;
            var belts = new Belt[numberOfCases];
            belts[0] = new Belt(new Position(0, 0), new Position(2, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 2, item = Item.GetEmpty()},
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                }
            };
            belts[1] = new Belt(new Position(0, 0), new Position(2, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                    new Belt.BeltSegment() {count = 2, item = new Item().MakeDummyItem(1)},
                }
            };
            belts[2] = new Belt(new Position(0, 0), new Position(2, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 3, item = Item.GetEmpty()},
                }
            };
            belts[3] = new Belt(new Position(0, 0), new Position(2, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                    new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                }
            };
            belts[4] = new Belt(new Position(0, 0), new Position(2, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                }
            };
            belts[5] = new Belt(new Position(0, 0), new Position(2, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                }
            };

            // Act
            var items = new Item[numberOfCases];
            var results = new bool[numberOfCases];
            for (int i = 0; i < numberOfCases; i++) {
                results[i] = belts[i].TryRemoveLastItemFromBelt(out items[i]);
            }


            // Assert
            var corrects = new List<Belt.BeltSegment>[numberOfCases];
            var correctItems = new Item[numberOfCases];

            corrects[0] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 3, item = Item.GetEmpty()},
            };
            correctItems[0] = new Item().MakeDummyItem(1);

            corrects[1] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
            };
            correctItems[1] = new Item().MakeDummyItem(1);

            corrects[2] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 3, item = Item.GetEmpty()},
            };
            correctItems[2] = Item.GetEmpty();

            corrects[3] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                new Belt.BeltSegment() {count = 2, item = Item.GetEmpty()},
            };
            correctItems[3] = new Item().MakeDummyItem(1);

            corrects[4] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
            };
            correctItems[4] = new Item().MakeDummyItem(1);

            corrects[5] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
            };
            correctItems[5] = Item.GetEmpty();

            Assert.AreEqual(items, correctItems);
            for (int i = 0; i < numberOfCases; i++) {
                Assert.AreEqual(results[i], !items[i].isEmpty());
                Assert.IsTrue(BeltSegmentEqualityChecker(belts[i].items, corrects[i]));
            }
        }

        [Test]
        public void TestBeltUpdateLastSpaceRemoval() {
            // Arrange
            int numberOfCases = 4;
            var belts = new Belt[numberOfCases];

            belts[0] = new Belt(new Position(0, 0), new Position(2, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 2, item = new Item().MakeDummyItem(1)},
                    new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                }
            };
            belts[1] = new Belt(new Position(0, 0), new Position(2, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                    new Belt.BeltSegment() {count = 2, item = Item.GetEmpty()},
                }
            };
            belts[2] = new Belt(new Position(0, 0), new Position(2, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 3, item = Item.GetEmpty()},
                }
            };
            belts[3] = new Belt(new Position(0, 0), new Position(2, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                    new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                }
            };

            // Act
            for (int i = 0; i < numberOfCases; i++) {
                FactorySimulator.UpdateBelt(belts[i]);
            }


            // Assert
            var corrects = new List<Belt.BeltSegment>[numberOfCases];

            corrects[0] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                new Belt.BeltSegment() {count = 2, item = new Item().MakeDummyItem(1)},
            };
            corrects[1] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
            };
            corrects[2] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 3, item = Item.GetEmpty()},
            };
            corrects[3] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 2, item = Item.GetEmpty()},
                new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
            };


            for (int i = 0; i < numberOfCases; i++) {
                Assert.IsTrue(BeltSegmentEqualityChecker(belts[i].items, corrects[i]));
            }
        }

        [Test]
        public void TestBeltUpdateMidSpaceRemoval() {
            // Arrange
            int numberOfCases = 4;
            var belts = new Belt[numberOfCases];

            belts[0] = new Belt(new Position(0, 0), new Position(2, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(2)},
                    new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                }
            };
            belts[1] = new Belt(new Position(0, 0), new Position(5, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                    new Belt.BeltSegment() {count = 2, item = new Item().MakeDummyItem(3)},
                    new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(2)},
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                }
            };
            belts[2] = new Belt(new Position(0, 0), new Position(4, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                    new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(2)},
                    new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                }
            };
            belts[3] = new Belt(new Position(0, 0), new Position(4, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                    new Belt.BeltSegment() {count = 2, item = Item.GetEmpty()},
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                }
            };

            // Act
            for (int i = 0; i < numberOfCases; i++) {
                FactorySimulator.UpdateBelt(belts[i]);
            }


            // Assert
            var corrects = new List<Belt.BeltSegment>[numberOfCases];

            corrects[0] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(2)},
                new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
            };
            corrects[1] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 2, item = Item.GetEmpty()},
                new Belt.BeltSegment() {count = 2, item = new Item().MakeDummyItem(3)},
                new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(2)},
                new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
            };
            corrects[2] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(2)},
                new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
            };
            corrects[3] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
            };


            for (int i = 0; i < numberOfCases; i++) {
                Assert.IsTrue(BeltSegmentEqualityChecker(belts[i].items, corrects[i]));
            }
        }

        [Test]
        public void TestBeltUpdateMidSpaceRemovalAndMerge() {
            // Arrange
            int numberOfCases = 3;
            var belts = new Belt[numberOfCases];

            belts[0] = new Belt(new Position(0, 0), new Position(2, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                    new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                }
            };
            belts[1] = new Belt(new Position(0, 0), new Position(5, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                    new Belt.BeltSegment() {count = 2, item = new Item().MakeDummyItem(2)},
                    new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(2)},
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                }
            };
            belts[2] = new Belt(new Position(0, 0), new Position(4, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                    new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                    new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                    new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                }
            };

            // Act
            for (int i = 0; i < numberOfCases; i++) {
                FactorySimulator.UpdateBelt(belts[i]);
            }


            // Assert
            var corrects = new List<Belt.BeltSegment>[numberOfCases];

            corrects[0] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                new Belt.BeltSegment() {count = 2, item = new Item().MakeDummyItem(1)},
            };
            corrects[1] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 2, item = Item.GetEmpty()},
                new Belt.BeltSegment() {count = 3, item = new Item().MakeDummyItem(2)},
                new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
            };
            corrects[2] = new List<Belt.BeltSegment>() {
                new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                new Belt.BeltSegment() {count = 1, item = new Item().MakeDummyItem(1)},
                new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()},
                new Belt.BeltSegment() {count = 2, item = new Item().MakeDummyItem(1)},
            };


            for (int i = 0; i < numberOfCases; i++) {
                Assert.IsTrue(BeltSegmentEqualityChecker(belts[i].items, corrects[i]));
            }
        }

        int CountBeltSlots(Belt belt) {
            int count = 0;
            foreach (var segment in belt.items) {
                count += segment.count;
            }

            return count;
        }

        void AssertBeltLength(Belt belt) {
            var distance = Position.Distance(belt.startPos, belt.endPos);
            Assert.AreEqual(distance + 1, belt.length);

        }

        void AssertBeltSlotCounts(Belt belt) {
            AssertBeltLength(belt);
            Assert.AreEqual(belt.length * 4, CountBeltSlots(belt));
        }

        [Test]
        public void TestInsertingItemsToBelt() {
            var belt = new Belt(new Position(0, 0), new Position(3, 0)) {
                items = new List<Belt.BeltSegment>() {
                    new Belt.BeltSegment() {count = 16, item = Item.GetEmpty()},
                }
            };

            // insert items to the belt
            var dummy1 = new Item().MakeDummyItem(1);
            var dummy2 = new Item().MakeDummyItem(2);
            for (int i = 0; i < 16; i++) {
                // add items in chunks
                if (i < 3 || (i > 7 && i < 10) || i > 15) {
                    belt.TryInsertItemToBelt(dummy1);
                } else if (i < 5) {
                    belt.TryInsertItemToBelt(dummy2);
                }

                AssertBeltSlotCounts(belt);
            }
        }
    }
}