using System.Collections.Generic;
using UnityEngine;
using HexFactoryGame.Core;
using HexFactoryGame.Factories;

namespace HexFactoryGame.Grid
{
    /// <summary>
    /// Manages factory placement on the hex grid
    /// Enforces one-building-per-hex rule and spatial constraints
    /// </summary>
    public class HexGrid : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private float hexSize = 1.0f;
        [SerializeField] private bool showDebugGizmos = true;
        
        private Dictionary<HexCoord, IFactory> placedFactories = new Dictionary<HexCoord, IFactory>();
        private Dictionary<HexCoord, GameObject> factoryGameObjects = new Dictionary<HexCoord, GameObject>();
        
        /// <summary>
        /// Convert hex coordinates to world position for 3D placement
        /// Using flat-topped hex orientation
        /// </summary>
        public Vector3 HexToWorldPosition(HexCoord hex)
        {
            float x = hexSize * (3.0f / 2.0f * hex.q);
            float z = hexSize * (Mathf.Sqrt(3.0f) / 2.0f * hex.q + Mathf.Sqrt(3.0f) * hex.r);
            return new Vector3(x, 0, z);
        }
        
        /// <summary>
        /// Convert world position to hex coordinates
        /// </summary>
        public HexCoord WorldToHexPosition(Vector3 worldPos)
        {
            float q = (2.0f / 3.0f * worldPos.x) / hexSize;
            float r = (-1.0f / 3.0f * worldPos.x + Mathf.Sqrt(3.0f) / 3.0f * worldPos.z) / hexSize;
            
            return RoundToHex(q, r);
        }
        
        /// <summary>
        /// Round fractional hex coordinates to the nearest integer hex
        /// </summary>
        private HexCoord RoundToHex(float q, float r)
        {
            float s = -q - r;
            
            int rq = Mathf.RoundToInt(q);
            int rr = Mathf.RoundToInt(r);
            int rs = Mathf.RoundToInt(s);
            
            float q_diff = Mathf.Abs(rq - q);
            float r_diff = Mathf.Abs(rr - r);
            float s_diff = Mathf.Abs(rs - s);
            
            if (q_diff > r_diff && q_diff > s_diff)
                rq = -rr - rs;
            else if (r_diff > s_diff)
                rr = -rq - rs;
            
            return new HexCoord(rq, rr);
        }
        
        /// <summary>
        /// Check if a hex position is available for building
        /// </summary>
        public bool IsPositionAvailable(HexCoord position)
        {
            return !placedFactories.ContainsKey(position);
        }
        
        /// <summary>
        /// Place a factory at the specified hex position
        /// </summary>
        public bool PlaceFactory(HexCoord position, IFactory factory, GameObject factoryPrefab)
        {
            if (!IsPositionAvailable(position))
            {
                Debug.LogWarning($"Cannot place factory at {position} - position already occupied");
                return false;
            }
            
            // Place the factory
            placedFactories[position] = factory;
            
            // Instantiate visual representation
            if (factoryPrefab != null)
            {
                Vector3 worldPos = HexToWorldPosition(position);
                GameObject factoryObject = Instantiate(factoryPrefab, worldPos, Quaternion.identity);
                factoryObject.name = $"{factory.FactoryType}_at_{position}";
                factoryGameObjects[position] = factoryObject;
            }
            
            Debug.Log($"Placed {factory.FactoryType} at {position}");
            return true;
        }
        
        /// <summary>
        /// Remove a factory from the specified position
        /// </summary>
        public bool RemoveFactory(HexCoord position)
        {
            if (!placedFactories.ContainsKey(position))
                return false;
                
            placedFactories.Remove(position);
            
            if (factoryGameObjects.TryGetValue(position, out GameObject factoryObject))
            {
                if (factoryObject != null)
                    DestroyImmediate(factoryObject);
                factoryGameObjects.Remove(position);
            }
            
            return true;
        }
        
        /// <summary>
        /// Get the factory at a specific position
        /// </summary>
        public IFactory GetFactory(HexCoord position)
        {
            placedFactories.TryGetValue(position, out IFactory factory);
            return factory;
        }
        
        /// <summary>
        /// Get all neighboring factories
        /// </summary>
        public List<IFactory> GetNeighboringFactories(HexCoord position)
        {
            List<IFactory> neighbors = new List<IFactory>();
            HexCoord[] neighborPositions = position.GetNeighbors();
            
            foreach (HexCoord neighborPos in neighborPositions)
            {
                IFactory neighbor = GetFactory(neighborPos);
                if (neighbor != null)
                    neighbors.Add(neighbor);
            }
            
            return neighbors;
        }
        
        private void OnDrawGizmos()
        {
            if (!showDebugGizmos) return;
            
            Gizmos.color = Color.cyan;
            
            // Draw placed factories
            foreach (var kvp in placedFactories)
            {
                Vector3 worldPos = HexToWorldPosition(kvp.Key);
                Gizmos.DrawWireCube(worldPos, Vector3.one * hexSize);
                
                // Draw hex outline
                DrawHexGizmo(worldPos);
            }
        }
        
        private void DrawHexGizmo(Vector3 center)
        {
            Vector3[] vertices = new Vector3[6];
            for (int i = 0; i < 6; i++)
            {
                float angle = 60f * i * Mathf.Deg2Rad;
                vertices[i] = center + new Vector3(
                    hexSize * Mathf.Cos(angle),
                    0,
                    hexSize * Mathf.Sin(angle)
                );
            }
            
            for (int i = 0; i < 6; i++)
            {
                Gizmos.DrawLine(vertices[i], vertices[(i + 1) % 6]);
            }
        }
    }
}