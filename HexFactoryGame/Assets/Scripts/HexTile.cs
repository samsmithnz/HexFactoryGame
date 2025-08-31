using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]
public class HexTile : MonoBehaviour
{
    [Header("Factory Settings")]
    public FactoryType factoryType = FactoryType.None;
    public string factoryName = "";
    public bool isActive = false;
    
    [Header("Visual Settings")]
    public Color emptyColor = Color.gray;
    public Color mineColor = new Color(0.5f, 0.3f, 0.1f, 1.0f); // Brown
    public Color smelterColor = new Color(0.8f, 0.4f, 0.1f, 1.0f); // Orange
    public Color assemblerColor = new Color(0.2f, 0.6f, 0.8f, 1.0f); // Blue
    public Color highlightColor = Color.yellow;
    public Color selectedColor = Color.green;

    [Header("Coordinate")]
    public int gridX = 0;
    public int gridY = 0;

    private Renderer rend;
    private Color originalColor;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        UpdateTileAppearance();
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<MeshCollider>();
        }
    }

    public void SetFactory(FactoryType type, string name)
    {
        factoryType = type;
        factoryName = name;
        isActive = type != FactoryType.None;
        UpdateTileAppearance();
    }

    public void UpdateTileAppearance()
    {
        if (rend == null)
        {
            rend = GetComponent<Renderer>();
        }
        
        switch (factoryType)
        {
            case FactoryType.Mine:
                originalColor = mineColor;
                rend.material.color = mineColor;
                break;
            case FactoryType.Smelter:
                originalColor = smelterColor;
                rend.material.color = smelterColor;
                break;
            case FactoryType.Assembler:
                originalColor = assemblerColor;
                rend.material.color = assemblerColor;
                break;
            default:
                originalColor = emptyColor;
                rend.material.color = emptyColor;
                break;
        }
    }

    public void Highlight()
    {
        if (rend == null)
        {
            rend = GetComponent<Renderer>();
        }
        rend.material.color = highlightColor;
    }

    public void Select()
    {
        if (rend == null)
        {
            rend = GetComponent<Renderer>();
        }
        rend.material.color = selectedColor;
    }

    public void RestoreColor()
    {
        UpdateTileAppearance();
    }

    public void SetGridPosition(int x, int y)
    {
        gridX = x;
        gridY = y;
        gameObject.name = $"HexTile_{x}_{y}";
    }
}

public enum FactoryType
{
    None,
    Mine,
    Smelter,
    Assembler
}