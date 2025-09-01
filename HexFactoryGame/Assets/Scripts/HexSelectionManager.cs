using UnityEngine;
using UnityEngine.UI;

public class HexSelectionManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject infoPanel;
    public Text infoText;
    
    private HexTile selectedTile;
    private HexGridGenerator gridGenerator;
    private FactoryManager factoryManager;
    
    void Start()
    {
        gridGenerator = FindFirstObjectByType<HexGridGenerator>();
        factoryManager = FindFirstObjectByType<FactoryManager>();
        CreateInfoPanel();
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                HexTile tile = hit.collider.GetComponent<HexTile>();
                if (tile != null)
                {
                    SelectTile(tile);
                }
            }
        }
        
        // Factory placement hotkeys
        if (selectedTile != null)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                PlaceFactory(FactoryType.Mine, "Mine");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                PlaceFactory(FactoryType.Smelter, "Smelter");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                PlaceFactory(FactoryType.Assembler, "Assembler");
            }
            else if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
            {
                RemoveFactory();
            }
        }
    }
    
    void SelectTile(HexTile tile)
    {
        // Deselect previous tile
        if (selectedTile != null)
        {
            selectedTile.RestoreColor();
        }
        
        // Select new tile
        selectedTile = tile;
        selectedTile.Select();
        UpdateInfoPanel();
    }
    
    void PlaceFactory(FactoryType type, string name)
    {
        if (selectedTile != null)
        {
            selectedTile.SetFactory(type, name);
            UpdateInfoPanel();
        }
    }
    
    void RemoveFactory()
    {
        if (selectedTile != null)
        {
            selectedTile.SetFactory(FactoryType.None, "");
            UpdateInfoPanel();
        }
    }
    
    void CreateInfoPanel()
    {
        // Find or create Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("HexFactoryCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Create info panel
        infoPanel = new GameObject("InfoPanel");
        infoPanel.transform.SetParent(canvas.transform, false);
        
        Image panelImage = infoPanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.7f);
        
        RectTransform panelRect = infoPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(0, 1);
        panelRect.pivot = new Vector2(0, 1);
        panelRect.anchoredPosition = new Vector2(10, -10);
        panelRect.sizeDelta = new Vector2(300, 150);
        
        // Create text
        GameObject textObj = new GameObject("InfoText");
        textObj.transform.SetParent(infoPanel.transform, false);
        
        infoText = textObj.AddComponent<Text>();
        infoText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        infoText.fontSize = 14;
        infoText.color = Color.white;
        infoText.alignment = TextAnchor.UpperLeft;
        
        RectTransform textRect = infoText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 10);
        textRect.offsetMax = new Vector2(-10, -10);
        
        infoText.text = "HexFactoryGame\n\nClick a tile to select\n\nHotkeys:\n1 - Mine\n2 - Smelter\n3 - Assembler\nDel - Remove\n\nWASD - Move\nQ/E - Rotate\nMouse Wheel - Zoom";
    }
    
    void UpdateInfoPanel()
    {
        if (selectedTile == null || infoText == null) return;
        
        string resourceInfo = "";
        if (factoryManager != null)
        {
            resourceInfo = $"\nResources:\nIron Ore: {factoryManager.GetResource("iron_ore")}\nCopper Ore: {factoryManager.GetResource("copper_ore")}\nStone: {factoryManager.GetResource("stone")}\nIron Ingot: {factoryManager.GetResource("iron_ingot")}\nCopper Ingot: {factoryManager.GetResource("copper_ingot")}\nGears: {factoryManager.GetResource("gear")}\nCircuits: {factoryManager.GetResource("circuit")}";
        }
        
        infoText.text = $"HexFactoryGame\n\nSelected: ({selectedTile.gridX}, {selectedTile.gridY})\nFactory: {selectedTile.factoryType}\nName: {selectedTile.factoryName}\nActive: {selectedTile.isActive}\n\nHotkeys:\n1 - Mine\n2 - Smelter\n3 - Assembler\nDel - Remove\n\nWASD - Move\nQ/E - Rotate\nMouse Wheel - Zoom{resourceInfo}";
    }
}