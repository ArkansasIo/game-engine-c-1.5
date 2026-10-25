namespace GameEngine.V15.Physics;

public readonly record struct Vector3f(float X,float Y,float Z)
{
    public static Vector3f Zero => new(0,0,0);
    public static Vector3f operator +(Vector3f a, Vector3f b) => new(a.X+b.X,a.Y+b.Y,a.Z+b.Z);
    public static Vector3f operator *(Vector3f a,float s) => new(a.X*s,a.Y*s,a.Z*s);
}
public sealed class Rigidbody
{
    public Vector3f Position { get; set; }
    public Vector3f Velocity { get; set; }
    public float Mass { get; set; } = 1;
    public void AddForce(Vector3f force,float dt) => Velocity += force * (dt / Math.Max(Mass,0.0001f));
    public void Integrate(float dt) => Position += Velocity * dt;
}
