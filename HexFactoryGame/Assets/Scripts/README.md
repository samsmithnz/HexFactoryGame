# HexFactoryGame - Implementation Using HexGame Foundation

This implementation reuses the hex coordinate system and grid components from the existing HexGame project, adapting them for factory automation gameplay.

## Core Components (Reused from HexGame)

### HexagonMesh.cs
- Procedural hexagon mesh generation
- Handles hex tile 3D visualization
- Adapted from HexGame with factory-appropriate default colors

### HexColor.cs
- Color enumeration for different tile states
- Extended from HexGame's original Blue/Green to include factory colors

### HexGridGenerator.cs
- Grid generation logic using proper hex mathematics
- Even-q vertical layout with correct neighbor calculations
- Adapted from HexGame's grid system for factory placement

## Factory-Specific Components

### HexTile.cs
- Adapted from HexGame's tile system
- Added factory-specific properties: FactoryType, factoryName, isActive
- Color coding: Brown (mines), Orange (smelters), Blue (assemblers)

### FactoryManager.cs
- Production logic for mines → smelters → assemblers
- Simple resource tracking and processing
- Follows the architectural constraint of max 2 inputs per factory

### HexSelectionManager.cs
- Adapted from HexGame's selection system
- Interactive factory placement with hotkeys (1-3)
- Real-time resource display

### CameraControl.cs
- Reused from HexGame
- WASD movement, Q/E rotation, mouse wheel zoom

### GameBootstrap.cs
- Automatic scene setup and component initialization
- Creates demo factory layout

## Factory System Design

**Strict Architectural Compliance:**
- **Mines**: 0 inputs → 1 output (raw resources)
- **Smelters**: 1 input → 1 output (ore → ingot)
- **Assemblers**: 1-2 inputs → 1 output (components → products)

**Current Production Chain:**
1. Iron Mine → Iron Smelter → Basic Assembler (makes gears from 2 iron ingots)
2. Copper Mine → Copper Smelter → Advanced Assembler (1 copper ingot + 1 gear → circuit)

## Usage

1. Click tiles to select them
2. Press hotkeys to place factories:
   - `1` - Mine
   - `2` - Smelter  
   - `3` - Assembler
   - `Delete` - Remove factory
3. Watch automated production in real-time
4. Camera controls: WASD (move), Q/E (rotate), Mouse Wheel (zoom)

## Technical Integration

The implementation successfully integrates HexGame's proven hex coordinate system with factory automation mechanics, maintaining:
- Proper hex grid mathematics
- Efficient neighbor calculations
- Visual consistency with hex world layout
- Modular component architecture

Production cycles run every 2 seconds, with debug output showing resource accumulation as the factory network operates.