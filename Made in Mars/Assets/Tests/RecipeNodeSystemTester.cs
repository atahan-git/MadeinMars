using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class RecipeNodeSystemTester
    {
        public RecipeSet CreateRecipeSet() {
            return ScriptableObject.CreateInstance<RecipeSet>();
        }

        [Test]
        public void CheckRecipeSetCreation() {
            var recipeSet = CreateRecipeSet();
            Assert.IsNotNull(recipeSet);
        }
    }
}
