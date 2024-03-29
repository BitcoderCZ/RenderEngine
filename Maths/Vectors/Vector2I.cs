﻿using System;
using System.Runtime.InteropServices;

namespace GameEngine.Maths.Vectors
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2I
    {
        public int X { get; set; }
        public int Y { get; set; }

        public static readonly Vector2I Zero = new Vector2I(0, 0);
        public static readonly Vector2I One = new Vector2I(1, 1);
        public static readonly Vector2I UnitX = new Vector2I(1, 0);
        public static readonly Vector2I UnitY = new Vector2I(0, 1);

        public Vector2I(int x, int y)
        {
            X = x;
            Y = y;
        }

        #region operators

        public static Vector2I operator +(Vector2I a, Vector2I b) => new Vector2I(a.X + b.X, a.Y + b.Y);
        public static Vector2I operator -(Vector2I a, Vector2I b) => new Vector2I(a.X - b.X, a.Y - b.Y);
        public static Vector2I operator *(Vector2I a, Vector2I b) => new Vector2I(a.X * b.X, a.Y * b.Y);
        public static Vector2I operator /(Vector2I a, Vector2I b) => new Vector2I(a.X / b.X, a.Y / b.Y);
        public static Vector2I operator %(Vector2I a, Vector2I b) => new Vector2I(a.X % b.X, a.Y % b.Y);

        public static Vector2I operator +(Vector2I a, int b) => new Vector2I(a.X + b, a.Y + b);
        public static Vector2I operator -(Vector2I a, int b) => new Vector2I(a.X - b, a.Y - b);
        public static Vector2I operator *(Vector2I a, int b) => new Vector2I(a.X * b, a.Y * b);
        public static Vector2I operator /(Vector2I a, int b) => new Vector2I(a.X / b, a.Y / b);
        public static Vector2I operator %(Vector2I a, int b) => new Vector2I(a.X % b, a.Y % b);

        public static Vector2I operator -(Vector2I a) => new Vector2I(-a.X, -a.Y);

        public static Vector2I operator ++(Vector2I a) => new Vector2I(a.X + 1, a.Y + 1);
        public static Vector2I operator --(Vector2I a) => new Vector2I(a.X - 1, a.Y - 1);

        public static bool operator ==(Vector2I a, Vector2I b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(Vector2I a, Vector2I b) => a.X != b.X || a.Y != b.Y;

        public static implicit operator Vector2F(Vector2I a) => new Vector2F(a.X, a.Y);
        public static implicit operator Vector2D(Vector2I a) => new Vector2D(a.X, a.Y);

        #endregion

        public override string ToString()
           => $"X: {X}, Y: {Y}";

        public override bool Equals(object obj)
        {
            if (this == null && obj == null)
                return true;
            else if (obj == null)
                return false;
            else if (obj is Vector2I other)
                return this == other;
            else
                return false;
        }

        public override int GetHashCode()
            => new { X, Y }.GetHashCode();
    }
}
