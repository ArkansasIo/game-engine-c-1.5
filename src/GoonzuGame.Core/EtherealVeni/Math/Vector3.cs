namespace EtherealVeni.Math;

public readonly record struct Vector3(double X, double Y, double Z)
{
    public static readonly Vector3 Zero = new(0, 0, 0);
    public static readonly Vector3 One = new(1, 1, 1);
    public double Length => System.Math.Sqrt(X * X + Y * Y + Z * Z);
    public double LengthSquared => X * X + Y * Y + Z * Z;
    public Vector3 Normalized => LengthSquared <= double.Epsilon ? Zero : this / Length;
    public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Vector3 operator *(Vector3 a, double s) => new(a.X * s, a.Y * s, a.Z * s);
    public static Vector3 operator /(Vector3 a, double s) => s == 0 ? throw new DivideByZeroException() : new(a.X / s, a.Y / s, a.Z / s);
    public static double Dot(Vector3 a, Vector3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    public static Vector3 Cross(Vector3 a, Vector3 b) => new(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
    public static double Distance(Vector3 a, Vector3 b) => (a - b).Length;
}
