using System;
using System.Runtime.InteropServices;

namespace GameEngine.Maths.Vectors
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2D
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vector2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static readonly Vector2D Zero = new Vector2D(0d, 0d);
        public static readonly Vector2D One = new Vector2D(1d, 1d);
        public static readonly Vector2D UnitX = new Vector2D(1d, 0d);
        public static readonly Vector2D UnitY = new Vector2D(0d, 1d);

        public static Vector2D operator +(Vector2D a, Vector2D b) => new Vector2D(a.X + b.X, a.Y + b.Y);
        public static Vector2D operator -(Vector2D a, Vector2D b) => new Vector2D(a.X - b.X, a.Y - b.Y);

        public static Vector2D operator +(Vector2D a, double b)
            => new Vector2D(a.X + b, a.Y + b);
        public static Vector2D operator ++(Vector2D a)
            => new Vector2D(a.X + 1d, a.Y + 1d);

        public static Vector2D operator *(Vector2D a, double b)
            => new Vector2D(a.X * b, a.Y * b);
        public static Vector2D operator /(Vector2D a, double b)
            => new Vector2D(a.X / b, a.Y / b);

        public static bool operator ==(Vector2D a, Vector2D b)
            => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(Vector2D a, Vector2D b)
            => !(a == b);

        public override string ToString()
            => $"X: {X}, Y: {Y}";
    }
}
