using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexFactoryGame.Recipes
{
    /// <summary>
    /// Manager for loading and validating recipes from JSON data
    /// </summary>
    public class RecipeManager : MonoBehaviour
    {
        [SerializeField] private TextAsset recipeJsonFile;
        private Dictionary<string, Recipe> recipes = new Dictionary<string, Recipe>();
        
        private void Awake()
        {
            LoadRecipes();
        }
        
        /// <summary>
        /// Load recipes from JSON file and validate them
        /// </summary>
        private void LoadRecipes()
        {
            if (recipeJsonFile == null)
            {
                Debug.LogError("No recipe JSON file assigned to RecipeManager");
                return;
            }
            
            try
            {
                Recipe[] recipeArray = JsonUtility.FromJson<RecipeArray>("{\"recipes\":" + recipeJsonFile.text + "}").recipes;
                
                foreach (Recipe recipe in recipeArray)
                {
                    if (RecipeValidator.IsValid(recipe))
                    {
                        recipes[recipe.id] = recipe;
                        Debug.Log($"Loaded recipe: {recipe.id} (Tier {recipe.tier})");
                    }
                    else
                    {
                        Debug.LogError($"Invalid recipe: {recipe.id}");
                    }
                }
                
                Debug.Log($"Loaded {recipes.Count} valid recipes");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load recipes: {e.Message}");
            }
        }
        
        /// <summary>
        /// Get a recipe by ID
        /// </summary>
        public Recipe GetRecipe(string id)
        {
            recipes.TryGetValue(id, out Recipe recipe);
            return recipe;
        }
        
        /// <summary>
        /// Get all recipes for a specific factory type
        /// </summary>
        public List<Recipe> GetRecipesForFactory(string factoryType)
        {
            return recipes.Values.Where(r => r.factory == factoryType).ToList();
        }
        
        /// <summary>
        /// Get all recipes for a specific tier
        /// </summary>
        public List<Recipe> GetRecipesForTier(int tier)
        {
            return recipes.Values.Where(r => r.tier == tier).ToList();
        }
        
        /// <summary>
        /// Get all loaded recipes
        /// </summary>
        public Dictionary<string, Recipe> GetAllRecipes()
        {
            return new Dictionary<string, Recipe>(recipes);
        }
        
        /// <summary>
        /// Helper class for JSON deserialization
        /// </summary>
        [System.Serializable]
        private class RecipeArray
        {
            public Recipe[] recipes;
        }
    }
}