using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Maths.Vectors
{
    public struct Vector2F
    {
        public float X { get; set; }
        public float Y { get; set; }

        public static readonly Vector2F Zero = new Vector2F(0f, 0f);
        public static readonly Vector2F One = new Vector2F(1f, 1f);

        public Vector2F(float x, float y)
        {
            X = x;
            Y = y;
        }

        #region operators

        public static Vector2F operator +(Vector2F a, Vector2F b)
            => new Vector2F(a.X + b.X, a.Y + b.Y);
        public static Vector2F operator -(Vector2F a, Vector2F b)
            => new Vector2F(a.X - b.X, a.Y - b.Y);
        public static Vector2F operator -(Vector2F a)
            => new Vector2F(-a.X, -a.Y);
        public static Vector2F operator *(Vector2F a, float b)
            => new Vector2F(a.X * b, a.Y * b);
        public static Vector2F operator /(Vector2F a, float b)
            => new Vector2F(a.X / b, a.Y / b);

        public static explicit operator Vector2I(Vector2F a)
            => new Vector2I((int)a.X, (int)a.Y);
        public static implicit operator Vector2D(Vector2F a)
            => new Vector2D(a.X, a.Y);

        public static bool operator ==(Vector2F a, Vector2F b)
            => a.Equal(b);
        public static bool operator !=(Vector2F a, Vector2F b)
            => !a.Equal(b);

        #endregion

        public bool Equal(Vector2F other)
            => X == other.X && Y == other.Y;

        public override bool Equals(object obj)
        {
            if (obj is Vector2F v)
                return Equal(v);

            return false;
        }

        public override int GetHashCode()
            => new { X, Y }.GetHashCode();

        public override string ToString()
            => $"X: {X}, Y: {Y}";
    }
}
