using UnityEngine;
using HexFactoryGame.Core;
using HexFactoryGame.Factories;

namespace HexFactoryGame.Components
{
    /// <summary>
    /// MonoBehaviour component representing a factory building on the hex grid
    /// Provides Unity integration for the factory system
    /// </summary>
    public class FactoryBuilding : MonoBehaviour
    {
        [Header("Factory Configuration")]
        [SerializeField] private string factoryType = "basic_assembler";
        [SerializeField] private HexCoord hexPosition;
        [SerializeField] private float productionProgress = 0f;
        
        [Header("Visual Settings")]
        [SerializeField] private GameObject outputIndicator;
        [SerializeField] private Transform[] inputPorts;
        [SerializeField] private Color idleColor = Color.gray;
        [SerializeField] private Color workingColor = Color.green;
        [SerializeField] private Color starvedColor = Color.red;
        
        private IFactory factory;
        private HexGrid hexGrid;
        private Renderer mainRenderer;
        
        public HexCoord HexPosition 
        { 
            get => hexPosition; 
            set => hexPosition = value; 
        }
        
        public IFactory Factory => factory;
        
        private void Awake()
        {
            mainRenderer = GetComponent<Renderer>();
            hexGrid = FindObjectOfType<HexGrid>();
            
            // Create factory instance based on type
            CreateFactoryInstance();
        }
        
        private void Start()
        {
            // Position the building at the correct world coordinates
            if (hexGrid != null)
            {
                transform.position = hexGrid.HexToWorldPosition(hexPosition);
            }
            
            // Configure visual elements based on factory type
            ConfigureVisuals();
        }
        
        private void Update()
        {
            // Simple production simulation
            SimulateProduction();
            UpdateVisuals();
        }
        
        private void CreateFactoryInstance()
        {
            factory = factoryType.ToLower() switch
            {
                "mine" => new Mine(),
                "smelter" => new Smelter(),
                "basic_assembler" => new BasicAssembler(),
                _ => new BasicAssembler()
            };
        }
        
        private void ConfigureVisuals()
        {
            if (inputPorts != null && inputPorts.Length > factory.MaxInputs)
            {
                // Hide excess input ports
                for (int i = factory.MaxInputs; i < inputPorts.Length; i++)
                {
                    if (inputPorts[i] != null)
                        inputPorts[i].gameObject.SetActive(false);
                }
            }
        }
        
        private void SimulateProduction()
        {
            // Simple production progress simulation
            if (factory != null)
            {
                productionProgress += Time.deltaTime / factory.CraftTime;
                if (productionProgress >= 1f)
                {
                    OnProductionComplete();
                    productionProgress = 0f;
                }
            }
        }
        
        private void OnProductionComplete()
        {
            Debug.Log($"{factory.FactoryType} at {hexPosition} completed production cycle");
            
            // Animate output indicator
            if (outputIndicator != null)
            {
                StartCoroutine(AnimateOutput());
            }
        }
        
        private System.Collections.IEnumerator AnimateOutput()
        {
            if (outputIndicator != null)
            {
                Vector3 originalScale = outputIndicator.transform.localScale;
                outputIndicator.transform.localScale = originalScale * 1.2f;
                yield return new WaitForSeconds(0.2f);
                outputIndicator.transform.localScale = originalScale;
            }
        }
        
        private void UpdateVisuals()
        {
            if (mainRenderer == null) return;
            
            // Change color based on production state
            Color currentColor = productionProgress > 0.1f ? workingColor : idleColor;
            
            // Check if factory is "starved" (no inputs when needed)
            if (factory.MaxInputs > 0 && productionProgress < 0.1f)
            {
                currentColor = starvedColor;
            }
            
            mainRenderer.material.color = Color.Lerp(mainRenderer.material.color, currentColor, Time.deltaTime * 2f);
        }
        
        /// <summary>
        /// Get neighboring factory buildings
        /// </summary>
        public FactoryBuilding[] GetNeighbors()
        {
            if (hexGrid == null) return new FactoryBuilding[0];
            
            HexCoord[] neighborPositions = hexPosition.GetNeighbors();
            System.Collections.Generic.List<FactoryBuilding> neighbors = new System.Collections.Generic.List<FactoryBuilding>();
            
            foreach (HexCoord neighborPos in neighborPositions)
            {
                FactoryBuilding neighbor = FindFactoryAtPosition(neighborPos);
                if (neighbor != null)
                    neighbors.Add(neighbor);
            }
            
            return neighbors.ToArray();
        }
        
        private FactoryBuilding FindFactoryAtPosition(HexCoord position)
        {
            FactoryBuilding[] allFactories = FindObjectsOfType<FactoryBuilding>();
            foreach (FactoryBuilding factory in allFactories)
            {
                if (factory.hexPosition == position)
                    return factory;
            }
            return null;
        }
        
        private void OnDrawGizmosSelected()
        {
            // Draw hex position and connections
            Gizmos.color = Color.yellow;
            
            if (hexGrid != null)
            {
                Vector3 worldPos = hexGrid.HexToWorldPosition(hexPosition);
                Gizmos.DrawWireCube(worldPos, Vector3.one);
                
                // Draw connections to neighbors
                HexCoord[] neighbors = hexPosition.GetNeighbors();
                Gizmos.color = Color.cyan;
                foreach (HexCoord neighbor in neighbors)
                {
                    Vector3 neighborWorldPos = hexGrid.HexToWorldPosition(neighbor);
                    Gizmos.DrawLine(worldPos, neighborWorldPos);
                }
            }
        }
        
        /// <summary>
        /// Validate that this factory's configuration follows architectural rules
        /// </summary>
        [ContextMenu("Validate Factory")]
        public void ValidateFactory()
        {
            if (factory == null)
            {
                Debug.LogError($"Factory at {hexPosition} has no factory instance!");
                return;
            }
            
            bool isValid = true;
            
            if (factory.MaxInputs > 2)
            {
                Debug.LogError($"Factory {factoryType} violates max inputs rule (has {factory.MaxInputs}, max allowed is 2)");
                isValid = false;
            }
            
            if (factory.MaxOutputs != 1)
            {
                Debug.LogError($"Factory {factoryType} violates single output rule (has {factory.MaxOutputs} outputs)");
                isValid = false;
            }
            
            if (isValid)
            {
                Debug.Log($"Factory {factoryType} at {hexPosition} is valid ✓");
            }
        }
    }
}