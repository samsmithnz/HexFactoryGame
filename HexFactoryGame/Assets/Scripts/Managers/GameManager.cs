using UnityEngine;
using HexFactoryGame.Core;
using HexFactoryGame.Factories;
using HexFactoryGame.Recipes;
using HexFactoryGame.Components;

namespace HexFactoryGame.Managers
{
    /// <summary>
    /// Main game manager demonstrating the hex factory system
    /// Shows integration between coordinates, factories, and recipes
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Game Configuration")]
        [SerializeField] private HexGrid hexGrid;
        [SerializeField] private RecipeManager recipeManager;
        [SerializeField] private GameObject[] factoryPrefabs;
        
        [Header("Demo Setup")]
        [SerializeField] private bool createDemoLayout = true;
        [SerializeField] private int demoLayoutSize = 3;
        
        private void Start()
        {
            if (hexGrid == null)
                hexGrid = FindObjectOfType<HexGrid>();
                
            if (recipeManager == null)
                recipeManager = FindObjectOfType<RecipeManager>();
                
            if (createDemoLayout)
                CreateDemoLayout();
                
            // Validate the implementation
            ValidateSystem();
        }
        
        /// <summary>
        /// Create a demonstration layout showing the hex system in action
        /// </summary>
        private void CreateDemoLayout()
        {
            if (hexGrid == null) 
            {
                Debug.LogWarning("No HexGrid found for demo layout");
                return;
            }
            
            Debug.Log("Creating demo hex factory layout...");
            
            // Create a simple production chain: Mine -> Smelter -> Assembler
            
            // Place mines at the corners
            PlaceDemoFactory(new HexCoord(-2, 0), new Mine(), "Mine");
            PlaceDemoFactory(new HexCoord(2, -2), new Mine(), "Mine");
            PlaceDemoFactory(new HexCoord(0, 2), new Mine(), "Mine");
            
            // Place smelters in a ring
            PlaceDemoFactory(new HexCoord(-1, 0), new Smelter(), "Smelter");
            PlaceDemoFactory(new HexCoord(1, -1), new Smelter(), "Smelter");
            PlaceDemoFactory(new HexCoord(0, 1), new Smelter(), "Smelter");
            
            // Place assemblers in the center
            PlaceDemoFactory(new HexCoord(0, 0), new BasicAssembler(), "BasicAssembler");
            PlaceDemoFactory(new HexCoord(1, 0), new BasicAssembler(), "BasicAssembler");
            
            Debug.Log($"Demo layout created with {GetFactoryCount()} factories");
        }
        
        private void PlaceDemoFactory(HexCoord position, IFactory factory, string prefabName)
        {
            // Create a simple cube as factory representation
            GameObject factoryObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            factoryObject.name = $"{factory.FactoryType}_at_{position}";
            
            // Add the FactoryBuilding component
            FactoryBuilding building = factoryObject.AddComponent<FactoryBuilding>();
            building.HexPosition = position;
            
            // Set different colors for different factory types
            Renderer renderer = factoryObject.GetComponent<Renderer>();
            Material material = new Material(Shader.Find("Standard"));
            
            material.color = factory.FactoryType switch
            {
                "mine" => Color.brown,
                "smelter" => Color.orange,
                "basic_assembler" => Color.blue,
                _ => Color.gray
            };
            
            renderer.material = material;
            
            // Place using hex grid
            if (hexGrid.PlaceFactory(position, factory, factoryObject))
            {
                Debug.Log($"Placed {factory.FactoryType} (Tier {factory.Tier}) at {position}");
            }
        }
        
        private int GetFactoryCount()
        {
            return FindObjectsOfType<FactoryBuilding>().Length;
        }
        
        /// <summary>
        /// Validate that the hex system is working correctly
        /// </summary>
        [ContextMenu("Validate System")]
        public void ValidateSystem()
        {
            Debug.Log("=== HexFactoryGame System Validation ===");
            
            // Test coordinate system
            ValidateCoordinateSystem();
            
            // Test factory system
            ValidateFactorySystem();
            
            // Test recipe system
            ValidateRecipeSystem();
            
            Debug.Log("=== Validation Complete ===");
        }
        
        private void ValidateCoordinateSystem()
        {
            Debug.Log("Testing hex coordinate system...");
            
            HexCoord origin = new HexCoord(0, 0);
            HexCoord test = new HexCoord(1, -1);
            
            // Test conversion
            CubeCoord cube = test.ToCube();
            HexCoord backToAxial = cube.ToAxial();
            
            if (test == backToAxial)
                Debug.Log("✓ Coordinate conversion working correctly");
            else
                Debug.LogError("✗ Coordinate conversion failed");
                
            // Test distance
            int distance = origin.DistanceTo(test);
            if (distance == 1)
                Debug.Log("✓ Distance calculation working correctly");
            else
                Debug.LogError($"✗ Distance calculation failed: expected 1, got {distance}");
                
            // Test neighbors
            HexCoord[] neighbors = origin.GetNeighbors();
            if (neighbors.Length == 6)
                Debug.Log("✓ Neighbor calculation working correctly");
            else
                Debug.LogError($"✗ Neighbor calculation failed: expected 6, got {neighbors.Length}");
        }
        
        private void ValidateFactorySystem()
        {
            Debug.Log("Testing factory system...");
            
            Mine mine = new Mine();
            Smelter smelter = new Smelter();
            BasicAssembler assembler = new BasicAssembler();
            
            // Test input/output constraints
            bool mineValid = mine.MaxInputs == 0 && mine.MaxOutputs == 1;
            bool smelterValid = smelter.MaxInputs == 1 && smelter.MaxOutputs == 1;
            bool assemblerValid = assembler.MaxInputs == 2 && assembler.MaxOutputs == 1;
            
            if (mineValid && smelterValid && assemblerValid)
                Debug.Log("✓ Factory constraints working correctly");
            else
                Debug.LogError("✗ Factory constraints violated");
        }
        
        private void ValidateRecipeSystem()
        {
            Debug.Log("Testing recipe system...");
            
            if (recipeManager == null)
            {
                Debug.LogWarning("No RecipeManager found for validation");
                return;
            }
            
            var recipes = recipeManager.GetAllRecipes();
            if (recipes.Count > 0)
                Debug.Log($"✓ Recipe system loaded {recipes.Count} recipes");
            else
                Debug.LogWarning("No recipes loaded");
                
            // Test recipe validation
            Recipe testRecipe = new Recipe
            {
                id = "test",
                inputs = new System.Collections.Generic.List<Recipe.ItemCount> 
                { 
                    new Recipe.ItemCount("iron_ore", 1) 
                },
                output = new Recipe.ItemCount("iron_ingot", 1),
                time = 4.0f,
                factory = "smelter",
                tier = 1
            };
            
            if (RecipeValidator.IsValid(testRecipe))
                Debug.Log("✓ Recipe validation working correctly");
            else
                Debug.LogError("✗ Recipe validation failed");
        }
        
        /// <summary>
        /// Demonstrate hex pathfinding between two points
        /// </summary>
        [ContextMenu("Test Pathfinding")]
        public void TestPathfinding()
        {
            HexCoord start = new HexCoord(0, 0);
            HexCoord end = new HexCoord(3, -2);
            
            Debug.Log($"Testing pathfinding from {start} to {end}");
            Debug.Log($"Direct distance: {start.DistanceTo(end)} hexes");
            
            // Simple demonstration - in a real game you'd implement A* or similar
            Debug.Log("Note: Full pathfinding implementation would use A* algorithm with hex-aware heuristics");
        }
    }
}