namespace GameEngine.V15.Rendering;

public sealed class RenderCamera
{
    public float X {get;set;} public float Y {get;set;} public float Z {get;set;}
    public float Fov {get;set;}=60; public float NearClip {get;set;}=0.1f; public float FarClip {get;set;}=10000;
}
public readonly record struct RenderColor(byte R,byte G,byte B,byte A=255);
public interface IRenderBackend { void BeginFrame(); void EndFrame(); }
