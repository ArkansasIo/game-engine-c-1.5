using UnityEngine;

namespace ArkansasIo.GoonzuGameEngine;

public static class GoonzuWorldLoader
{
    public static GoonzuWorldDefinition LoadJson(TextAsset json)
    {
        if (json == null) throw new System.ArgumentNullException(nameof(json));
        var data = JsonUtility.FromJson<GoonzuWorldDefinitionDto>(json.text);
        var definition = ScriptableObject.CreateInstance<GoonzuWorldDefinition>();
        definition.version = data.version;
        definition.style = data.style;
        definition.regions = data.regions ?? System.Array.Empty<GoonzuRegion>();
        definition.towns = data.towns ?? System.Array.Empty<GoonzuTown>();
        definition.resources = data.resources ?? System.Array.Empty<GoonzuResource>();
        definition.starterSkills = data.starterSkills ?? System.Array.Empty<string>();
        definition.starterAssets = data.starterAssets;
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
