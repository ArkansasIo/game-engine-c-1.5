using System;
using System.Collections.Generic;

namespace ArkansasIo.GoonzuGameEngine;

/// <summary>
/// Validates imported world definitions and returns actionable errors instead of failing later
/// when the bootstrap attempts to build scene objects.
/// </summary>
public static class GoonzuWorldValidation
{
    public static IReadOnlyList<string> Validate(GoonzuWorldDefinition world)
    {
        var errors = new List<string>();
        if (world == null)
        {
            errors.Add("World definition is null.");
            return errors;
        }

        if (string.IsNullOrWhiteSpace(world.version))
            errors.Add("World version is required.");

        var townIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var town in world.towns ?? Array.Empty<GoonzuTown>())
        {
            if (town == null)
            {
                errors.Add("Town list contains a null entry.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(town.id))
                errors.Add("Town ID is required.");
            else if (!townIds.Add(town.id))
                errors.Add("Duplicate town ID: " + town.id);

            if (string.IsNullOrWhiteSpace(town.name))
                errors.Add("Town name is required: " + (town.id ?? "<missing-id>"));

            if (float.IsNaN(town.taxRate) || float.IsInfinity(town.taxRate) ||
                town.taxRate < 0f || town.taxRate > 1f)
                errors.Add("Tax rate must be between 0 and 1 for town: " + (town.id ?? "<missing-id>"));
        }

        var regionIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var region in world.regions ?? Array.Empty<GoonzuRegion>())
        {
            if (region == null)
            {
                errors.Add("Region list contains a null entry.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(region.id))
                errors.Add("Region ID is required.");
            else if (!regionIds.Add(region.id))
                errors.Add("Duplicate region ID: " + region.id);

            foreach (var townId in region.towns ?? Array.Empty<string>())
            {
                if (string.IsNullOrWhiteSpace(townId) || !townIds.Contains(townId))
                    errors.Add("Region " + (region.id ?? "<missing-id>") + " references unknown town: " + (townId ?? "<null>"));
            }
        }

        var resourceIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var resource in world.resources ?? Array.Empty<GoonzuResource>())
        {
            if (resource == null)
            {
                errors.Add("Resource list contains a null entry.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(resource.id))
                errors.Add("Resource ID is required.");
            else if (!resourceIds.Add(resource.id))
                errors.Add("Duplicate resource ID: " + resource.id);

            if (resource.tier < 0)
                errors.Add("Resource tier cannot be negative: " + (resource.id ?? "<missing-id>"));
        }

        return errors;
    }
}
