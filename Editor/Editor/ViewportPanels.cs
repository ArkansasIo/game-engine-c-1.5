using System;
using System.Collections.Generic;

namespace EtherealVeni.Editor;

public sealed class ViewportPanel : IEditorPanel
{
    public string Id => "viewport";
    public string Title => "World Viewport";
    public CameraPreview Camera { get; } = new();
}

public sealed class CameraPreview
{
    public CameraMode Mode { get; set; } = CameraMode.Isometric;
    public float Zoom { get; set; } = 1.0f;
    public float Yaw { get; set; }
    public float Pitch { get; set; } = 35f;
}

public enum CameraMode
{
    Isometric,
    ThirdPerson,
    OverTheShoulder,
    Free,
    Cinematic
}

public sealed class HierarchyPanel : IEditorPanel
{
    public string Id => "hierarchy";
    public string Title => "Hierarchy";
    public List<string> Entities { get; } = new();
}

public sealed class InspectorPanel : IEditorPanel
{
    public string Id => "inspector";
    public string Title => "Inspector";
    public string? SelectedEntity { get; set; }
}

public sealed class AssetBrowserPanel : IEditorPanel
{
    public string Id => "assets";
    public string Title => "Asset Browser";
    public List<string> SearchPaths { get; } = new();
}
