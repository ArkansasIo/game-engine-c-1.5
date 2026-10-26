using EtherealVeni.Math;

namespace EtherealVeni.World;

public sealed class Transform
{
    public Vector3 Position { get; set; } = Vector3.Zero;
    public Vector3 RotationEuler { get; set; } = Vector3.Zero;
    public Vector3 Scale { get; set; } = Vector3.One;
}
