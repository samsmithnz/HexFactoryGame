using NUnit.Framework;
using System.Collections.Generic;
using HexFactoryGame.Factories;
using HexFactoryGame.Recipes;

namespace HexFactoryGame.Tests
{
    /// <summary>
    /// Tests for factory implementation following architectural constraints
    /// </summary>
    public class FactoryTests
    {
        [Test]
        public void Factory_ShouldRespectInputLimits()
        {
            BasicAssembler factory = new BasicAssembler();
            Assert.LessOrEqual(factory.MaxInputs, 2);
            Assert.AreEqual(factory.MaxOutputs, 1);
        }
        
        [Test]
        public void Mine_ShouldHaveZeroInputs()
        {
            Mine mine = new Mine();
            Assert.AreEqual(0, mine.MaxInputs);
            Assert.AreEqual(1, mine.MaxOutputs);
        }
        
        [Test]
        public void Smelter_ShouldHaveOneInput()
        {
            Smelter smelter = new Smelter();
            Assert.AreEqual(1, smelter.MaxInputs);
            Assert.AreEqual(1, smelter.MaxOutputs);
        }
        
        [Test]
        public void BasicAssembler_ShouldHaveTwoMaxInputs()
        {
            BasicAssembler assembler = new BasicAssembler();
            Assert.AreEqual(2, assembler.MaxInputs);
            Assert.AreEqual(1, assembler.MaxOutputs);
        }
        
        [Test]
        public void Factory_ShouldValidateRecipeFactoryType()
        {
            BasicAssembler assembler = new BasicAssembler();
            
            Recipe validRecipe = new Recipe
            {
                id = "gear",
                factory = "basic_assembler",
                inputs = new List<Recipe.ItemCount> { new Recipe.ItemCount("iron_plate", 1) },
                output = new Recipe.ItemCount("gear", 1),
                time = 3.0f,
                tier = 2
            };
            
            Recipe invalidRecipe = new Recipe
            {
                id = "gear",
                factory = "smelter", // Wrong factory type
                inputs = new List<Recipe.ItemCount> { new Recipe.ItemCount("iron_plate", 1) },
                output = new Recipe.ItemCount("gear", 1),
                time = 3.0f,
                tier = 2
            };
            
            Assert.IsTrue(assembler.CanCraft(validRecipe));
            Assert.IsFalse(assembler.CanCraft(invalidRecipe));
        }
        
        [Test]
        public void Mine_ShouldRejectRecipesWithInputs()
        {
            Mine mine = new Mine();
            
            Recipe invalidRecipe = new Recipe
            {
                id = "ore",
                factory = "mine",
                inputs = new List<Recipe.ItemCount> { new Recipe.ItemCount("something", 1) }, // Mines should have no inputs
                output = new Recipe.ItemCount("iron_ore", 1),
                time = 4.0f,
                tier = 0
            };
            
            Assert.IsFalse(mine.CanCraft(invalidRecipe));
        }
        
        [Test]
        public void Smelter_ShouldRequireExactlyOneInput()
        {
            Smelter smelter = new Smelter();
            
            Recipe validRecipe = new Recipe
            {
                id = "iron_ingot",
                factory = "smelter",
                inputs = new List<Recipe.ItemCount> { new Recipe.ItemCount("iron_ore", 1) },
                output = new Recipe.ItemCount("iron_ingot", 1),
                time = 4.0f,
                tier = 1
            };
            
            Recipe invalidRecipe = new Recipe
            {
                id = "iron_ingot",
                factory = "smelter",
                inputs = new List<Recipe.ItemCount> 
                { 
                    new Recipe.ItemCount("iron_ore", 1),
                    new Recipe.ItemCount("coal", 1) // Too many inputs for smelter
                },
                output = new Recipe.ItemCount("iron_ingot", 1),
                time = 4.0f,
                tier = 1
            };
            
            Assert.IsTrue(smelter.CanCraft(validRecipe));
            Assert.IsFalse(smelter.CanCraft(invalidRecipe));
        }
    }
}