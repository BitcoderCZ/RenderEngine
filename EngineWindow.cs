#define ASINCDRAW

using GameEngine.Inputs;
using GameEngine.Maths;
using GameEngine.Maths.Vectors;
using GameEngine.Utils;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace GameEngine
{
    public sealed class EngineWindow : Window, IDisposable
    {
        public new int Width
        {
            get => (int)base.Width; 
            set
            {
                base.Width = value;
                SetRes();
            }
        }
        public new int Height
        {
            get => (int)base.Height; 
            set
            {
                base.Height = value;
                SetRes();
            }
        }

        public double resolution { get; private set; }

        public Vector2I position 
        {
            get => new Vector2I(MathPlus.RoundToInt(Left), MathPlus.RoundToInt(Top));
            set { Left = value.X; Top = value.Y; }
        }

        public Vector2D positionD
        {
            get => new Vector2D(Left, Top);
            set { Left = value.X; Top = value.Y; }
        }

        public IntPtr HostHandle { get; private set; }

        public Size HostSize { get; private set; }

        private Size BufferSize { get; set; }

        public Graphics GraphicsHost { get; private set; }

        public IntPtr GraphicsHostDeviceContext { get; set; }

        public BufferedGraphics BufferedGraphics { get; set; }

        public DirectBitmap Buffer;
        private DirectBitmap FronterBuffer;

        bool drawing = false;

        private TaskFactory factory;

        public EngineInput Input;

        public bool setUp { get; private set; } = false;

        public EngineWindow(Size windowSize)
        {
            Width = windowSize.Width;
            Height = windowSize.Height;

            SetRes();
        }

        [STAThread]
        public void Init(System.Windows.Forms.Panel hostControl)
        {
            if (setUp)
                return;

            HostHandle = hostControl.Handle();
            Input = new EngineInput(hostControl);

            HostSize = Input.Size;
            BufferSize = Input.Size;

            GraphicsHost = Graphics.FromHwnd(HostHandle);
            GraphicsHostDeviceContext = GraphicsHost.GetHdc();
            CreateSurface(Input.Size);
            CreateBuffers(BufferSize);

            factory = new TaskFactory();

            setUp = true;
        }

        [STAThread]
        public void ReInit(int width = -1, int height = -1)
        {
            if (width == -1 || height == -1) {
                HostSize = Input.Size;
                BufferSize = Input.Size;
            } else {
                Size s = new Size(width, height);
                HostSize = s;
                BufferSize = s;
            }

            GraphicsHost = Graphics.FromHwnd(HostHandle);
            GraphicsHostDeviceContext = GraphicsHost.GetHdc();
            CreateSurface(Input.Size);
            CreateBuffers(BufferSize);
        }

        private void CreateSurface(Size size)
        {
            BufferedGraphics = BufferedGraphicsManager.Current.Allocate(GraphicsHostDeviceContext, new Rectangle(Point.Empty, size));
            BufferedGraphics.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        }

        private void CreateBuffers(Size size)
        {
            Buffer = new DirectBitmap(size);
            FronterBuffer = new DirectBitmap(size);
        }

        public void Render()
        {
            if (!drawing)
            {
                drawing = true;
                Buffer.CopyTo(FronterBuffer);
#if ASINCDRAW
                factory.StartNew(drawToScreen);
#else
                drawToScreen();
#endif
            }
        }

        private void drawToScreen()
        {
            BufferedGraphics?.Graphics.DrawImage(
                FronterBuffer.Bitmap,
                new RectangleF(PointF.Empty, HostSize),
                new RectangleF(new PointF(-0.5f, -0.5f), BufferSize),
                GraphicsUnit.Pixel);

            // swap buffers
            BufferedGraphics?.Render(GraphicsHostDeviceContext);
            drawing = false;
        }

        private void SetRes() => resolution = (double)Width / (double)Height;

        public Vector2I RelativeToPixel(Vector2D value)
        {
            value.Y = -value.Y;
            value.Y *= resolution;
            value++;
            value /= 2d;
            value.X *= (double)Width;
            value.Y *= (double)Height;
            return (Vector2I)value;
        }

        public int RelativeToPixelX(double value)
        {
            value++;
            value /= 2;
            value *= (double)Width;
            return MathPlus.RoundToInt(value);
        }

        public int RelativeToPixelY(double value)
        {
            value *= resolution;
            value++;
            value /= 2;
            value *= (double)Height;
            return MathPlus.RoundToInt(value);
        }

        public void Dispose()
        {
            Input?.Dispose();
            Input = default;
            HostHandle = default;
            BufferedGraphics?.Dispose();
            BufferedGraphics = default;
            GraphicsHost?.Dispose();
            GraphicsHost = default;
            FronterBuffer?.Dispose();
            FronterBuffer = default;
            Buffer?.Dispose();
            Buffer = default;
            factory = default;
        }
    }
}
