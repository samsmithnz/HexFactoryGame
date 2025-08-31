# Copilot Instructions for HexFactoryGame

## Project Overview

HexFactoryGame is a relaxed factory automation game set on an isometric 3D hex world. This document provides guidance for GitHub Copilot and contributors when working on this project.

**Core Vision**: Build a clear, extensible production loop from basic resources to complex products while respecting strict architectural constraints that create emergent complexity through spatial planning.

## Fundamental Architectural Principles

### 🏭 Factory Design Rules (STRICT - Never Violate)
- **Single-output rule**: Every factory produces exactly ONE item type (no multiple variants)
- **Max 2 inputs**: Factories can have 0 (mines), 1, or 2 input slots - never more
- **Explicit transformations**: No hidden byproducts or surprise outputs
- **One hex per building**: Each building occupies exactly one hex tile

### 🔢 Coordinate System
- Use **axial coordinates (q,r)** or **cube coordinates (x,y,z)** for hex positioning
- Prefer axial for storage, cube for calculations
- All spatial logic should account for hex geometry, not square grids

### 📊 Data Structure Conventions

#### Recipe Format (JSON)
```json
{
  "id": "gear",
  "inputs": [
    {"item": "iron_plate", "count": 1},
    {"item": "iron_rod", "count": 1}
  ],
  "output": {"item": "gear", "count": 1},
  "time": 4.0,
  "factory": "basic_assembler",
  "tier": 2
}
```

**Rules for recipes:**
- Always validate max 2 inputs
- Single output item type only
- Include explicit timing
- Reference valid factory types
- Assign appropriate tier (0-5)

#### Coordinate Representations
```csharp
// Preferred axial coordinate structure
public struct HexCoord 
{
    public int q; // column
    public int r; // row
}

// For calculations, convert to cube when needed
public struct CubeCoord 
{
    public int x, y, z; // where x + y + z = 0
}
```

## Code Organization Patterns

### Naming Conventions
- **Factory Types**: Use descriptive names that match game terminology
  - `IronMine`, `BasicAssembler`, `AdvancedAssembler`, `ElectronicsBench`
- **Items**: Use clear, consistent naming
  - `iron_ore`, `iron_ingot`, `iron_plate`, `copper_wire`, `simple_circuit`
- **Coordinates**: Use `coord`, `position`, or `hexPos` for hex positions
- **Logistics**: Use `belt`, `connection`, `transport` for movement systems

### Type Declaration Guidelines
- **Explicit Types**: Always use explicit type declarations instead of `var` for better code clarity
- **Exception**: Only use `var` when the type is immediately obvious from the right-hand side (e.g., LINQ queries with complex anonymous types)
- **Readability**: Explicit types improve code readability and make intent clearer for contributors

### Class Structure Patterns
```csharp
// Base factory interface
public interface IFactory 
{
    string FactoryType { get; }
    int MaxInputs { get; } // 0, 1, or 2
    int MaxOutputs { get; } // Always 1
    bool CanCraft(Recipe recipe);
    float CraftTime { get; }
}

// Recipe validation
public class RecipeValidator 
{
    public static bool IsValid(Recipe recipe) 
    {
        return recipe.inputs.Count <= 2 && 
               recipe.output != null &&
               recipe.time > 0;
    }
}
```

## Tier System Guidelines

### Item Progression (Tiers 0-5)
- **Tier 0**: Raw resources (iron_ore, copper_ore, stone)
- **Tier 1**: Basic refined (iron_ingot, copper_ingot, stone_brick)
- **Tier 2**: Simple components (gear, wire, plate, frame)
- **Tier 3**: Composite parts (circuit, machine_casing, conveyor_module)
- **Tier 4**: Advanced systems (control_unit, logistics_core)
- **Tier 5**: Science/Victory items (science_pack_i, colony_module)

**Progression Rule**: Items should generally require lower-tier inputs, avoid skipping tiers

## Logistics & Transport Rules

### Belt System
- **Edge-based**: Belts run along hex edges, not through centers
- **Capacity tiers**: Mk1 (1.0 items/s), Mk2 (2.0 items/s), Mk3 (4.0 items/s)
- **Direction**: Each edge can host one belt direction (upgradeable to bidirectional)

### Throughput Balancing
- Target 60-80% belt utilization to create optimization pressure
- Avoid perfect ratios - slight imbalances encourage player engagement
- Example: If a mine produces 0.25 ore/s, smelter should consume slightly faster

## Visual & UI Development

### Hex Visualization
- **Building display**: Large output icon centered on hex
- **Input ports**: Small colored sockets on relevant edges
- **Efficiency overlay**: Color gradient (Red=starved, Yellow=partial, Green=full)
- **Flow indicators**: Arrows on belts, visible on hover

### UI Consistency
- Use hex-aware positioning for all building placement
- Maintain visual clarity over complex effects
- Show throughput information clearly
- Use consistent iconography across tiers

## Testing & Validation Patterns

### Factory Testing
```csharp
[Test]
public void Factory_ShouldRespectInputLimits() 
{
    BasicAssembler factory = new BasicAssembler();
    Assert.LessOrEqual(factory.MaxInputs, 2);
    Assert.AreEqual(factory.MaxOutputs, 1);
}

[Test]
public void Recipe_ShouldValidateConstraints() 
{
    Recipe invalidRecipe = new Recipe { 
        inputs = new List<ItemCount> { /* 3 inputs */ } 
    };
    Assert.False(RecipeValidator.IsValid(invalidRecipe));
}
```

### Coordinate Testing
```csharp
[Test]
public void HexCoord_ShouldConvertToCubeCorrectly() 
{
    HexCoord axial = new HexCoord(1, -1);
    CubeCoord cube = axial.ToCube();
    Assert.AreEqual(cube.x + cube.y + cube.z, 0);
}
```

## Performance Considerations

### Spatial Queries
- Use spatial indexing for hex grids (consider hex-specific data structures)
- Cache adjacency relationships for performance
- Optimize belt pathfinding for hex geometry

### Recipe Processing
- Pre-validate all recipes at load time
- Use efficient lookup structures for crafting checks
- Consider recipe trees for complex dependency analysis

## Future Tech Stack Expectations

Based on project structure (.gitignore patterns):
- **Primary**: C# with Unity (likely for 3D isometric rendering)
- **Data**: JSON for recipes, configurations, save games
- **Architecture**: Component-based systems (Unity ECS or similar)
- **Rendering**: 3D isometric view with hex-grid constraints

## Modding & Extensibility

### Recipe System
- All recipes should be data-driven (JSON files)
- Support runtime recipe loading/validation
- Maintain strict validation of the 2-input rule
- Allow theme packs to modify visuals without breaking logic

### Theme Support
- Separate visual representation from game logic
- Support multiple aesthetic themes (Industrial, Steampunk, Sci-fi, etc.)
- Keep core mechanics unchanged across themes

## Common Pitfalls to Avoid

### ❌ Don't Do This
- Create factories with 3+ inputs (violates core design)
- Use square-grid logic for hex positioning
- Mix visual themes with game logic
- Skip recipe validation
- Create hidden or implicit item transformations
- Use floating-point coordinates for hex positions
- Use `var` for variable declarations when the type isn't immediately obvious

### ✅ Do This Instead
- Always validate 2-input maximum
- Use proper hex coordinate math
- Separate presentation from logic
- Validate recipes at load time
- Make all transformations explicit in UI
- Use integer hex coordinates with proper conversion functions
- Use explicit type declarations for better code clarity

## Code Review Guidelines

When reviewing code, ensure:
1. **Architecture compliance**: No violations of single-output or 2-input rules
2. **Hex awareness**: All spatial code uses hex geometry correctly
3. **Data consistency**: Recipes follow JSON schema and validation
4. **Performance**: Spatial queries are optimized for hex grids
5. **Clarity**: Code reflects the game's design philosophy of "clarity-first"

## Documentation Standards

- Always include tier information in item documentation
- Document hex coordinate conventions clearly
- Provide examples for recipe creation
- Include performance implications for spatial operations
- Reference the main design document (README.md) for context

---

## Quick Reference

**Max inputs per factory**: 2  
**Max outputs per factory**: 1  
**Coordinate system**: Axial (q,r) or Cube (x,y,z)  
**Recipe format**: JSON with strict validation  
**Progression**: Tier-based (0-5)  
**Transport**: Edge-based belts on hex grid  

For complete design context, see [README.md](./README.md).