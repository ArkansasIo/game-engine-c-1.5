using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace ArkansasIo.GoonzuGameEngine.Tests;

public sealed class GoonzuCraftingServiceTests
{
    [Test]
    public void CraftConsumesIngredientsAndProducesResult()
    {
        var ore = CreateItem("mat.ore", "Ore", true, 99);
        var sword = CreateItem("weapon.sword", "Iron Sword", false, 1);
        var recipe = ScriptableObject.CreateInstance<GoonzuRecipeDefinition>();
        Set(recipe, "recipeId", "craft.iron_sword");
        Set(recipe, "displayName", "Iron Sword");
        Set(recipe, "result", sword);
        Set(recipe, "resultQuantity", 1);
        Set(recipe, "ingredients", new[] { new GoonzuRecipeIngredient { item = ore, quantity = 2 } });

        var inventory = new GoonzuInventory();
        Assert.That(inventory.Add(ore, 2), Is.True);

        var result = GoonzuCraftingService.Craft(recipe, inventory, 1);

        Assert.That(result.Success, Is.True);
        Assert.That(inventory.GetQuantity("mat.ore"), Is.EqualTo(0));
        Assert.That(inventory.GetQuantity("weapon.sword"), Is.EqualTo(1));

        Object.DestroyImmediate(recipe);
        Object.DestroyImmediate(ore);
        Object.DestroyImmediate(sword);
    }

    [Test]
    public void CraftFailsWithoutMaterialsAndDoesNotMutateInventory()
    {
        var ore = CreateItem("mat.ore", "Ore", true, 99);
        var sword = CreateItem("weapon.sword", "Iron Sword", false, 1);
        var recipe = ScriptableObject.CreateInstance<GoonzuRecipeDefinition>();
        Set(recipe, "recipeId", "craft.iron_sword");
        Set(recipe, "displayName", "Iron Sword");
        Set(recipe, "result", sword);
        Set(recipe, "ingredients", new[] { new GoonzuRecipeIngredient { item = ore, quantity = 2 } });

        var inventory = new GoonzuInventory();
        var result = GoonzuCraftingService.Craft(recipe, inventory, 1);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo(GoonzuCraftingErrors.InsufficientMaterials));
        Assert.That(inventory.GetQuantity("weapon.sword"), Is.EqualTo(0));

        Object.DestroyImmediate(recipe);
        Object.DestroyImmediate(ore);
        Object.DestroyImmediate(sword);
    }

    private static GoonzuItemDefinition CreateItem(string id, string name, bool stackable, int maxStack)
    {
        var item = ScriptableObject.CreateInstance<GoonzuItemDefinition>();
        Set(item, "itemId", id);
        Set(item, "displayName", name);
        Set(item, "stackable", stackable);
        Set(item, "maxStack", maxStack);
        return item;
    }

    private static void Set<T>(object target, string field, T value)
        => target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(target, value);
}
