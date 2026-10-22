using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace ArkansasIo.GoonzuGameEngine.Tests;

public sealed class GoonzuWorldValidationTests
{
    [Test]
    public void SeedWorldPassesValidation()
    {
        var world = ScriptableObject.CreateInstance<GoonzuWorldDefinition>();
        world.version = "1";
        world.towns = new[]
        {
            new GoonzuTown { id = "town-a", name = "Town A", taxRate = 0.05f },
            new GoonzuTown { id = "town-b", name = "Town B", taxRate = 0.08f }
        };
        world.regions = new[]
        {
            new GoonzuRegion { id = "region-a", name = "Region A", towns = new[] { "town-a", "town-b" } }
        };
        world.resources = new[]
        {
            new GoonzuResource { id = "iron-ore", tier = 1, gathering = "mining" }
        };

        try
        {
            Assert.That(GoonzuWorldValidation.Validate(world), Is.Empty);
        }
        finally
        {
            Object.DestroyImmediate(world);
        }
    }

    [Test]
    public void DuplicateIdsAndInvalidTaxAreReported()
    {
        var world = ScriptableObject.CreateInstance<GoonzuWorldDefinition>();
        world.version = "1";
        world.towns = new[]
        {
            new GoonzuTown { id = "same", name = "Town A", taxRate = 0.05f },
            new GoonzuTown { id = "same", name = "Town B", taxRate = 1.5f }
        };
        world.regions = new[]
        {
            new GoonzuRegion { id = "region-a", name = "Region A", towns = new[] { "unknown" } }
        };

        try
        {
            var errors = GoonzuWorldValidation.Validate(world);
            Assert.That(errors.Any(e => e.Contains("Duplicate town ID")), Is.True);
            Assert.That(errors.Any(e => e.Contains("Tax rate")), Is.True);
            Assert.That(errors.Any(e => e.Contains("unknown town")), Is.True);
        }
        finally
        {
            Object.DestroyImmediate(world);
        }
    }

    [Test]
    public void NullWorldReturnsDiagnostic()
    {
        var errors = GoonzuWorldValidation.Validate(null);
        Assert.That(errors.Count, Is.EqualTo(1));
        Assert.That(errors[0], Does.Contain("null"));
    }
}
