using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace ArkansasIo.GoonzuGameEngine.Tests;

public sealed class GoonzuInventoryTests
{
    [Test]
    public void AddAndRemoveStackedItems()
    {
        var item = CreateItem("mat.iron", "Iron Ore", true, 99);
        var inventory = new GoonzuInventory();
        Assert.That(inventory.Add(item, 25), Is.True);
        Assert.That(inventory.Add(item, 10), Is.True);
        Assert.That(inventory.GetQuantity("mat.iron"), Is.EqualTo(35));
        Assert.That(inventory.Remove("mat.iron", 12), Is.True);
        Assert.That(inventory.GetQuantity("mat.iron"), Is.EqualTo(23));
        Object.DestroyImmediate(item);
    }

    [Test]
    public void StackLimitRejectsOverflow()
    {
        var item = CreateItem("mat.herb", "Herb", true, 10);
        var inventory = new GoonzuInventory();
        Assert.That(inventory.Add(item, 10), Is.True);
        Assert.That(inventory.Add(item, 1), Is.False);
        Assert.That(inventory.GetQuantity("mat.herb"), Is.EqualTo(10));
        Object.DestroyImmediate(item);
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

    private static void Set<T>(GoonzuItemDefinition target, string field, T value)
        => typeof(GoonzuItemDefinition).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(target, value);
}
