namespace GameEngine.V15.Physics;

public readonly record struct Aabb(Vector3f Min, Vector3f Max)
{
    public bool Contains(Vector3f p) => p.X>=Min.X && p.X<=Max.X && p.Y>=Min.Y && p.Y<=Max.Y && p.Z>=Min.Z && p.Z<=Max.Z;
    public bool Intersects(Aabb b) => Min.X<=b.Max.X && Max.X>=b.Min.X && Min.Y<=b.Max.Y && Max.Y>=b.Min.Y && Min.Z<=b.Max.Z && Max.Z>=b.Min.Z;
}
public sealed class PhysicsWorld
{
    private readonly List<Rigidbody> _bodies = new();
    public void Add(Rigidbody body) => _bodies.Add(body);
    public void Step(float dt) { foreach(var body in _bodies) body.Integrate(dt); }
}
