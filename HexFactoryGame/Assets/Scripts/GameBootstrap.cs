using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject hexTilePrefab;
    
    [Header("Grid Settings")]
    public int gridWidth = 10;
    public int gridHeight = 8;
    public float hexRadius = 1f;
    
    void Start()
    {
        SetupGame();
    }
    
    void SetupGame()
    {
        // If no prefab is assigned, create one programmatically
        if (hexTilePrefab == null)
        {
            hexTilePrefab = CreateHexTilePrefab();
        }
        
        // Set up the grid generator
        HexGridGenerator gridGen = FindFirstObjectByType<HexGridGenerator>();
        if (gridGen == null)
        {
            GameObject gridObj = new GameObject("HexGrid");
            gridGen = gridObj.AddComponent<HexGridGenerator>();
        }
        
        gridGen.hexTilePrefab = hexTilePrefab;
        gridGen.width = gridWidth;
        gridGen.height = gridHeight;
        gridGen.hexRadius = hexRadius;
        gridGen.generateDemoLayout = true;
        
        // Ensure we have a factory manager
        if (FindFirstObjectByType<FactoryManager>() == null)
        {
            GameObject factoryObj = new GameObject("FactoryManager");
            factoryObj.AddComponent<FactoryManager>();
        }
        
        // Ensure we have a selection manager
        if (FindFirstObjectByType<HexSelectionManager>() == null)
        {
            GameObject selectionObj = new GameObject("HexSelectionManager");
            selectionObj.AddComponent<HexSelectionManager>();
        }
        
        // Set up camera controls
        Camera mainCamera = Camera.main;
        if (mainCamera != null && mainCamera.GetComponent<CameraControl>() == null)
        {
            mainCamera.gameObject.AddComponent<CameraControl>();
        }
    }
    
    GameObject CreateHexTilePrefab()
    {
        GameObject prefab = new GameObject("HexTile");
        
        // Add required components
        prefab.AddComponent<HexagonMesh>();
        prefab.AddComponent<HexTile>();
        prefab.AddComponent<MeshFilter>();
        prefab.AddComponent<MeshRenderer>();
        prefab.AddComponent<MeshCollider>();
        
        return prefab;
    }
}