using UnityEngine;

namespace ArkansasIo.GoonzuGameEngine;

/// <summary>Converts a seed-world JSON TextAsset into a validated in-memory world definition.</summary>
public static class GoonzuWorldLoader
{
    public static GoonzuWorldDefinition LoadJson(TextAsset json)
    {
        if (json == null)
            throw new System.ArgumentNullException(nameof(json));

        GoonzuWorldDefinitionDto data;
        try
        {
            data = JsonUtility.FromJson<GoonzuWorldDefinitionDto>(json.text);
        }
        catch (System.Exception exception)
        {
            throw new System.FormatException("Could not parse Goonzu world JSON: " + exception.Message, exception);
        }

        if (data == null)
            throw new System.FormatException("Could not parse Goonzu world JSON: empty or invalid root object.");

        var definition = ScriptableObject.CreateInstance<GoonzuWorldDefinition>();
        definition.version = data.version;
        definition.style = data.style;
        definition.regions = data.regions ?? System.Array.Empty<GoonzuRegion>();
        definition.towns = data.towns ?? System.Array.Empty<GoonzuTown>();
        definition.resources = data.resources ?? System.Array.Empty<GoonzuResource>();
        definition.starterSkills = data.starterSkills ?? System.Array.Empty<string>();
        definition.starterAssets = data.starterAssets;

        var errors = GoonzuWorldValidation.Validate(definition);
        if (errors.Count > 0)
        {
            // DestroyImmediate is required here because the failed object has not been added to a scene.
            Object.DestroyImmediate(definition);
            throw new System.FormatException("Invalid Goonzu world definition:\n- " + string.Join("\n- ", errors));
        }

        return definition;
    }

    [System.Serializable]
    private sealed class GoonzuWorldDefinitionDto
    {
        public string version;
        public string style;
        public GoonzuRegion[] regions;
        public GoonzuTown[] towns;
        public GoonzuResource[] resources;
        public string[] starterSkills;
        public GoonzuStarterAssets starterAssets;
    }
}
