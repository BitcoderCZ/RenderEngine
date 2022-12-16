using GameEngine.Inputs;
using System;
using System.Drawing;

namespace GameEngine.UI
{
    public class UIButton : UIElement
    {
        public string Text;
        public Action<UIManager, MouseButtons> OnClick;
        public int PadTop;
        public int PadBottom;
        public int PadLeft;
        public int PadRight;
        public int TextSize;

        public UIButton(string text, Action<UIManager, MouseButtons> onClick, int x, int y, int width, int height,
            int padTop, int padBottom, int padLeft, int padRight, int textSize)
        {
            Text = text;
            OnClick = onClick;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            PadTop = padTop;
            PadBottom = padBottom;
            PadLeft = padLeft;
            PadRight = padRight;
            TextSize = textSize;
        }

        public override void Draw(Engine engine, UIManager manager)
        {
            engine.FillInter(X, Y, X + Width, Y + Height, Color.White.ToArgb());
            engine.DrawRectInter(X, Y, Width, Height, Color.Black.ToArgb());
            engine.DrawRectInter(X + 1, Y + 1, Width - 2, Height - 2, Color.Black.ToArgb());
            engine.DrawRectInter(X + 2, Y + 2, Width - 4, Height - 4, Color.Black.ToArgb());
            manager.RenderAndCaschText(Text, TextSize, Color.Black.ToArgb(), X + PadLeft, Y + PadTop);
        }

        public override void OnMouseDown(UIManager manager, MouseButtons btn, bool onElement) 
        {
            if (onElement)
                OnClick.Invoke(manager, btn); 
        }
    }
}
