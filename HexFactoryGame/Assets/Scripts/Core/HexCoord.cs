using System;
using UnityEngine;

namespace HexFactoryGame.Core
{
    /// <summary>
    /// Preferred axial coordinate structure for hex positioning
    /// Use for storage and UI operations
    /// </summary>
    [System.Serializable]
    public struct HexCoord : IEquatable<HexCoord>
    {
        public int q; // column
        public int r; // row
        
        public HexCoord(int q, int r)
        {
            this.q = q;
            this.r = r;
        }
        
        /// <summary>
        /// Convert to cube coordinates for calculations
        /// </summary>
        public CubeCoord ToCube()
        {
            int x = q;
            int z = r;
            int y = -x - z; // cube constraint: x + y + z = 0
            return new CubeCoord(x, y, z);
        }
        
        /// <summary>
        /// Get neighbors in axial coordinates
        /// </summary>
        public HexCoord[] GetNeighbors()
        {
            return new HexCoord[]
            {
                new HexCoord(q + 1, r + 0), // East
                new HexCoord(q + 1, r - 1), // Northeast
                new HexCoord(q + 0, r - 1), // Northwest
                new HexCoord(q - 1, r + 0), // West
                new HexCoord(q - 1, r + 1), // Southwest
                new HexCoord(q + 0, r + 1)  // Southeast
            };
        }
        
        /// <summary>
        /// Calculate distance to another hex (using cube coordinates)
        /// </summary>
        public int DistanceTo(HexCoord other)
        {
            CubeCoord thisCube = this.ToCube();
            CubeCoord otherCube = other.ToCube();
            return thisCube.DistanceTo(otherCube);
        }
        
        public bool Equals(HexCoord other)
        {
            return q == other.q && r == other.r;
        }
        
        public override bool Equals(object? obj)
        {
            return obj is HexCoord other && Equals(other);
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(q, r);
        }
        
        public static bool operator ==(HexCoord left, HexCoord right)
        {
            return left.Equals(right);
        }
        
        public static bool operator !=(HexCoord left, HexCoord right)
        {
            return !left.Equals(right);
        }
        
        public override string ToString()
        {
            return $"HexCoord({q}, {r})";
        }
    }
}