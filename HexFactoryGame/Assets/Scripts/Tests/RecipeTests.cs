using NUnit.Framework;
using System.Collections.Generic;
using HexFactoryGame.Recipes;

namespace HexFactoryGame.Tests
{
    /// <summary>
    /// Tests for recipe validation system
    /// </summary>
    public class RecipeTests
    {
        [Test]
        public void Recipe_ShouldValidateConstraints()
        {
            Recipe validRecipe = new Recipe
            {
                id = "gear",
                inputs = new List<Recipe.ItemCount> 
                { 
                    new Recipe.ItemCount("iron_plate", 1),
                    new Recipe.ItemCount("iron_rod", 1)
                },
                output = new Recipe.ItemCount("gear", 1),
                time = 4.0f,
                factory = "basic_assembler",
                tier = 2
            };
            
            Assert.IsTrue(RecipeValidator.IsValid(validRecipe));
        }
        
        [Test]
        public void Recipe_ShouldRejectTooManyInputs()
        {
            Recipe invalidRecipe = new Recipe
            {
                id = "invalid",
                inputs = new List<Recipe.ItemCount> 
                { 
                    new Recipe.ItemCount("item1", 1),
                    new Recipe.ItemCount("item2", 1),
                    new Recipe.ItemCount("item3", 1) // Too many inputs
                },
                output = new Recipe.ItemCount("result", 1),
                time = 4.0f,
                factory = "basic_assembler",
                tier = 2
            };
            
            Assert.IsFalse(RecipeValidator.IsValid(invalidRecipe));
        }
        
        [Test]
        public void Recipe_ShouldRejectInvalidOutput()
        {
            Recipe invalidRecipe = new Recipe
            {
                id = "invalid",
                inputs = new List<Recipe.ItemCount> { new Recipe.ItemCount("iron_ore", 1) },
                output = null, // Invalid output
                time = 4.0f,
                factory = "smelter",
                tier = 1
            };
            
            Assert.IsFalse(RecipeValidator.IsValid(invalidRecipe));
        }
        
        [Test]
        public void Recipe_ShouldRejectInvalidTime()
        {
            Recipe invalidRecipe = new Recipe
            {
                id = "invalid",
                inputs = new List<Recipe.ItemCount> { new Recipe.ItemCount("iron_ore", 1) },
                output = new Recipe.ItemCount("iron_ingot", 1),
                time = 0.0f, // Invalid time
                factory = "smelter",
                tier = 1
            };
            
            Assert.IsFalse(RecipeValidator.IsValid(invalidRecipe));
        }
        
        [Test]
        public void Recipe_ShouldRejectInvalidTier()
        {
            Recipe invalidRecipe = new Recipe
            {
                id = "invalid",
                inputs = new List<Recipe.ItemCount> { new Recipe.ItemCount("iron_ore", 1) },
                output = new Recipe.ItemCount("iron_ingot", 1),
                time = 4.0f,
                factory = "smelter",
                tier = 10 // Invalid tier (should be 0-5)
            };
            
            Assert.IsFalse(RecipeValidator.IsValid(invalidRecipe));
        }
        
        [Test]
        public void Recipe_ShouldRejectEmptyFactory()
        {
            Recipe invalidRecipe = new Recipe
            {
                id = "invalid",
                inputs = new List<Recipe.ItemCount> { new Recipe.ItemCount("iron_ore", 1) },
                output = new Recipe.ItemCount("iron_ingot", 1),
                time = 4.0f,
                factory = "", // Empty factory
                tier = 1
            };
            
            Assert.IsFalse(RecipeValidator.IsValid(invalidRecipe));
        }
        
        [Test]
        public void Recipe_ShouldAcceptZeroInputs()
        {
            Recipe validRecipe = new Recipe
            {
                id = "ore_extraction",
                inputs = new List<Recipe.ItemCount>(), // Empty inputs for mine
                output = new Recipe.ItemCount("iron_ore", 1),
                time = 4.0f,
                factory = "mine",
                tier = 0
            };
            
            Assert.IsTrue(RecipeValidator.IsValid(validRecipe));
        }
    }
}