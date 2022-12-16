using GameEngine.Maths.Vectors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace GameEngine.Utils
{
    public static class U
    {
        public static int EnvironmentProcessorCount { get; } = Environment.ProcessorCount;

        public static ParallelOptions ParallelOptionsDefault =
            new ParallelOptions { MaxDegreeOfParallelism = EnvironmentProcessorCount };

        public static T Cloned<T>(this T cloneable) where T : ICloneable
            => (T)cloneable.Clone();

        public static void Fill<T>(this T[] array, T value)
        {
            var length = array.Length;
            if (length == 0) return;

            int seed = Math.Min(32, array.Length);
            for (var i = 0; i < seed; i++)
                array[i] = value;

            int count;
            for (count = seed; count <= length / 2; count *= 2)
                Array.Copy(array, 0, array, count, count);

            int leftover = length - count;
            if (leftover > 0)
                Array.Copy(array, 0, array, count, leftover);
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            T[] array = collection.ToArray();
            for (int i = 0; i < array.Length; i++)
                action.Invoke(array[i]);
        }

        public static IntPtr Handle(this System.Windows.Forms.Control window)
            => window.IsDisposed ? default : Handle((System.Windows.Forms.IWin32Window)window);

        public static IntPtr Handle(this System.Windows.Forms.IWin32Window window) => window.Handle;

        [STAThread]
        public static IntPtr Handle(this System.Windows.Media.Visual window)
        {
            System.Windows.Interop.HwndSource handleSource = window.HandleSource();
            return handleSource == null || handleSource.IsDisposed ? default : handleSource.Handle;
        }

        [STAThread]
        public static System.Windows.Interop.HwndSource HandleSource(this System.Windows.Media.Visual window)
            => System.Windows.PresentationSource.FromVisual(window) as System.Windows.Interop.HwndSource;

        public static void Swap<T>(ref T value0, ref T value1)
        {
            var temp = value0;
            value0 = value1;
            value1 = temp;
        }

        public static int ToRgba(this Color color)
        {
            return ((((color.A << 8) + color.B) << 8) + color.G << 8) + color.R;
        }

        public static Color FromRgbaToColor(this int color)
        {
            return Color.FromArgb
            (
                (color >> 24) & 0xFF,
                (color >> 0) & 0xFF,
                (color >> 8) & 0xFF,
                (color >> 16) & 0xFF
            );
        }

        #region conversions

        public static Vector2F ToVector2F(this Point p) => new Vector2F(p.X, p.Y);
        public static Vector2F ToVector2F(this System.Windows.Point p) => new Vector2F((float)p.X, (float)p.Y);
        public static Vector2D ToVector2D(this Point p) => new Vector2D(p.X, p.Y);
        public static Vector2D ToVector2D(this System.Windows.Point p) => new Vector2D(p.X, p.Y);

        #endregion
    }
}
