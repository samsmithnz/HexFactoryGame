# HexFactoryGame - Hex Solution Implementation

This implementation provides a complete hex-based factory automation system following the architectural specifications in [copilot-instructions.md](../../../copilot-instructions.md).

## Architecture Overview

### Core Hex System
- **HexCoord**: Axial coordinates (q,r) for storage and UI operations
- **CubeCoord**: Cube coordinates (x,y,z) for mathematical calculations  
- **Conversion**: Seamless conversion between coordinate systems
- **Spatial Operations**: Distance calculation, neighbor detection, pathfinding support

### Factory System
- **IFactory Interface**: Enforces strict architectural constraints
- **BaseFactory**: Common functionality for all factory types
- **Factory Types**:
  - **Mine**: 0 inputs, extracts raw resources (Tier 0)
  - **Smelter**: 1 input, processes raw materials (Tier 1)
  - **BasicAssembler**: 1-2 inputs, creates manufactured goods (Tier 2)

### Recipe System
- **Recipe Class**: JSON-serializable recipe structure
- **RecipeValidator**: Enforces max 2 inputs, 1 output rules
- **RecipeManager**: Loads and manages recipes from JSON files
- **Sample Data**: 10 example recipes showing tier progression

### Unity Integration
- **HexGrid**: Manages 3D factory placement on hex world
- **FactoryBuilding**: MonoBehaviour component for in-game factories
- **GameManager**: Demonstrates complete system integration

## Architectural Compliance

✅ **Single-output rule**: Every factory produces exactly ONE item type  
✅ **Max 2 inputs**: Factories have 0 (mines), 1, or 2 input slots - never more  
✅ **Explicit transformations**: No hidden byproducts or surprise outputs  
✅ **One hex per building**: Each building occupies exactly one hex tile  
✅ **Hex-aware spatial logic**: All positioning uses proper hex geometry  
✅ **Data-driven recipes**: JSON-based recipe system with validation  
✅ **Tier-based progression**: Items organized into tiers 0-5  

## Usage Examples

### Basic Coordinate Operations
```csharp
// Create hex coordinates
HexCoord position = new HexCoord(1, -1);
CubeCoord cube = position.ToCube();

// Calculate distance
HexCoord origin = new HexCoord(0, 0);
int distance = origin.DistanceTo(position); // Returns 1

// Get neighbors
HexCoord[] neighbors = position.GetNeighbors(); // Returns 6 neighbors
```

### Factory System
```csharp
// Create factories
Mine mine = new Mine();
Smelter smelter = new Smelter();
BasicAssembler assembler = new BasicAssembler();

// Validate constraints
Assert.AreEqual(0, mine.MaxInputs);
Assert.AreEqual(1, smelter.MaxInputs);  
Assert.AreEqual(2, assembler.MaxInputs);
Assert.AreEqual(1, mine.MaxOutputs); // All factories have 1 output
```

### Recipe Validation
```csharp
Recipe recipe = new Recipe
{
    id = "iron_smelting",
    inputs = new List<Recipe.ItemCount> { new Recipe.ItemCount("iron_ore", 1) },
    output = new Recipe.ItemCount("iron_ingot", 1),
    time = 4.0f,
    factory = "smelter",
    tier = 1
};

bool isValid = RecipeValidator.IsValid(recipe); // Returns true
```

### Hex Grid Placement
```csharp
HexGrid grid = FindObjectOfType<HexGrid>();
HexCoord position = new HexCoord(0, 0);
Mine mine = new Mine();

// Check if position is available
if (grid.IsPositionAvailable(position))
{
    grid.PlaceFactory(position, mine, minePrefab);
}

// Convert to world position for 3D rendering
Vector3 worldPos = grid.HexToWorldPosition(position);
```

## File Structure

```
Assets/Scripts/
├── Core/
│   ├── HexCoord.cs           # Axial coordinate system
│   ├── CubeCoord.cs          # Cube coordinate system  
│   └── HexGrid.cs            # 3D hex grid management
├── Factories/
│   ├── IFactory.cs           # Factory interface
│   └── BasicFactories.cs     # Mine, Smelter, BasicAssembler
├── Recipes/
│   ├── Recipe.cs             # Recipe data structure & validation
│   └── RecipeManager.cs      # Recipe loading & management
├── Components/
│   └── FactoryBuilding.cs    # Unity MonoBehaviour integration
├── Managers/
│   └── GameManager.cs        # System integration & demo
└── Tests/
    ├── HexCoordTests.cs      # Coordinate system tests
    ├── FactoryTests.cs       # Factory validation tests
    └── RecipeTests.cs        # Recipe validation tests
```

## Testing

The implementation includes comprehensive unit tests:

- **HexCoordTests**: Coordinate conversion, distance calculation, neighbor detection
- **FactoryTests**: Input/output limits, recipe validation, factory type constraints  
- **RecipeTests**: JSON validation, constraint enforcement, tier validation

Run tests in Unity Test Runner or validate manually using GameManager's context menu.

## Demo Scene Setup

1. Create empty GameObject, add **HexGrid** component
2. Create empty GameObject, add **RecipeManager** component  
3. Assign `base_recipes.json` to RecipeManager's Recipe Json File field
4. Create empty GameObject, add **GameManager** component
5. Enable "Create Demo Layout" on GameManager
6. Play scene to see automatic hex factory layout

The demo creates a production chain with mines, smelters, and assemblers positioned on a hex grid with proper spacing and neighbor relationships.

## Performance Considerations

- Uses integer coordinates to avoid floating-point precision issues
- Efficient neighbor calculation using pre-computed offsets
- Dictionary-based factory lookup for O(1) position queries
- Recipe validation occurs at load time, not runtime
- Spatial indexing ready for large-scale factory networks

## Extension Points

- **Belt System**: Add edge-based conveyor belts following hex geometry
- **Advanced Factories**: Electronics bench, integration plant, etc.
- **Resource Nodes**: Specific resource types at world positions
- **Adjacency Bonuses**: Performance boosts for neighboring factories
- **Pathfinding**: A* algorithm using hex distance heuristics
- **Save/Load**: Serialize hex positions and factory states
- **Multiplayer**: Network sync using hex coordinates as authoritative positions

This implementation provides a solid foundation for a hex-based factory automation game while strictly adhering to the architectural principles defined in the project documentation.