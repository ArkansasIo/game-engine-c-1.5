namespace GameEngine.V15.Crafting;

public sealed record Ingredient(string ItemId,int Quantity);
public sealed record Recipe(string Id,string OutputId,int OutputQuantity,IReadOnlyList<Ingredient> Ingredients);
public sealed class CraftingSystem
{
    public bool CanCraft(Recipe recipe,Func<string,int> count)=>recipe.Ingredients.All(i=>count(i.ItemId)>=i.Quantity);
}
