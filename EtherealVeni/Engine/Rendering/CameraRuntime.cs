using EtherealVeni.Engine.Runtime;

namespace EtherealVeni.Engine.Rendering;

public enum CameraMode
{
    Isometric,
    ThirdPerson,
    OverTheShoulder,
    Cinematic,
    Free
}

public sealed class CameraState
{
    public CameraMode Mode { get; set; } = CameraMode.Isometric;
    public Vector3 Position { get; set; } = new(0, -12, 10);
    public Vector3 Target { get; set; } = Vector3.Zero;
    public double FieldOfView { get; set; } = 60;
    public double Distance { get; set; } = 14;
}

public sealed class CameraController
{
    public CameraState State { get; } = new();

    public void SetMode(CameraMode mode) => State.Mode = mode;

    public void SetTarget(Vector3 target) => State.Target = target;

    public void Zoom(double delta)
    {
        State.Distance = Math.Clamp(State.Distance + delta, 2, 100);
    }
}
