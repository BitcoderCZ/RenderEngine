using GameEngine.Font;
using System.Drawing;

namespace GameEngine.UI
{
    public class UIText : UIElement
    {
        public string Text;

        public int Color;

        public int Size;

        public UIText(string text, int x, int y, int color, int size = 32)
        {
            Text = text;
            X = x;
            Y = y;
            Color = color;
            Size = size;
            Width = -1;
            Height = -1;
        }

        public override void Draw(Engine engine, UIManager manager)
        {
            manager.RenderAndCaschText(Text, Size, Color, X, Y);
        }

        public void CenterX(Engine engine, UIManager manager, int xOff)
        {
            Size textSize = FontRender.GetTextSize(manager.CurrentFont, Text, Size);
            X = engine.WTD.Value.Buffer.Width / 2 - textSize.Width / 2 + xOff;
        }
    }
}
