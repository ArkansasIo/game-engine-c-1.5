using System;
using UnityEngine;

namespace ArkansasIo.GoonzuGameEngine;

[CreateAssetMenu(menuName = "Goonzu/C 1.5 World Definition", fileName = "GoonzuWorldDefinition")]
public sealed class GoonzuWorldDefinition : ScriptableObject
{
    public string version = "0.3.1";
    public string style = "original_2d_isometric_fantasy";
    public GoonzuRegion[] regions = Array.Empty<GoonzuRegion>();
    public GoonzuTown[] towns = Array.Empty<GoonzuTown>();
    public GoonzuResource[] resources = Array.Empty<GoonzuResource>();
    public string[] starterSkills = Array.Empty<string>();
    public GoonzuStarterAssets starterAssets;
}

[Serializable]
public sealed class GoonzuRegion
{
    public string id = "";
    public string name = "";
    public string biome = "";
    public string[] towns = Array.Empty<string>();
}

[Serializable]
public sealed class GoonzuTown
{
    public string id = "";
    public string name = "";
    [Range(0f, 1f)] public float taxRate;
    public string[] services = Array.Empty<string>();

    // Optional authored world position. Legacy manifests can omit this and use fallback placement.
    public bool hasGridPosition;
    public int gridX;
    public int gridY;
}

[Serializable]
public sealed class GoonzuResource
{
    public string id = "";
    public int tier;
    public string gathering = "";
    public string recipe = "";
}

[Serializable]
public struct GoonzuStarterAssets
{
    public string character;
    public string town;
    public string tileset;
    public string ui;
}
