using GameEngine.Maths.Vectors;
using GameEngine.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public static class Converter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2D ToVector2D(this Physics.Vector2F v)
            => new Vector2D(v.X, v.Y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2I ToVector2I(this Physics.Vector2F v)
            => new Vector2I((int)v.X, (int)v.Y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2I ToVector2I(this Maths.Vectors.Vector2F v)
            => new Vector2I((int)v.X, (int)v.Y);
    }
}
