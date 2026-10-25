namespace EtherealVeni.Platform;

public enum PlatformTarget
{
    WindowsX64,
    LinuxX64,
    MacOSX64,
    MacOSArm64,
    PlayStation5,
    Xbox,
    NintendoSwitch,
    NintendoSwitch2
}

public enum GraphicsBackend
{
    DirectX12,
    Vulkan,
    SDL3
}

public sealed record PlatformCapabilities(
    PlatformTarget Target,
    GraphicsBackend Backend,
    bool RequiresFirstPartySdk,
    bool SupportsController,
    bool SupportsKeyboardMouse);
