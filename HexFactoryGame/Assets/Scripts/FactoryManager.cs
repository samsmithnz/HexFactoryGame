using UnityEngine;
using System.Collections.Generic;

public class FactoryManager : MonoBehaviour
{
    [Header("Settings")]
    public float productionInterval = 2.0f; // seconds between production cycles
    
    private HexGridGenerator gridGenerator;
    private float lastProductionTime;
    
    [Header("Resource Tracking")]
    public Dictionary<string, int> resources = new Dictionary<string, int>();
    
    private void Start()
    {
        gridGenerator = FindFirstObjectByType<HexGridGenerator>();
        lastProductionTime = Time.time;
        
        // Initialize resource counters
        resources["iron_ore"] = 0;
        resources["copper_ore"] = 0;
        resources["stone"] = 0;
        resources["iron_ingot"] = 0;
        resources["copper_ingot"] = 0;
        resources["gear"] = 0;
        resources["circuit"] = 0;
    }
    
    private void Update()
    {
        if (Time.time - lastProductionTime >= productionInterval)
        {
            ProcessProduction();
            lastProductionTime = Time.time;
        }
    }
    
    private void ProcessProduction()
    {
        if (gridGenerator == null) return;
        
        // Process all factories in order: Mines -> Smelters -> Assemblers
        ProcessMines();
        ProcessSmelters();
        ProcessAssemblers();
        
        // Update UI or debug output
        PrintResources();
    }
    
    private void ProcessMines()
    {
        for (int x = 0; x < gridGenerator.width; x++)
        {
            for (int y = 0; y < gridGenerator.height; y++)
            {
                HexTile tile = gridGenerator.GetTile(x, y);
                if (tile != null && tile.factoryType == FactoryType.Mine && tile.isActive)
                {
                    ProduceMineOutput(tile);
                }
            }
        }
    }
    
    private void ProcessSmelters()
    {
        for (int x = 0; x < gridGenerator.width; x++)
        {
            for (int y = 0; y < gridGenerator.height; y++)
            {
                HexTile tile = gridGenerator.GetTile(x, y);
                if (tile != null && tile.factoryType == FactoryType.Smelter && tile.isActive)
                {
                    ProcessSmelter(tile);
                }
            }
        }
    }
    
    private void ProcessAssemblers()
    {
        for (int x = 0; x < gridGenerator.width; x++)
        {
            for (int y = 0; y < gridGenerator.height; y++)
            {
                HexTile tile = gridGenerator.GetTile(x, y);
                if (tile != null && tile.factoryType == FactoryType.Assembler && tile.isActive)
                {
                    ProcessAssembler(tile);
                }
            }
        }
    }
    
    private void ProduceMineOutput(HexTile mine)
    {
        // Simple mine production based on factory name
        if (mine.factoryName.Contains("Iron"))
        {
            resources["iron_ore"]++;
        }
        else if (mine.factoryName.Contains("Copper"))
        {
            resources["copper_ore"]++;
        }
        else if (mine.factoryName.Contains("Stone"))
        {
            resources["stone"]++;
        }
    }
    
    private void ProcessSmelter(HexTile smelter)
    {
        // Simple smelting: ore -> ingot (1:1 ratio)
        if (smelter.factoryName.Contains("Iron") && resources["iron_ore"] > 0)
        {
            resources["iron_ore"]--;
            resources["iron_ingot"]++;
        }
        else if (smelter.factoryName.Contains("Copper") && resources["copper_ore"] > 0)
        {
            resources["copper_ore"]--;
            resources["copper_ingot"]++;
        }
    }
    
    private void ProcessAssembler(HexTile assembler)
    {
        // Simple assembly: basic assembler makes gears, advanced makes circuits
        if (assembler.factoryName.Contains("Basic") && resources["iron_ingot"] >= 2)
        {
            resources["iron_ingot"] -= 2;
            resources["gear"]++;
        }
        else if (assembler.factoryName.Contains("Advanced") && resources["copper_ingot"] >= 1 && resources["gear"] >= 1)
        {
            resources["copper_ingot"]--;
            resources["gear"]--;
            resources["circuit"]++;
        }
    }
    
    private void PrintResources()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("=== Factory Resources ===");
        foreach (var kvp in resources)
        {
            sb.AppendLine($"{kvp.Key}: {kvp.Value}");
        }
        Debug.Log(sb.ToString());
    }
    
    public int GetResource(string resourceType)
    {
        return resources.ContainsKey(resourceType) ? resources[resourceType] : 0;
    }
}