using System;

namespace HexFactoryGame.Core
{
    /// <summary>
    /// Cube coordinate structure for hex calculations
    /// Use for mathematical operations and pathfinding
    /// Constraint: x + y + z = 0
    /// </summary>
    [System.Serializable]
    public struct CubeCoord : IEquatable<CubeCoord>
    {
        public int x, y, z; // where x + y + z = 0
        
        public CubeCoord(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        
        /// <summary>
        /// Convert to axial coordinates for storage
        /// </summary>
        public HexCoord ToAxial()
        {
            int q = x;
            int r = z;
            return new HexCoord(q, r);
        }
        
        /// <summary>
        /// Validate that the cube coordinate constraint is satisfied
        /// </summary>
        public bool IsValid()
        {
            return x + y + z == 0;
        }
        
        /// <summary>
        /// Calculate distance to another cube coordinate
        /// </summary>
        public int DistanceTo(CubeCoord other)
        {
            return (Math.Abs(x - other.x) + Math.Abs(y - other.y) + Math.Abs(z - other.z)) / 2;
        }
        
        /// <summary>
        /// Get neighbors in cube coordinates
        /// </summary>
        public CubeCoord[] GetNeighbors()
        {
            return new CubeCoord[]
            {
                new CubeCoord(x + 1, y - 1, z + 0), // East
                new CubeCoord(x + 1, y + 0, z - 1), // Northeast
                new CubeCoord(x + 0, y + 1, z - 1), // Northwest
                new CubeCoord(x - 1, y + 1, z + 0), // West
                new CubeCoord(x - 1, y + 0, z + 1), // Southwest
                new CubeCoord(x + 0, y - 1, z + 1)  // Southeast
            };
        }
        
        public bool Equals(CubeCoord other)
        {
            return x == other.x && y == other.y && z == other.z;
        }
        
        public override bool Equals(object? obj)
        {
            return obj is CubeCoord other && Equals(other);
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(x, y, z);
        }
        
        public static bool operator ==(CubeCoord left, CubeCoord right)
        {
            return left.Equals(right);
        }
        
        public static bool operator !=(CubeCoord left, CubeCoord right)
        {
            return !left.Equals(right);
        }
        
        public override string ToString()
        {
            return $"CubeCoord({x}, {y}, {z})";
        }
    }
}