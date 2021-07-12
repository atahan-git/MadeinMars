using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;


namespace Tests
{
    public class IntegrationTester
    {
        [SetUp]
        public void SetUp() {
            Time.timeScale = 100;
        }
		
        [TearDown]
        public void TearDown() {
            Time.timeScale = 1;
        }
		
        [UnityTest]
        public IEnumerator TestStarterFactoryFunction() {

            SceneManager.LoadScene("Play Level");

            yield return new WaitUntil(() => SceneManager.GetActiveScene().isLoaded);
			
            Assert.IsTrue(SceneManager.GetActiveScene().isLoaded);
            

            var storageSimObject = Grid.s.GetTile(new Position(122, 110)).simObject;
            Assert.IsNotNull(storageSimObject);
            var storage = storageSimObject as Building;

            foreach (var slot in storage.inv.inventoryItemSlots) {
                slot.count = 0;
            }
            
            Assert.AreEqual(0, storage.inv.GetTotalAmountOfItems());

            yield return new WaitForSeconds(100f);
			
            Assert.IsNotNull(storage);

            var itemAmount = storage.inv.GetTotalAmountOfItems();
            Assert.IsTrue(itemAmount > 0);
        }
    }
}
