namespace ArkansasIo.GoonzuGameEngine;

public readonly record struct GoonzuCraftResult(bool Success, string ErrorCode, string Message);

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
        if (recipe == null || inventory == null || !recipe.IsValid(out var error))
            return new(false, GoonzuCraftingErrors.InvalidRecipe, error ?? "Recipe is invalid.");

        if (playerLevel < recipe.RequiredLevel)
            return new(false, GoonzuCraftingErrors.InsufficientLevel, "Crafting level requirement not met.");

        foreach (var ingredient in recipe.Ingredients)
        {
            if (!inventory.Has(ingredient.item.ItemId, ingredient.quantity))
                return new(false, GoonzuCraftingErrors.InsufficientMaterials, $"Missing {ingredient.item.DisplayName}.");
        }

        if (!CanReceive(inventory, recipe.Result, recipe.ResultQuantity))
            return new(false, GoonzuCraftingErrors.OutputUnavailable, "Inventory cannot receive the crafted result.");

        foreach (var ingredient in recipe.Ingredients)
            inventory.Remove(ingredient.item.ItemId, ingredient.quantity);

        inventory.Add(recipe.Result, recipe.ResultQuantity);
        return new(true, string.Empty, $"Crafted {recipe.ResultQuantity} x {recipe.Result.DisplayName}.");
    }

    private static bool CanReceive(GoonzuInventory inventory, GoonzuItemDefinition item, int quantity)
    {
        if (item == null || quantity <= 0) return false;
        var current = inventory.GetQuantity(item.ItemId);
        if (!item.Stackable) return quantity == 1 && current == 0;
        return current <= item.MaxStack - quantity;
    }
}
