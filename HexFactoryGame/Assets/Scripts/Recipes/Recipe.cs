using System.Collections.Generic;
using UnityEngine;

namespace HexFactoryGame.Recipes
{
    /// <summary>
    /// Recipe structure following JSON format specification
    /// Rules: max 2 inputs, single output item type, explicit timing
    /// </summary>
    [System.Serializable]
    public class Recipe
    {
        public string id;
        public List<ItemCount> inputs = new List<ItemCount>();
        public ItemCount output;
        public float time;
        public string factory;
        public int tier;
        
        [System.Serializable]
        public class ItemCount
        {
            public string item;
            public int count;
            
            public ItemCount(string item, int count)
            {
                this.item = item;
                this.count = count;
            }
        }
    }
    
    /// <summary>
    /// Recipe validation following architectural constraints
    /// </summary>
    public static class RecipeValidator
    {
        public static bool IsValid(Recipe recipe)
        {
            if (recipe == null) return false;
            
            // Max 2 inputs constraint
            if (recipe.inputs.Count > 2) return false;
            
            // Must have valid output
            if (recipe.output == null || string.IsNullOrEmpty(recipe.output.item) || recipe.output.count <= 0)
                return false;
                
            // Must have positive time
            if (recipe.time <= 0) return false;
            
            // Must have valid factory type
            if (string.IsNullOrEmpty(recipe.factory)) return false;
            
            // Validate all inputs
            foreach (ItemCount input in recipe.inputs)
            {
                if (input == null || string.IsNullOrEmpty(input.item) || input.count <= 0)
                    return false;
            }
            
            // Valid tier range (0-5 as per specifications)
            if (recipe.tier < 0 || recipe.tier > 5) return false;
            
            return true;
        }
    }
}