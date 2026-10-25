using System;
using EtherealVeni.Platform;

namespace EtherealVeni.Rendering;

public interface IGraphicsDevice : IDisposable
{
    GraphicsBackend Backend { get; }
    void Initialize(int width, int height);
    void BeginFrame();
    void EndFrame();
}

public sealed class GraphicsDeviceDescriptor
{
    public GraphicsBackend PreferredBackend { get; init; } = GraphicsBackend.Vulkan;
    public bool EnableValidation { get; init; }
    public bool EnableDebugMarkers { get; init; }
}
