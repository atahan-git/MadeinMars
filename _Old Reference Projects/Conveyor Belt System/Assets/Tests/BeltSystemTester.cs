using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
	public class BeltSystemTester {

		[SetUp]
		public void ResetScene () {

			EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);

		}


		[Test]
		public void BeltPositionSetCheck () {
			// Arrange
			GameObject myBelt = new GameObject();
			myBelt.AddComponent<BeltObject>();
			myBelt.transform.position = new Vector3(5, 2, 0);

			// Act
			myBelt.GetComponent<BeltObject>().SetPosBasedOnWorlPos();

			// Assert
			Assert.AreEqual(myBelt.GetComponent<BeltObject>().pos.x, 5);
			Assert.AreEqual(myBelt.GetComponent<BeltObject>().pos.y, 2);
		}


		[Test]
		public void CreateBeltItemSlotsCheck () {
			// Arrange
			GameObject myBeltwithTwoinTwoOuts_12totalSlots = new GameObject();
			myBeltwithTwoinTwoOuts_12totalSlots.AddComponent<BeltObject>();
			myBeltwithTwoinTwoOuts_12totalSlots.GetComponent<BeltObject>().beltInputs = new bool[] { true, true, false, false };
			myBeltwithTwoinTwoOuts_12totalSlots.GetComponent<BeltObject>().beltOutputs = new bool[] { false, false, true, true };

			GameObject myBeltwithOneinOneOut_8totalSlots = new GameObject();
			myBeltwithOneinOneOut_8totalSlots.AddComponent<BeltObject>();
			myBeltwithOneinOneOut_8totalSlots.GetComponent<BeltObject>().beltInputs = new bool[] { true, false, false, false };
			myBeltwithOneinOneOut_8totalSlots.GetComponent<BeltObject>().beltOutputs = new bool[] { false, false, true, false };

			// Act
			List<BeltItemSlot> slotList12 = myBeltwithTwoinTwoOuts_12totalSlots.GetComponent<BeltObject>().CreateBeltItemSlots();
			List<BeltItemSlot> slotList8 = myBeltwithOneinOneOut_8totalSlots.GetComponent<BeltObject>().CreateBeltItemSlots();

			// Assert
			Assert.AreEqual(slotList12.Count, 12);
			Assert.AreEqual(slotList8.Count, 8);
		}


		[Test]
		public void BeltPrePassCheck () {
			// Arrange
			List<BeltObject> allBelts = new List<BeltObject>();
			List<List<BeltObject>> beltGroups = new List<List<BeltObject>>();

			List<BeltItemSlot> allBeltItemsSlots = new List<BeltItemSlot>();
			List<List<BeltItemSlot>> beltItemSlotGroups = new List<List<BeltItemSlot>>();

			List<BeltItem> allBeltItems = new List<BeltItem>();


			// Act
			BeltPreProcessor beltProc = new BeltPreProcessor(allBelts, beltGroups, allBeltItemsSlots, beltItemSlotGroups, allBeltItems, (int x, int y) => null);

			beltProc.PrepassBelts();

			// Assert
		}

		[Test]
		public void BeltItemSlotProcessorCheck () {
			// Arrange
			List<List<BeltItemSlot>> dummyBeltItemSlotGroup = new List<List<BeltItemSlot>>();
			dummyBeltItemSlotGroup.Add(new List<BeltItemSlot>());
			dummyBeltItemSlotGroup[0].Add(new BeltItemSlot(Vector3.zero));
			dummyBeltItemSlotGroup[0].Add(new BeltItemSlot(Vector3.zero));
			BeltItemSlot.ConnectBelts(dummyBeltItemSlotGroup[0][0], dummyBeltItemSlotGroup[0][1]);

			List<BeltItem> beltItems = new List<BeltItem>();
			beltItems.Add(new GameObject().AddComponent<BeltItem>().GetComponent<BeltItem>());
			dummyBeltItemSlotGroup[0][0].myItem = beltItems[0];
			beltItems[0].mySlot = dummyBeltItemSlotGroup[0][0];

			BeltItemSlotUpdateProcessor myBeltItemSlotUpdateProcessor = new BeltItemSlotUpdateProcessor(beltItems, dummyBeltItemSlotGroup);

			// Act
			myBeltItemSlotUpdateProcessor.UpdateBeltItemSlots();

			// Assert
			Assert.IsNull(dummyBeltItemSlotGroup[0][0].myItem);
			Assert.IsNotNull(dummyBeltItemSlotGroup[0][1].myItem);
		}


		[Test]
		public void BeltItemGFXProcesorCheck () {
			// Arrange
			List<BeltItem> beltItems = new List<BeltItem>();

			beltItems.Add(new GameObject().AddComponent<BeltItem>().GetComponent<BeltItem>());
			beltItems[0].transform.position = Vector3.zero;

			beltItems[0].mySlot = new BeltItemSlot(Vector3.zero + Vector3.forward * BeltObject.beltItemSlotDistance);

			BeltItemGfxUpdateProcessor myBeltItemGfxProcessor = new BeltItemGfxUpdateProcessor(beltItems);

			// Act
			myBeltItemGfxProcessor.UpdateBeltItemGfxs(2f, 0.25f);

			// Assert
			Assert.AreEqual(Vector3.Distance(beltItems[0].transform.position, (Vector3.forward * BeltObject.beltItemSlotDistance) / 2f), 0f, 0.05f);

			// Act
			myBeltItemGfxProcessor.UpdateBeltItemGfxs(2f, 0.25f);

			// Assert
			Assert.AreEqual(Vector3.Distance(beltItems[0].transform.position, (Vector3.forward * BeltObject.beltItemSlotDistance)), 0f, 0.05f);
		}

		[Test]
		public void DrawLinesCheck () {

			Assert.AreEqual(DrawNumber(5), 5);
			Assert.AreEqual(DrawNumber(-5), 5);
		}

		[Test]
		public void SortCheck () {
			List<BeltItemSlot> beltItemSlotGroup = new List<BeltItemSlot>();

			beltItemSlotGroup.Add(new BeltItemSlot(Vector3.zero));
			beltItemSlotGroup.Add(new BeltItemSlot(Vector3.zero));
			beltItemSlotGroup.Add(new BeltItemSlot(Vector3.zero));
			beltItemSlotGroup.Add(new BeltItemSlot(Vector3.zero));
			beltItemSlotGroup.Add(new BeltItemSlot(Vector3.zero));


			beltItemSlotGroup[0].index = 5;
			beltItemSlotGroup[1].index = -2;
			beltItemSlotGroup[2].index = 3;
			beltItemSlotGroup[3].index = 3;
			beltItemSlotGroup[4].index = 1;


			beltItemSlotGroup.Sort((x, y) => x.index.CompareTo(y.index));

			Assert.AreEqual(beltItemSlotGroup[0].index, -2);
			Assert.AreEqual(beltItemSlotGroup[1].index, 1);
			Assert.AreEqual(beltItemSlotGroup[2].index, 3);
			Assert.AreEqual(beltItemSlotGroup[3].index, 3);
			Assert.AreEqual(beltItemSlotGroup[4].index, 5);
		}

		public int DrawNumber (int num) {
			int lineCount = 0;
			for (int i = 0; Mathf.Abs(i) < Mathf.Abs(num); i += num / Mathf.Abs(num)) {
				lineCount++;
			}
			return lineCount;
		}
	}
}
