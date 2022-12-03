using GameEngine.Maths;
using GameEngine.Maths.Vectors;
using GameEngine.Utils;
using System;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Size = System.Drawing.Size;

namespace GameEngine
{
    public abstract class Engine : Application
    {
        protected EngineWindow GameWindow { get; private set; }

        protected EngineWindow[] otherWindows { get; private set; }

        public enum WindowToDraw : byte
        {
            GameWindow = 1,
            Window0 = 2,
            Window1 = 3,
            Window2 = 4,
            Window3 = 5,
            Window4 = 6,
            Window5 = 7,
        }

        public FpsCounter FpsCounter { get; protected set; }

        protected DateTime FrameStarted { get; set; }

        internal Ref<EngineWindow> WTD;

        /// <summary>
        /// Maximum FPS. Set -1 to unlimited. Default is 60
        /// </summary>
        protected int TargetFPS { get => targetFPS; set { targetFPS = value; targetElapsedTime = 1000d / (double)targetFPS; } }

        private int targetFPS;

        private double targetElapsedTime;

        public Engine()
        {
            
        }

        [STAThread]
        public void Run(Size windowSize, string windowTitle)
        {
            GameWindow = new EngineWindow(windowSize)
            {
                Title = windowTitle,
                ResizeMode = ResizeMode.CanMinimize,
            };

            System.Windows.Forms.Panel hostControl = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                BackColor = Color.Transparent,
                ForeColor = Color.Transparent,
            };

            void EnsureFocus(System.Windows.Forms.Control control)
            {
                if (!control.Focused)
                {
                    control.Focus();
                }
            }

            hostControl.MouseEnter += (sender, args) => EnsureFocus(hostControl);
            hostControl.MouseClick += (sender, args) => EnsureFocus(hostControl);

            System.Windows.Forms.Integration.WindowsFormsHost windowsFormsHost = new System.Windows.Forms.Integration.WindowsFormsHost
            {
                Child = hostControl,
            };

            GameWindow.Content = windowsFormsHost;

            GameWindow.Closed += (sender, args) => Environment.Exit(0);

            ShutdownMode = ShutdownMode.OnMainWindowClose;

            FpsCounter = new FpsCounter(new TimeSpan(0, 0, 0, 0, 500));

            GameWindow.Show();
            GameWindow.Init(hostControl);

            SetWindowToDraw(WindowToDraw.GameWindow);
            otherWindows = new EngineWindow[6];

            TargetFPS = 60;

            Initialize();

            double overflow = 0d;

            while (!Dispatcher.HasShutdownStarted)
            {
                FrameStarted = DateTime.UtcNow;
                FpsCounter.StartFrame();

                System.Windows.Forms.Application.DoEvents();

                try
                {
                    drawInternal();
                } catch (Exception e)
                {
                    Console.WriteLine($"Exception was thrown renderring a frame. Message: {e.ToString()}");
                }

                GameWindow.Render();

                for (int i = 0; i < otherWindows.Length; i++)
                {
                    if (otherWindows[i] != null && otherWindows[i].setUp)
                        otherWindows[i].Render();
                }

                if (targetFPS != -1)
                {
                    while ((DateTime.UtcNow - FrameStarted).TotalMilliseconds < targetElapsedTime - overflow) { }
                    overflow = (DateTime.UtcNow - FrameStarted).TotalMilliseconds - targetElapsedTime;
                }
                FpsCounter.StopFrame();

                if (Dispatcher.HasShutdownStarted)
                    break;
            }

            Font.FontLibrary.DisposeAll();

            GameWindow?.Dispose();

            for (int i = 0; i < otherWindows.Length; i++)
                otherWindows[i]?.Dispose();
        }

        public new void Exit()
        {
            Font.FontLibrary.DisposeAll();

            GameWindow?.Dispose();

            for (int i = 0; i < otherWindows.Length; i++)
                otherWindows[i]?.Dispose();
        }

        protected virtual void Initialize() { }

        protected abstract void drawInternal();

        protected void SetWindowToDraw(WindowToDraw windowToDraw)
        {
            if (windowToDraw == WindowToDraw.GameWindow && GameWindow != null && GameWindow.setUp)
                this.WTD = new Ref<EngineWindow>(() => GameWindow);
            else if (windowToDraw == WindowToDraw.Window0 && otherWindows[0] != null && otherWindows[0].setUp)
                this.WTD = new Ref<EngineWindow>(() => otherWindows[0]);
            else if (windowToDraw == WindowToDraw.Window1 && otherWindows[1] != null && otherWindows[1].setUp)
                this.WTD = new Ref<EngineWindow>(() => otherWindows[1]);
            else if (windowToDraw == WindowToDraw.Window2 && otherWindows[2] != null && otherWindows[2].setUp)
                this.WTD = new Ref<EngineWindow>(() => otherWindows[2]);
            else if (windowToDraw == WindowToDraw.Window3 && otherWindows[3] != null && otherWindows[3].setUp)
                this.WTD = new Ref<EngineWindow>(() => otherWindows[3]);
            else if (windowToDraw == WindowToDraw.Window4 && otherWindows[4] != null && otherWindows[4].setUp)
                this.WTD = new Ref<EngineWindow>(() => otherWindows[4]);
            else if (windowToDraw == WindowToDraw.Window5 && otherWindows[5] != null && otherWindows[5].setUp)
                this.WTD = new Ref<EngineWindow>(() => otherWindows[5]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">0 to 5 (0 <= index <= 5)</param>
        protected void InitializeWindow(byte index, Size windowSize, string windowTitle, bool allowTransparency = false)
        {
            if (index < 0 || index > 5)
                throw new IndexOutOfRangeException($"Index must be 0 <= index({index}) <= 5");

            EngineWindow window = new EngineWindow(windowSize)
            {
                Title = windowTitle,
                ResizeMode = ResizeMode.CanMinimize,
            };

            System.Windows.Forms.Panel hostControl = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                BackColor = Color.Transparent,
                ForeColor = Color.Transparent,
            };

            void EnsureFocus(System.Windows.Forms.Control control)
            {
                if (!control.Focused)
                {
                    control.Focus();
                }
            }

            hostControl.MouseEnter += (sender, args) => EnsureFocus(hostControl);
            hostControl.MouseClick += (sender, args) => EnsureFocus(hostControl);

            System.Windows.Forms.Integration.WindowsFormsHost windowsFormsHost = new System.Windows.Forms.Integration.WindowsFormsHost
            {
                Child = hostControl,
            };

            window.Content = windowsFormsHost;

            window.Closed += (sender, args) => Environment.Exit(0);

            otherWindows[index] = window;
            if (allowTransparency)
            {
                otherWindows[index].WindowStyle = WindowStyle.None;
                otherWindows[index].ResizeMode = ResizeMode.NoResize;
                otherWindows[index].AllowsTransparency = true;
            }

            window.Show();
            window.Init(hostControl);
        }

        protected void Clear(int color) => WTD.Value.Buffer.Clear(color);
        protected void Clear(Color color) => WTD.Value.Buffer.Clear(color);

        void Clip(ref int x, ref int y)
        {
            if (x < 0) x = 0;
            if (x > WTD.Value.Buffer.Width) x = WTD.Value.Buffer.Width;
            if (y < 0) y = 0;
            if (y > WTD.Value.Buffer.Height) y = WTD.Value.Buffer.Height;
        }

        protected void Draw(Vector2I pos, Color color)
            => Draw(pos.X, pos.Y, color);
        protected void Draw(Vector2I pos, int color)
            => Draw(pos.X, pos.Y, color);
        protected virtual void Draw(int x, int y, Color color)
        {
            if (x >= GameWindow.Width || x < 0 || y >= GameWindow.Height || y < 0 || color.A == 0)
                return;
            WTD.Value.Buffer.SetPixel(x, y, color);
        }
        protected virtual void Draw(int x, int y, int color)
        {
            if (x >= GameWindow.Buffer.Width || x < 0 || y >= GameWindow.Buffer.Height || y < 0)
                return;
            WTD.Value.Buffer.Write(x, y, color);
        }

        protected void DrawPoint(int x, int y, int size, Color color)
            => DrawPoint(x, y, size, color.ToArgb());
        protected void DrawPoint(int x, int y, int size, int color)
        {
            int s = size >> 1;
            Fill(x - s, y - s, x + s, y + s, color);
        }

        protected void Fill(Vector2I v1, Vector2I v2, Color color)
            => Fill(v1.X, v1.Y, v2.X, v2.Y, color.ToArgb());
        protected void Fill(Vector2I v1, Vector2I v2, int color)
            => Fill(v1.X, v1.Y, v2.X, v2.Y, color);
        protected void Fill(int x1, int y1, int x2, int y2, Color color)
            => Fill(x1, y1, x2, y2, color.ToArgb());
        protected void Fill(int x1, int y1, int x2, int y2, int color)
        {
            Clip(ref x1, ref y1);
            Clip(ref x2, ref y2);

            int length = x2 - x1;
            for (int x = x1; x < x2; x++)
                Draw(x, y1, color);
            for (int y = y1; y < y2; y++)
                WTD.Value.Buffer.Write(y * WTD.Value.Buffer.Width + x1, WTD.Value.Buffer.Data, y1 * WTD.Value.Buffer.Width + x1, length);
        }

        internal void FillInter(int x1, int y1, int x2, int y2, int color)
        {
            Clip(ref x1, ref y1);
            Clip(ref x2, ref y2);
            
            int length = x2 - x1;
            for (int x = x1; x < x2; x++)
                Draw(x, y1, color);
            for (int y = y1; y < y2; y++)
                WTD.Value.Buffer.Write(y * WTD.Value.Buffer.Width + x1, WTD.Value.Buffer.Data, y1 * WTD.Value.Buffer.Width + x1, length);

        }

        protected void DrawDb(Vector2I v1, DirectBitmap db)
            => DrawDb(v1.X, v1.Y, db);
        protected void DrawDb(int x1, int y1, DirectBitmap db)
        {
            int yOff = y1 < 0 ? y1 : 0;
            for (int y = yOff; y < db.Height && y + y1 < WTD.Value.Buffer.Height; y++)
            {
                WTD.Value.Buffer.Write((y + y1) * WTD.Value.Buffer.Width + x1, db.Data, y * db.Width, db.Width);
            }
        }

        protected void DrawDbP(Vector2I v1, Vector2I fromOff, DirectBitmap db)
            => DrawDbP(v1.X, v1.Y, fromOff.X, fromOff.Y, db);
        protected void DrawDbP(int x1, int y1, int fromXOff, int fromYOff, DirectBitmap db)
        {
            for (int x = fromXOff; x < db.Width; x++)
                for (int y = fromYOff; y < db.Height; y++)
                    WTD.Value.Buffer.Write((y + y1) * WTD.Value.Buffer.Width + x1 + x, db.Data[(y) * db.Width + x]);
        }

        protected void DrawDbPP(Vector2I v1, Vector2I fromOff, Vector2I sizeOff, DirectBitmap db)
            => DrawDbPP(v1.X, v1.Y, fromOff.X, fromOff.Y, sizeOff.X, sizeOff.Y, db);
        protected void DrawDbPP(int x1, int y1, int fromXOff, int fromYOff, int sXOff, int sYOff, DirectBitmap db)
        {
            for (int x = fromXOff; x < db.Width - sXOff; x++)
                for (int y = fromYOff; y < db.Height - sYOff; y++)
                    WTD.Value.Buffer.Write((y + y1) * WTD.Value.Buffer.Width + x1 + x, db.Data[(y) * db.Width + x]);
        }

        protected void DrawDBAdvenced(int x,int y, DirectBitmap db, uint scale, byte flip)
        {
            if (db == null)
                return;

            int fxs = 0, fxm = 1, fx = 0;
            int fys = 0, fym = 1, fy = 0;
            if ((flip & 0b_0000_0001) == 0b_0000_0001) { fxs = db.Width - 1; fxm = -1; } // horizontal
            if ((flip & 0b_0000_0010) == 0b_0000_0010) { fys = db.Height - 1; fym = -1; } // vertical

            if (scale > 1)
            {
                fx = fxs;
                for (int i = 0; i < db.Width; i++, fx += fxm)
                {
                    fy = fys;
                    for (int j = 0; j < db.Height; j++, fy += fym)
                        for (uint _is = 0; _is < scale; _is++)
                            for (uint js = 0; js < scale; js++)
                                Draw((int)(x + (i * scale) + _is), (int)(y + (j * scale) + js), db.Data[fx + fy * db.Width]);
                }
            }
            else
            {
                fx = fxs;
                for (int i = 0; i < db.Width; i++, fx += fxm)
                {
                    fy = fys;
                    for (int j = 0; j < db.Height; j++, fy += fym)
                        Draw(x + i, y + j, db.Data[fx + fy * db.Width]);
                }
            }
        }

        protected void DrawDBClipped(Vector2I pos, DirectBitmap db)
            => DrawDBClipped(pos.X, pos.Y, db);
        protected void DrawDBClipped(int x, int y, DirectBitmap db)
        {
            int rx = x;
            int ry = y;
            int rex = rx + db.Width;
            int rey = ry + db.Height;
            if (rx < 0 || ry < 0 || rex > WTD.Value.Buffer.Width || rey > WTD.Value.Buffer.Height) // Draw Partly
            {
                if (rx < db.Width - 1 || ry < db.Height - 1 || rx >= WTD.Value.Buffer.Width || ry >= WTD.Value.Buffer.Height) // No Draw
                    return;

                int xx = 0;
                if (rx < 0)
                    xx = Math.Abs(rx);

                int yy = 0;
                if (ry < db.Height)
                    yy = Math.Abs(db.Height - ry);

                int xxx = 0;
                if (rex > GameWindow.Buffer.Width)
                    xxx = Math.Abs(rex - WTD.Value.Buffer.Width);

                int yyy = 0;
                if (rey > GameWindow.Buffer.Height)
                    yyy = Math.Abs(rey - WTD.Value.Buffer.Height);

                DrawDbPP(rx, ry, xx, yy, xxx, yyy, db);
            }
            else
                DrawDb(rx, ry, db); // Full draw
        }


        #region alpha
        protected void DrawDbA(Vector2I v1, DirectBitmap db)
            => DrawDbA(v1.X, v1.Y, db);
        protected void DrawDbA(int x1, int y1, DirectBitmap db)
        {
            for (int x = 0; x < db.Width; x++)
                for (int y = 0; y < db.Height; y++)
                    if (db.Data[y * db.Width + x] != 0)
                        WTD.Value.Buffer.Write((y + y1) * WTD.Value.Buffer.Width + x1 + x, db.Data[y * db.Width + x]);
        }

        protected void DrawDbPPA(Vector2I v1, Vector2I fromOff, Vector2I sizeOff, DirectBitmap db)
            => DrawDbPPA(v1.X, v1.Y, fromOff.X, fromOff.Y, sizeOff.X, sizeOff.Y, db);
        protected void DrawDbPPA(int x1, int y1, int fromXOff, int fromYOff, int sXOff, int sYOff, DirectBitmap db)
        {
            for (int x = fromXOff; x < db.Width - sXOff; x++)
                for (int y = fromYOff; y < db.Height - sYOff; y++)
                    if (db.Data[y * db.Width + x] != 0)
                        WTD.Value.Buffer.Write((y + y1) * WTD.Value.Buffer.Width + x1 + x, db.Data[y * db.Width + x]);
        }

        protected void DrawDBClippedA(Vector2I pos, DirectBitmap db)
           => DrawDBClippedA(pos.X, pos.Y, db);
        protected void DrawDBClippedA(int x, int y, DirectBitmap db)
        {
            int rx = x;
            int ry = y;
            int rex = rx + db.Width;
            int rey = ry + db.Height;
            if (rx < 0 || ry < 0 || rex > WTD.Value.Buffer.Width || rey > WTD.Value.Buffer.Height) // Draw Partly
            {
                if (rx < -(db.Width - 1) || ry < -(db.Height - 1) || rx >= WTD.Value.Buffer.Width || ry >= WTD.Value.Buffer.Height) // No Draw
                    return;

                int xx = 0;
                if (rx < 0)
                    xx = Math.Abs(rx);

                int yy = 0;
                if (ry < 0)
                    yy = Math.Abs(ry);

                int xxx = 0;
                if (rex > GameWindow.Buffer.Width)
                    xxx = Math.Abs(rex - WTD.Value.Buffer.Width);

                int yyy = 0;
                if (rey > GameWindow.Buffer.Height)
                    yyy = Math.Abs(rey - WTD.Value.Buffer.Height);

                DrawDbPPA(rx, ry, xx, yy, xxx, yyy, db);
            }
            else
                DrawDbA(rx, ry, db); // Full draw
        }
        #endregion

        internal void DrawDbAInter(int x1, int y1, DirectBitmap db)
        {
            for (int x = 0; x < db.Width; x++)
                for (int y = 0; y < db.Height; y++)
                    if (x1 + x >= 0 && y1 + y >= 0 && x1 + x < WTD.Value.Buffer.Width && y1 + y < WTD.Value.Buffer.Height &&
                        db.Data[y * db.Width + x] != 0)
                        WTD.Value.Buffer.Write((y + y1) * WTD.Value.Buffer.Width + x1 + x, db.Data[y * db.Width + x]);
        }


        protected void DrawString(int x, int y, string s, Font.Font f, int size, Color color, int xOffset = 2, Font.RenderType rt = Font.RenderType.UpToDown)
            => Font.FontRender.Render(f, WTD.Value.Buffer, s, new Vector2I(x, y), size, color.ToArgb(), xOffset, rt);
        protected void DrawString(int x, int y, string s, Font.Font f, int size, int color, int xOffset = 2, Font.RenderType rt = Font.RenderType.UpToDown)
            => Font.FontRender.Render(f, WTD.Value.Buffer, s, new Vector2I(x, y), size, color, xOffset, rt);

        protected void DrawLine(Vector2D v1, Vector2D v2, Color color)
            => DrawLine(WTD.Value.RelativeToPixel(v1), WTD.Value.RelativeToPixel(v2), color.ToArgb());
        protected void DrawLine(Vector2D v1, Vector2D v2, int color)
            => DrawLine(WTD.Value.RelativeToPixel(v1), WTD.Value.RelativeToPixel(v2), color);
        protected void DrawLine(double x1, double y1, double x2, double y2, Color color)
            => DrawLine(x1, y1, x2, y2, color.ToArgb());
        protected void DrawLine(double x1, double y1, double x2, double y2, int color)
            => DrawLine(WTD.Value.RelativeToPixelX(x1), WTD.Value.RelativeToPixelY(y1),
                WTD.Value.RelativeToPixelX(x2), WTD.Value.RelativeToPixelY(y2), color);
        protected void DrawLine(Vector2I v1, Vector2I v2, Color color)
            => DrawLine(v1.X, v1.Y, v2.X, v2.Y, color.ToArgb());
        protected void DrawLine(Vector2I v1, Vector2I v2, int color)
            => DrawLine(v1.X, v1.Y, v2.X, v2.Y, color);
        protected void DrawLine(int x1, int y1, int x2, int y2, Color color)
            => DrawLine(x1, y1, x2, y2, color.ToArgb());
        protected void DrawLine(int x1, int y1, int x2, int y2, int color)
        {
            int x, y, dx, dy, dx1, dy1, px, py, xe, ye, i;
            dx = x2 - x1; dy = y2 - y1;
            dx1 = Math.Abs(dx); dy1 = Math.Abs(dy);
            px = 2 * dy1 - dx1; py = 2 * dx1 - dy1;
            if (dy1 <= dx1)
            {
                if (dx >= 0)
                { x = x1; y = y1; xe = x2; }
                else
                { x = x2; y = y2; xe = x1; }

                Draw(x, y, color);

                for (i = 0; x < xe; i++)
                {
                    x = x + 1;
                    if (px < 0)
                        px = px + 2 * dy1;
                    else
                    {
                        if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0)) y = y + 1; else y = y - 1;
                        px = px + 2 * (dy1 - dx1);
                    }
                    Draw(x, y, color);
                }
            }
            else
            {
                if (dy >= 0)
                { x = x1; y = y1; ye = y2; }
                else
                { x = x2; y = y2; ye = y1; }

                Draw(x, y, color);

                for (i = 0; y < ye; i++)
                {
                    y = y + 1;
                    if (py <= 0)
                        py = py + 2 * dx1;
                    else
                    {
                        if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0)) x = x + 1; else x = x - 1;
                        py = py + 2 * (dx1 - dy1);
                    }
                    Draw(x, y, color);
                }
            }
        }

        protected void DrawCircle(Vector2D pos, Int32 radius, Color color)
            => DrawCircle(WTD.Value.RelativeToPixel(pos), radius, color.ToArgb());
        protected void DrawCircle(Vector2D pos, Int32 radius, int color)
            => DrawCircle(WTD.Value.RelativeToPixel(pos), radius, color);
        protected void DrawCircle(double x, double y, Int32 radius, Color color)
            => DrawCircle(WTD.Value.RelativeToPixelX(x), WTD.Value.RelativeToPixelY(y), radius, color.ToArgb());
        protected void DrawCircle(double x, double y, Int32 radius, int color)
            => DrawCircle(WTD.Value.RelativeToPixelX(x), WTD.Value.RelativeToPixelY(y), radius, color);
        protected void DrawCircle(Vector2I pos, Int32 radius, Color color)
            => DrawCircle(pos.X, pos.Y, radius, color.ToArgb());
        protected void DrawCircle(Vector2I pos, Int32 radius, int color)
            => DrawCircle(pos.X, pos.Y, radius, color);
        protected void DrawCircle(Int32 x, Int32 y, Int32 radius,  Color color)
            => DrawCircle(x, y, radius, color.ToArgb());
        protected void DrawCircle(Int32 xp, Int32 yp, Int32 radius, int color)
        {
            int off = MathPlus.RoundToInt(radius / 2.35);
            DrawLine(-off + xp, radius + yp, off + xp, radius + yp, color);  //  --
            DrawLine(off + xp, radius + yp, radius + xp, off + yp, color);   //    \
            DrawLine(radius + xp, off + yp, radius + xp, -off + yp, color);  //     |
            DrawLine(radius + xp, -off + yp, off + xp, -radius + yp, color); //    /
            DrawLine(-off + xp, -radius + yp, off + xp, -radius + yp, color);  //  --

            DrawLine(-radius + xp, -off + yp, -off + xp, -radius + yp, color); //    /
            DrawLine(-radius + xp, off + yp, -radius + xp, -off + yp, color);  //     |
            DrawLine(-off + xp, radius + yp, -radius + xp, off + yp, color);   //    \
            /*int x = 0, y = radius;
            int d = 3 - 2 * radius;
            drawCircle(xp, yp, x, y, color);
            while (y >= x)
            {
                x++;

                if (d > 0)
                {
                    y--;
                    d = d + 4 * (x - y) + 10;
                }
                else
                    d = d + 4 * x + 6;
                drawCircle(xp, yp, x, y, color);
            }*/
        }

        protected void FillCircle(Vector2I pos, Int32 radius, Color color)
            => FillCircle(pos.X, pos.Y, radius, color.ToArgb());
        protected void FillCircle(Vector2I pos, Int32 radius, int color)
            => FillCircle(pos.X, pos.Y, radius, color);
        protected void FillCircle(Int32 x, Int32 y, Int32 radius, Color color)
            => FillCircle(x, y, radius, color.ToArgb());
        protected void FillCircle(Int32 x, Int32 y, Int32 radius, int color)
        { // Thanks to IanM-Matrix1 #PR121
            if (radius < 0 || x < -radius || y < -radius || x - GameWindow.Width > radius || y - GameWindow.Height > radius)
                return;

            if (radius > 0)
            {
                int x0 = 0;
                int y0 = radius;
                int d = 3 - 2 * radius;

                while (y0 >= x0)
                {
                    drawline(x - y0, x + y0, y - x0, color);
                    if (x0 > 0) drawline(x - y0, x + y0, y + x0, color);

                    if (d < 0)
                        d += 4 * x0++ + 6;
                    else
                    {
                        if (x0 != y0)
                        {
                            drawline(x - x0, x + x0, y - y0, color);
                            drawline(x - x0, x + x0, y + y0, color);
                        }
                        d += 4 * (x0++ - y0--) + 10;
                    }
                }
            }
            else
                Draw(x, y, color);
        }

        protected void DrawRect(Vector2I pos, Vector2I size, Color color)
            => DrawRect(pos.X, pos.Y, size.X, size.Y, color.ToArgb());
        protected void DrawRect(Vector2I pos, Vector2I size, int color)
            => DrawRect(pos.X, pos.Y, size.X, size.Y, color);
        protected void DrawRect(Int32 x, Int32 y, Int32 w, Int32 h, Color color)
            => DrawRect(x, y, w, h, color.ToArgb());
        protected void DrawRect(Int32 x, Int32 y, Int32 w, Int32 h, int color)
        {
            DrawLine(x, y, x + w, y, color);
            DrawLine(x + w, y, x + w, y + h, color);
            DrawLine(x + w, y + h, x, y + h, color);
            DrawLine(x, y + h, x, y, color);
        }

        internal void DrawRectInter(Int32 x, Int32 y, Int32 w, Int32 h, int color)
        {
            DrawLine(x, y, x + w, y, color);
            DrawLine(x + w, y, x + w, y + h, color);
            DrawLine(x + w, y + h, x, y + h, color);
            DrawLine(x, y + h, x, y, color);
        }

        protected void FillRect(Vector2I pos, Vector2I size, Color color)
            => FillRect(pos.X, pos.Y, size.X, size.Y, color.ToArgb());
        protected void FillRect(Vector2I pos, Vector2I size, int color)
            => FillRect(pos.X, pos.Y, size.X, size.Y, color);
        protected void FillRect(Int32 x, Int32 y, Int32 w, Int32 h, Color color)
            => FillRect(x, y, w, h, color.ToArgb());
        protected void FillRect(Int32 x, Int32 y, Int32 w, Int32 h, int color)
        {
            Int32 x2 = x + w;
            Int32 y2 = y + h;

            if (x < 0) x = 0;
            if (x >= (Int32)GameWindow.Width) x = (Int32)GameWindow.Width;
            if (y < 0) y = 0;
            if (y >= (Int32)GameWindow.Height) y = (Int32)GameWindow.Height;

            if (x2 < 0) x2 = 0;
            if (x2 >= (Int32)GameWindow.Width) x2 = (Int32)GameWindow.Width;
            if (y2 < 0) y2 = 0;
            if (y2 >= (Int32)GameWindow.Height) y2 = (Int32)GameWindow.Height;

            for (int i = x; i < x2; i++)
                for (int j = y; j < y2; j++)
                    Draw(i, j, color);
        }

        protected void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Color color)
            => DrawTriangle(x1, y1, x2, y2, x3, y3, color.ToArgb());
        protected void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, int color)
        {
            DrawLine(x1, y1, x2, y2, color);
            DrawLine(x2, y2, x3, y3, color);
            DrawLine(x3, y3, x1, y1, color);
        }

        void SWAP(ref int x, ref int y)
        {
            int t = x; x = y; y = t;
        }
        void drawline(int sx, int ex, int ny, int color)
        {
            for (int i = sx; i <= ex; i++) Draw(i, ny, color);
        }
        protected void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Color color)
            => FillTriangle(x1, y1, x2, y2, x3, y3, color.ToArgb());
        protected void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3, int color)
        {
            int t1x, t2x, y, minx, maxx, t1xp, t2xp;
            bool changed1 = false;
            bool changed2 = false;
            int signx1, signx2, dx1, dy1, dx2, dy2;
            int e1, e2;
            // Sort vertices
            if (y1 > y2) { SWAP(ref y1, ref y2); SWAP(ref x1, ref x2); }
            if (y1 > y3) { SWAP(ref y1, ref y3); SWAP(ref x1, ref x3); }
            if (y2 > y3) { SWAP(ref y2, ref y3); SWAP(ref x2, ref x3); }

            t1x = t2x = x1; y = y1;   // Starting points
            dx1 = (int)(x2 - x1); if (dx1 < 0) { dx1 = -dx1; signx1 = -1; }
            else signx1 = 1;
            dy1 = (int)(y2 - y1);

            dx2 = (int)(x3 - x1); if (dx2 < 0) { dx2 = -dx2; signx2 = -1; }
            else signx2 = 1;
            dy2 = (int)(y3 - y1);

            if (dy1 > dx1)
            {   // swap values
                SWAP(ref dx1, ref dy1);
                changed1 = true;
            }
            if (dy2 > dx2)
            {   // swap values
                SWAP(ref dy2, ref dx2);
                changed2 = true;
            }

            e2 = (int)(dx2 >> 1);
            // Flat top, just process the second half
            if (y1 == y2) goto next;
            e1 = (int)(dx1 >> 1);

            for (int i = 0; i < dx1;)
            {
                t1xp = 0; t2xp = 0;
                if (t1x < t2x) { minx = t1x; maxx = t2x; }
                else { minx = t2x; maxx = t1x; }
                // process first line until y value is about to change
                while (i < dx1)
                {
                    i++;
                    e1 += dy1;
                    while (e1 >= dx1)
                    {
                        e1 -= dx1;
                        if (changed1) t1xp = signx1;//t1x += signx1;
                        else goto next1;
                    }
                    if (changed1) break;
                    else t1x += signx1;
                }
            // Move line
            next1:
                // process second line until y value is about to change
                while (true)
                {
                    e2 += dy2;
                    while (e2 >= dx2)
                    {
                        e2 -= dx2;
                        if (changed2) t2xp = signx2;//t2x += signx2;
                        else goto next2;
                    }
                    if (changed2) break;
                    else t2x += signx2;
                }
            next2:
                if (minx > t1x) minx = t1x; if (minx > t2x) minx = t2x;
                if (maxx < t1x) maxx = t1x; if (maxx < t2x) maxx = t2x;
                drawline(minx, maxx, y, color);    // Draw line from min to max points found on the y
                                               // Now increase y
                if (!changed1) t1x += signx1;
                t1x += t1xp;
                if (!changed2) t2x += signx2;
                t2x += t2xp;
                y += 1;
                if (y == y2) break;

            }
        next:
            // Second half
            dx1 = (int)(x3 - x2); if (dx1 < 0) { dx1 = -dx1; signx1 = -1; }
            else signx1 = 1;
            dy1 = (int)(y3 - y2);
            t1x = x2;

            if (dy1 > dx1)
            {   // swap values
                SWAP(ref dy1, ref dx1);
                changed1 = true;
            }
            else changed1 = false;

            e1 = (int)(dx1 >> 1);

            for (int i = 0; i <= dx1; i++)
            {
                t1xp = 0; t2xp = 0;
                if (t1x < t2x) { minx = t1x; maxx = t2x; }
                else { minx = t2x; maxx = t1x; }
                // process first line until y value is about to change
                while (i < dx1)
                {
                    e1 += dy1;
                    while (e1 >= dx1)
                    {
                        e1 -= dx1;
                        if (changed1) { t1xp = signx1; break; }//t1x += signx1;
                        else goto next3;
                    }
                    if (changed1) break;
                    else t1x += signx1;
                    if (i < dx1) i++;
                }
            next3:
                // process second line until y value is about to change
                while (t2x != x3)
                {
                    e2 += dy2;
                    while (e2 >= dx2)
                    {
                        e2 -= dx2;
                        if (changed2) t2xp = signx2;
                        else goto next4;
                    }
                    if (changed2) break;
                    else t2x += signx2;
                }
            next4:

                if (minx > t1x) minx = t1x; if (minx > t2x) minx = t2x;
                if (maxx < t1x) maxx = t1x; if (maxx < t2x) maxx = t2x;
                drawline(minx, maxx, y, color);
                if (!changed1) t1x += signx1;
                t1x += t1xp;
                if (!changed2) t2x += signx2;
                t2x += t2xp;
                y += 1;
                if (y > y3) return;
            }
        }
    }
}
