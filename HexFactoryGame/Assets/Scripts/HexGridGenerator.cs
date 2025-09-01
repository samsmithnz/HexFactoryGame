using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject hexTilePrefab;
    public int width = 10;
    public int height = 8;
    
    [Header("Visual Settings")]
    public float hexRadius = 1f;
    public Material defaultMaterial;

    [Header("Demo Factory Layout")]
    public bool generateDemoLayout = true;

    private HexTile[,] hexGrid;

    private void Start()
    {
        GenerateGrid();
        if (generateDemoLayout)
        {
            CreateDemoFactoryLayout();
        }
    }

    public void GenerateGrid()
    {
        // Clear any existing tiles
        foreach (Transform child in transform)
        {
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }

        // Initialize grid array
        hexGrid = new HexTile[width, height];

        // Correct hexagon tiling mathematics
        float horizontalSpacing = hexRadius * 1.5f;
        float verticalSpacing = hexRadius * Mathf.Sqrt(3f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xPos = x * horizontalSpacing;
                float zPos = y * verticalSpacing;
                
                // Offset every other column
                if (x % 2 == 1)
                {
                    zPos += verticalSpacing * 0.5f;
                }
                
                Vector3 pos = new Vector3(xPos, 0, zPos);
                GameObject tile = Instantiate(hexTilePrefab, pos, Quaternion.identity, this.transform);
                
                // Set up the hex tile component
                HexTile hexTile = tile.GetComponent<HexTile>();
                if (hexTile != null)
                {
                    hexTile.SetGridPosition(x, y);
                    hexGrid[x, y] = hexTile;
                }

                // Set up the mesh component
                HexagonMesh mesh = tile.GetComponent<HexagonMesh>();
                if (mesh != null)
                {
                    mesh.SetRadius(hexRadius);
                    mesh.defaultMaterial = defaultMaterial;
                    mesh.Regenerate();
                }
            }
        }
    }

    private void CreateDemoFactoryLayout()
    {
        // Create a simple production chain demonstration
        if (hexGrid == null) return;

        // Place some mines
        PlaceFactory(1, 1, FactoryType.Mine, "Iron Mine");
        PlaceFactory(3, 1, FactoryType.Mine, "Copper Mine");
        PlaceFactory(5, 1, FactoryType.Mine, "Stone Quarry");

        // Place smelters
        PlaceFactory(1, 3, FactoryType.Smelter, "Iron Smelter");
        PlaceFactory(3, 3, FactoryType.Smelter, "Copper Smelter");

        // Place assemblers
        PlaceFactory(2, 5, FactoryType.Assembler, "Basic Assembler");
        PlaceFactory(4, 5, FactoryType.Assembler, "Advanced Assembler");
    }

    private void PlaceFactory(int x, int y, FactoryType type, string name)
    {
        if (x >= 0 && x < width && y >= 0 && y < height && hexGrid[x, y] != null)
        {
            hexGrid[x, y].SetFactory(type, name);
        }
    }

    public HexTile GetTile(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return hexGrid[x, y];
        }
        return null;
    }

    public HexTile[] GetNeighbors(int x, int y)
    {
        // Use correct neighbor offsets for even-q vertical layout
        (int dx, int dy)[] evenOffsets = new (int, int)[] { (1, 0), (-1, 0), (0, 1), (0, -1), (1, -1), (-1, -1) };
        (int dx, int dy)[] oddOffsets = new (int, int)[] { (1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (-1, 1) };
        (int dx, int dy)[] neighborOffsets = (x % 2 == 0) ? evenOffsets : oddOffsets;

        System.Collections.Generic.List<HexTile> neighbors = new System.Collections.Generic.List<HexTile>();
        
        foreach ((int dx, int dy) in neighborOffsets)
        {
            int nx = x + dx;
            int ny = y + dy;
            HexTile neighbor = GetTile(nx, ny);
            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }
        
        return neighbors.ToArray();
    }
}