using HexFactoryGame.Recipes;

namespace HexFactoryGame.Factories
{
    /// <summary>
    /// Base abstract factory class implementing common functionality
    /// </summary>
    public abstract class BaseFactory : IFactory
    {
        public abstract string FactoryType { get; }
        public abstract int MaxInputs { get; }
        public virtual int MaxOutputs => 1; // Always 1 per architectural constraint
        public abstract float CraftTime { get; }
        public abstract int Tier { get; }
        
        public virtual bool CanCraft(Recipe recipe)
        {
            if (!RecipeValidator.IsValid(recipe)) return false;
            if (recipe.factory != FactoryType) return false;
            if (recipe.inputs.Count > MaxInputs) return false;
            return true;
        }
    }
    
    /// <summary>
    /// Mine factory: 0 inputs, extracts raw resources
    /// </summary>
    public class Mine : BaseFactory
    {
        public override string FactoryType => "mine";
        public override int MaxInputs => 0; // Mines have no inputs
        public override float CraftTime => 4.0f; // 4 seconds per ore as per specification
        public override int Tier => 0; // Base tier
        
        public override bool CanCraft(Recipe recipe)
        {
            if (!base.CanCraft(recipe)) return false;
            // Mines should have no inputs
            return recipe.inputs.Count == 0;
        }
    }
    
    /// <summary>
    /// Smelter factory: 1 input, processes raw materials into basic products
    /// </summary>
    public class Smelter : BaseFactory
    {
        public override string FactoryType => "smelter";
        public override int MaxInputs => 1; // Single input for smelting
        public override float CraftTime => 4.0f; // 4 seconds per ingot to match mine output
        public override int Tier => 1;
        
        public override bool CanCraft(Recipe recipe)
        {
            if (!base.CanCraft(recipe)) return false;
            // Smelters should have exactly 1 input
            return recipe.inputs.Count == 1;
        }
    }
    
    /// <summary>
    /// Basic Assembler: 1-2 inputs, creates simple manufactured goods
    /// </summary>
    public class BasicAssembler : BaseFactory
    {
        public override string FactoryType => "basic_assembler";
        public override int MaxInputs => 2; // Can handle up to 2 inputs
        public override float CraftTime => 3.0f; // Slightly faster than smelter for flow balance
        public override int Tier => 2;
        
        public override bool CanCraft(Recipe recipe)
        {
            if (!base.CanCraft(recipe)) return false;
            // Basic assembler can handle 1 or 2 inputs
            return recipe.inputs.Count >= 1 && recipe.inputs.Count <= 2;
        }
    }
}