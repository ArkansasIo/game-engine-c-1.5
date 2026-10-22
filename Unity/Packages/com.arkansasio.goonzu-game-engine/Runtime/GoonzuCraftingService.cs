namespace ArkansasIo.GoonzuGameEngine;

public readonly struct GoonzuCraftResult
{
    public readonly bool Success;
    public readonly string ErrorCode;
    public readonly string Message;

    public GoonzuCraftResult(bool success, string errorCode, string message)
    {
        Success = success;
        ErrorCode = errorCode;
        Message = message;
    }
}

public static class GoonzuCraftingErrors
{
    public const string InvalidRecipe = "invalid_recipe";
    public const string InsufficientLevel = "insufficient_level";
    public const string InsufficientMaterials = "insufficient_materials";
    public const string OutputUnavailable = "output_unavailable";
}

public static class GoonzuCraftingService
{
    public static GoonzuCraftResult Craft(GoonzuRecipeDefinition recipe, GoonzuInventory inventory, int playerLevel)
    {
        string error = string.Empty;
        if (recipe == null || inventory == null || !recipe.IsValid(out error))
            return new GoonzuCraftResult(false, GoonzuCraftingErrors.InvalidRecipe, string.IsNullOrEmpty(error) ? "Recipe is invalid." : error);

        if (playerLevel < recipe.RequiredLevel)
            return new GoonzuCraftResult(false, GoonzuCraftingErrors.InsufficientLevel, "Crafting level requirement not met.");

        foreach (var ingredient in recipe.Ingredients)
        {
            if (!inventory.Has(ingredient.item.ItemId, ingredient.quantity))
                return new GoonzuCraftResult(false, GoonzuCraftingErrors.InsufficientMaterials, $"Missing {ingredient.item.DisplayName}.");
        }

        if (!CanReceive(inventory, recipe.Result, recipe.ResultQuantity))
            return new GoonzuCraftResult(false, GoonzuCraftingErrors.OutputUnavailable, "Inventory cannot receive the crafted result.");

        foreach (var ingredient in recipe.Ingredients)
            inventory.Remove(ingredient.item.ItemId, ingredient.quantity);

        inventory.Add(recipe.Result, recipe.ResultQuantity);
        return new GoonzuCraftResult(true, string.Empty, $"Crafted {recipe.ResultQuantity} x {recipe.Result.DisplayName}.");
    }

    private static bool CanReceive(GoonzuInventory inventory, GoonzuItemDefinition item, int quantity)
    {
        if (item == null || quantity <= 0) return false;
        var current = inventory.GetQuantity(item.ItemId);
        if (!item.Stackable) return quantity == 1 && current == 0;
        return quantity <= item.MaxStack - current;
    }
}
