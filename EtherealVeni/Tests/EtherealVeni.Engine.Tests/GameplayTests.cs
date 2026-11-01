using EtherealVeni.Engine.Gameplay;
using EtherealVeni.Engine.Runtime;
using EtherealVeni.Engine.World;
using Xunit;

namespace EtherealVeni.Engine.Tests;

public sealed class GameplayTests
{
    [Fact]
    public void CharacterStartsAtLevelOneAndProgresses()
    {
        var character = new Character("Test Hero");
        Assert.Equal(1, character.Level);
        character.AddExperience(RpgProgression.RequiredExperience(2));
        Assert.Equal(2, character.Level);
    }

    [Fact]
    public void LevelLookupMatchesCharacterProgression()
    {
        var xp = RpgProgression.RequiredExperience(12);
        Assert.Equal(12, RpgProgression.LevelForExperience(xp));
        Assert.Equal(11, RpgProgression.LevelForExperience(xp - 1));
    }

    [Fact]
    public void InventoryAddAndRemoveAreConsistent()
    {
        var inventory = new Inventory(slotCapacity: 2, weightCapacity: 20);
        inventory.Register(new ItemDefinition("potion", "Healing Potion", ItemCategory.Consumable, MaxStack: 5, Weight: 1));

        Assert.True(inventory.Add("potion", 7).Success);
        Assert.Equal(7, inventory.Count("potion"));
        Assert.Equal(2, inventory.UsedSlots);
        Assert.True(inventory.Remove("potion", 3).Success);
        Assert.Equal(4, inventory.Count("potion"));
    }

    [Fact]
    public void InventoryRejectsCapacityOverflowWithoutPartialMutation()
    {
        var inventory = new Inventory(slotCapacity: 1, weightCapacity: 2);
        inventory.Register(new ItemDefinition("ore", "Ore", ItemCategory.Material, MaxStack: 2, Weight: 1));

        var result = inventory.Add("ore", 3);

        Assert.False(result.Success);
        Assert.Equal(0, inventory.Count("ore"));
        Assert.Equal(0, inventory.UsedSlots);
    }

    [Fact]
    public void QuestCompletesOnlyWhenAllObjectivesAreSatisfied()
    {
        var definition = new QuestDefinition(
            "intro",
            "First Steps",
            "Complete the training",
            new[]
            {
                new QuestObjectiveDefinition("kill", "Defeat a slime", QuestObjectiveKind.Kill, "slime", 2),
                new QuestObjectiveDefinition("collect", "Find a key", QuestObjectiveKind.Collect, "key")
            });
        var quest = new QuestProgress(definition);

        Assert.True(quest.Start());
        Assert.True(quest.Report(QuestObjectiveKind.Kill, "slime", 2));
        Assert.Equal(QuestState.Active, quest.State);
        Assert.True(quest.Report(QuestObjectiveKind.Collect, "key"));
        Assert.Equal(QuestState.Completed, quest.State);
    }

    [Fact]
    public void WorldGenerationIsDeterministicForSameSeedAndDimensions()
    {
        var pipeline = new WorldGenerationPipeline();
        var seed = WorldSeed.FromText("unit-test-world");

        var first = pipeline.Generate(seed, 128, 64);
        var second = pipeline.Generate(seed, 128, 64);

        Assert.Equal(first, second);
        Assert.Equal(first.Features, second.Features);
    }

    [Fact]
    public void EntityStoresAndRemovesComponents()
    {
        var entity = new Entity("Test Entity");
        entity.Add(new Transform());

        Assert.True(entity.TryGet<Transform>(out _));
        Assert.True(entity.Remove<Transform>());
        Assert.False(entity.TryGet<Transform>(out _));
    }
}
