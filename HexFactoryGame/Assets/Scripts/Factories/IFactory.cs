using HexFactoryGame.Recipes;

namespace HexFactoryGame.Factories
{
    /// <summary>
    /// Base factory interface following strict architectural rules:
    /// - Single-output rule: Every factory produces exactly ONE item type
    /// - Max 2 inputs: Factories can have 0 (mines), 1, or 2 input slots - never more
    /// - Explicit transformations: No hidden byproducts or surprise outputs
    /// - One hex per building: Each building occupies exactly one hex tile
    /// </summary>
    public interface IFactory
    {
        string FactoryType { get; }
        int MaxInputs { get; } // 0, 1, or 2
        int MaxOutputs { get; } // Always 1
        bool CanCraft(Recipe recipe);
        float CraftTime { get; }
        int Tier { get; }
    }
}