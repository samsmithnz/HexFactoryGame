using NUnit.Framework;
using HexFactoryGame.Core;

namespace HexFactoryGame.Tests
{
    /// <summary>
    /// Tests for hex coordinate system functionality
    /// </summary>
    public class HexCoordTests
    {
        [Test]
        public void HexCoord_ShouldConvertToCubeCorrectly()
        {
            HexCoord axial = new HexCoord(1, -1);
            CubeCoord cube = axial.ToCube();
            
            // Verify cube constraint: x + y + z = 0
            Assert.AreEqual(0, cube.x + cube.y + cube.z);
            
            // Verify conversion values
            Assert.AreEqual(1, cube.x);
            Assert.AreEqual(0, cube.y);
            Assert.AreEqual(-1, cube.z);
        }
        
        [Test]
        public void CubeCoord_ShouldConvertToAxialCorrectly()
        {
            CubeCoord cube = new CubeCoord(1, 0, -1);
            HexCoord axial = cube.ToAxial();
            
            Assert.AreEqual(1, axial.q);
            Assert.AreEqual(-1, axial.r);
        }
        
        [Test]
        public void CubeCoord_ShouldValidateConstraint()
        {
            CubeCoord validCube = new CubeCoord(1, -1, 0);
            CubeCoord invalidCube = new CubeCoord(1, 2, 3);
            
            Assert.IsTrue(validCube.IsValid());
            Assert.IsFalse(invalidCube.IsValid());
        }
        
        [Test]
        public void HexCoord_ShouldCalculateDistanceCorrectly()
        {
            HexCoord origin = new HexCoord(0, 0);
            HexCoord neighbor = new HexCoord(1, 0);
            HexCoord distant = new HexCoord(2, -1);
            
            Assert.AreEqual(1, origin.DistanceTo(neighbor));
            Assert.AreEqual(2, origin.DistanceTo(distant));
        }
        
        [Test]
        public void HexCoord_ShouldGetCorrectNeighbors()
        {
            HexCoord center = new HexCoord(0, 0);
            HexCoord[] neighbors = center.GetNeighbors();
            
            Assert.AreEqual(6, neighbors.Length);
            
            // Verify each neighbor is distance 1 from center
            foreach (HexCoord neighbor in neighbors)
            {
                Assert.AreEqual(1, center.DistanceTo(neighbor));
            }
        }
        
        [Test]
        public void HexCoord_ShouldTestEqualityCorrectly()
        {
            HexCoord hex1 = new HexCoord(1, 2);
            HexCoord hex2 = new HexCoord(1, 2);
            HexCoord hex3 = new HexCoord(2, 1);
            
            Assert.AreEqual(hex1, hex2);
            Assert.AreNotEqual(hex1, hex3);
            Assert.IsTrue(hex1 == hex2);
            Assert.IsTrue(hex1 != hex3);
        }
    }
}