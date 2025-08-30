# HexFactoryGame
A new attempt to make a strategy factory game

# Hex Factory Game – Core Design Draft

## Vision
A relaxed–yet–tunable factory automation game set on an isometric 3D hex world. You start with a few resource hexes (Iron Ore, Copper Ore, Stone) and expand into a layered production network. Every building occupies exactly one hex and produces exactly one output item. Logistic connections (roads or conveyor belts) run along hex edges. Progression is driven by:
1. Unlocking new factory types
2. Expanding logistical reach
3. Fulfilling contracts / milestone deliveries for Money & Research
4. Optimizing space, adjacency, and throughput

## Fundamental Principles
- Single-output rule: Every factory produces exactly one item type (no multiple variants).
- One or two inputs only (mines = 0 inputs).
- All transformations are explicit; no hidden byproducts.
- Clarity-first chains; complexity emerges from layering.
- Items travel discretely; throughput matters.
- Spatial constraints & adjacency bonuses create planning depth.

---

## World / Grid
- World: Hex tiles (axial coordinates q,r or cube coordinates x,y,z).
- Tile categories: Terrain (Grass, Hill, Mountain, Water), Resource Node (Iron, Copper, Stone, Optional: Coal/Crystal later).
- Buildables: Roads/Belts (edges), Buildings (centers).
- Elevation (optional later): cost scaling + visual variation.

### Logistics on Hexes
Two possible models (choose one early):
1. Edge Belts: Each edge can host 1 belt direction (or 1 lane each direction if upgraded).
2. Road Network: Items move via carts; intersections = any tile with ≥2 connected edges.

Recommendation: Start with Edge Belts for clarity, introduce Roads (higher capacity, costlier) later.

---

## Currencies & Progression
- Money: Earned by delivering contract items to a Central Hub or Export Depot.
- Research Points (RP): Earned by sending specific “Science Pack” style items to a Laboratory.
- Upkeep (optional stretch): Power cost or maintenance fees to prevent runaway sprawl.
- Tech Unlocks: Gate advanced factories, belts, storage tiers, scanning.

---

## Item Tiering Overview
| Tier | Category           | Examples |
|------|--------------------|----------|
| 0    | Raw Resources      | Iron Ore, Copper Ore, Stone |
| 1    | Basic Refined      | Iron Ingot, Copper Ingot, Stone Brick |
| 2    | Simple Components  | Gear, Wire, Plate, Frame |
| 3    | Composite Parts    | Circuit, Machine Casing, Conveyor Module |
| 4    | Advanced Systems   | Control Unit, Logistics Core |
| 5    | Science / Victory  | Science Pack I/II, Colony Module |

(Keep initial release maybe to Tier 3–4, add higher tiers later.)

---

## Core Items & Chains

### Tier 0 → Tier 1 (Smelting / Shaping)
| Inputs                    | Factory Type | Output        | Base Time (s) | Notes |
|---------------------------|--------------|---------------|---------------|-------|
| Iron Ore                  | Smelter      | Iron Ingot    | 4.0           | +10% speed if adjacent to Coal Depot (if Coal added) |
| Copper Ore                | Smelter      | Copper Ingot  | 4.0           | Symmetry keeps early decisions spatial |
| Stone                     | Kiln         | Stone Brick   | 5.0           | Could consume 0.2 Fuel (optional system) |

### Tier 1 → Tier 2 (Basic Components)
| Inputs                 | Factory Type       | Output         | Time | Ratio |
|------------------------|--------------------|----------------|------|-------|
| Iron Ingot             | Press              | Iron Plate     | 3.0  | 1:1   |
| Iron Ingot             | Cutter             | Iron Rod       | 2.5  | 1:1   |
| Iron Rod + Iron Plate  | Assembly (2-input) | Gear           | 4.0  | 1+1:1 |
| Copper Ingot           | Wire Mill          | Copper Wire    | 2.0  | 1:2 (two wires) |
| Wire + Iron Plate      | Assembly           | Simple Circuit | 5.0  | 1+1:1 |
| Stone Brick            | Masonry            | Foundation     | 6.0  | 1:1 (used for expansions / upgrades) |

(If enforcing strictly one output item quantity=1: For Copper Wire either keep 1:1 and adjust time, or allow quantity >1 but still single item type – acceptable.)

### Tier 2 → Tier 3
| Inputs                               | Factory Type          | Output            | Time | Notes |
|--------------------------------------|-----------------------|-------------------|------|-------|
| Gear + Iron Plate + (optional) Wire* | Advanced Assembly(2?) | Machine Casing    | 8.0  | If >2 inputs needed, split pre-subassemblies to keep 2-input cap. |
| Gear + Copper Wire                   | Motor Assembly        | Motor             | 7.0  | Combines mech + electrical |
| Simple Circuit + Copper Wire         | Electronics Bench     | Control Board     | 8.0  | Feeds into Control Unit |
| Machine Casing + Motor               | Mechanization Plant   | Actuator Module   | 10.0 | Mid-level gating |
| Control Board + Actuator Module      | Integration Plant     | Control Unit      | 12.0 | High-value item |

To obey 2-input limit:
Break multi-input recipes into staged subassemblies.

Example decomposition:
1. Gear + Iron Plate -> Frame
2. Frame + Wire -> Machine Casing

Revised Table (Strict 2-input):
| Stage | Inputs              | Output          |
|-------|---------------------|-----------------|
| A     | Gear + Iron Plate   | Frame           |
| B     | Frame + Copper Wire | Machine Casing  |

### Science / Progression Items
| Inputs                     | Output (Science) | Time | Unlocks Example |
|----------------------------|------------------|------|-----------------|
| Simple Circuit + Gear      | Science Pack I   | 10   | Belts Mk2, Warehouse II |
| Control Board + Actuator   | Science Pack II  | 16   | Advanced Assembly, Research Scanner |
| Control Unit + Machine Casing | Science Pack III | 24 | Endgame modules |

---

## Building Types

| Category       | Building          | Input Slots | Output Slots | Core Purpose |
|----------------|-------------------|-------------|--------------|--------------|
| Extraction     | Iron Mine         | 0           | 1 (Ore)      | Base resource generation |
|                | Copper Mine       | 0           | 1            |              |
|                | Stone Quarry      | 0           | 1            |              |
| Processing     | Smelter           | 1           | 1            | Ore -> Ingot |
|                | Kiln              | 1           | 1            | Stone -> Brick |
| Shaping        | Press / Cutter    | 1           | 1            | Ingot -> Plate/Rod |
| Component      | Wire Mill         | 1           | 1            | Ingot -> Wire |
| Assembly (L1)  | Basic Assembler   | 2           | 1            | Tier 2 items |
| Assembly (L2)  | Advanced Assembler| 2           | 1            | Tier 3 items |
| Integration    | Electronics Bench | 2           | 1            | Circuits etc |
|                | Mechanization Pl. | 2           | 1            | Actuator |
|                | Integration Plant | 2           | 1            | Control Unit |
| Research       | Laboratory        | 1           | 0 (Consumes) | Generates RP |
| Logistics      | Warehouse (I–III) | n (buffer)  | n            | Storage & buffering (internally multi-slot but each outbound belt chooses item) |
|                | Hub / Export Dock | n           | Money/RP     | Contract fulfillment |
| Transport      | Belt (Mk1–Mk3)    | —           | —            | Move items (capacity scaling) |
|                | Road + Cart Stop  | —           | —            | Higher batch movement (later) |
| Utility        | Scanner            | —          | —            | Reveals new resource nodes |
|                | Power Gen (opt)    | Fuel -> Power | Power | Add later if wanting energy layer |

---

## Logistics & Throughput

### Belts
| Tier | Capacity (items / s per edge) | Unlock |
|------|-------------------------------|--------|
| Mk1  | 1.0                           | Start |
| Mk2  | 2.0                           | Science Pack I |
| Mk3  | 4.0                           | Science Pack II |

### Throughput Balancing Guideline
Target each early production chain step to consume 60–80% of a preceding belt so players feel modest pressure to optimize without early bottleneck deadlock.

Example baseline:
- Mine: 0.25 Ore/s (4s per Ore)
- Smelter: Consumes 1 Ore every 4s -> matches one Mine 1:1 (encourage placing in pairs or triangles)
- Press: 1 Ingot every 3s (slightly > Smelter output → mild queueing)
Provide upgrade modules later (Speed Module: +20% craft speed, +25% power).

---

## Adjacency & Spatial Bonuses (Optional Layer)
- Smelter adjacent (edge-touching) to 2+ Mines: -0.5s craft time (capped).
- Assembly adjacent to Warehouse: +10% input pull speed.
- Labs adjacent to 3 different Science Pack feeders: +15% RP yield.

Keep bonuses small (5–15%) to reward layout without making math opaque.

---

## Progression Milestones (Example)
1. Starter: Produce first Iron Ingot.
2. Basic Logistics: Build 10 Belts.
3. Mechanization: Produce first Gear.
4. Electronics Age: Deliver 10 Simple Circuits.
5. Research Online: Deliver 5 Science Pack I.
6. Expansion: Unlock Advanced Assembler.
7. Automation: Sustain 1 Control Unit per minute.
8. Victory (Scenario): Deliver 50 Control Units + 20 Science Pack III (Colony Launch).

---

## Economy / Contracts
Contracts appear every X minutes:
- Offer: Deliver N of Item (e.g., 50 Iron Plates) within T minutes
- Reward: Money + small RP
- Failure: No penalty or small reputation decrease (used later for contract quality tiering)

Dynamic scaling: As average item/min production rises, contract volumes increase.

---

## Difficulty Levers
- Resource Node Richness (Ore per node / depletion yes/no)
- Belt Cost (cheaper for sandbox, expensive for puzzle mode)
- Power Requirement: Off by default; can add later.

---

## Possible Themes / Skins (Brainstorm)

1. Classic Industrial (default): Steel, soot, copper patina.
2. Dieselpunk: Thick rivets, exhaust, amber glow.
3. Steampunk: Brass, gears, steam plumes, Victorian lab.
4. Solar Punk: White ceramic, green roofs, bio-belts with plants.
5. Alien Biotech: Organic pods (Mines = harvesting protrusions; belts = tendrils).
6. Retro-Future 1960s: Clean lines, pastel consoles, punch-card labs.
7. Magitech: Crystals (Iron→Rune Metal), labs = Arcanum Circles, belts = glyph paths.
8. Dwarven Underforge: Lava kilns, rune hammers, stone aesthetic.
9. Sky Archipelago: Floating hex islands; logistics includes air skiffs (roads replaced).
10. Post-Collapse Salvage: Scrap piles as resource nodes; patchwork machines.
11. Arctic Research Colony: Thermal modules; heat network mechanic.
12. Underwater Dome: Pressure-rated modules; oxygen as a resource.
13. Martian Colony: Dust storms slowing belts, solar power peaks.
14. Cyber Neon: Holographic overlays, items = data-chips, circuits glow.
15. Botanical Growth: Farms instead of mines; “processing” is gene-splicing.

You can treat themes as purely cosmetic or as optional rule-sets (e.g., Sky archipelago adds wind-lift logistics differences).

---

## Visual / UI Notes
- Each building shows a single large output icon atop the hex.
- Input ports: small colored sockets on relevant edges, auto-align to connected belts.
- Efficiency overlay: Color gradient (Red = starved, Yellow = partial, Green = full throughput).
- Flow direction arrows on belts fade in when hovered.

---

## Data & Modding Friendliness
Represent recipes as JSON:
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
Enforce max 2 inputs in validator.

---

## Early Game Layout Example (Text Diagram)
(Each letter = Hex)
M = Mine, S = Smelter, P = Press, W = Wire Mill, A = Assembler, H = Hub

      .  M  .
    M  S  S  P
      Belt  A  H
    .  W  .  .

Iron Mine → Smelter → Press → Plate
Second Mine → Smelter → Rod
Plate + Rod → Gear (Assembler)
Copper path (not drawn) feeds Wire Mill → Wire to A for Simple Circuit.

---

## Balancing Strategy (Initial Targets)
- Player should craft first Gear within 10 minutes.
- First Science Pack I within 25–30 minutes.
- Throughput scaling: Doubling base production every ~15–20 minutes with expansion.
- Provide *just enough* friction (space + belt cost) to encourage refactoring.

---

## Potential Extensions
- Power Grid (Generators, Batteries, Overload penalties)
- Pollution / Attracting Hazards
- Trains or Bulk Freight (multi-hex vehicles)
- Modular Upgrades (Speed, Efficiency, Quality for chance at bonus item)
- Blueprint / Copy-Paste system
- Multiplayer Co-op (shared map, claim-based building)
- Seasonal Events (resource surges)

---

## Next Steps
1. Decide on logistic model (edge belts vs road carts).
2. Lock initial recipe list (above) & decompose any 3-input into chained 2-input.
3. Prototype data-driven recipe loader.
4. Implement Mine → Smelter → Assembler vertical slice.
5. Add Hub delivery & simple contract UI.
6. Introduce Science Pack I & tech unlock for Belts Mk2.
7. Iterate on pacing metrics (time per craft, belt speed).

---

## Open Questions
- Will resources deplete? (Suggest: Infinite for first release; add depletion mode later.)
- Add optional fuel (Coal) early or postpone?
- Do Warehouses filter automatically or require config?
- Include diagonal (edge) adjacency bonuses or only direct hex neighbors?
- Fog of War / Exploration scanning?

Document decisions as they’re made to keep scope aligned.

---

## Summary
This design seeds a clear, extensible production loop from three starting resources toward layered high-value products while respecting a strict 1-output, max 2-input factory constraint. The theme system can layer aesthetic or mechanical variations later. Immediate focus: crisp early chain, clear visual feedback, satisfying progression pacing.
