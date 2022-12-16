using GameEngine.Font;
using GameEngine.Inputs;
using GameEngine.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Font_ = GameEngine.Font.Font;

namespace GameEngine.UI
{
    public class UIManager
    {
        public List<(uint id, IUIElement element)> elements { get; private set; }

        public Random rng { get; private set; }

        private Dictionary<CaschedTextInfo, DirectBitmap> cashedText;

        private Engine engine;

        private List<Font_> fonts;

        public int selectedFontIndex;

        public Font_ CurrentFont => fonts[selectedFontIndex];


        public int CaschedTextCount => cashedText.Count;


        public UIManager(Engine _engine, params Font_[] _fonts)
        {
            engine = _engine;
            if (_fonts == null)
                fonts = new List<Font_>();
            else
                fonts = _fonts.ToList();
            selectedFontIndex = 0;
            cashedText = new Dictionary<CaschedTextInfo, DirectBitmap>();
            elements = new List<(uint id, IUIElement element)>();
            rng = new Random(DateTime.Now.Millisecond * DateTime.Now.Second);

            engine.WTD.Value.Input.KeyDown += OnKeyDown;
            engine.WTD.Value.Input.KeyUp += OnKeyUp;
            engine.WTD.Value.Input.MouseDown += OnMouseDown;
            engine.WTD.Value.Input.MouseUp += OnMouseUp;
        }

        private void OnKeyDown(object sender, IKeyEventArgs args)
        {
            if (args.Key != Key.None)
                for (int i = 0; i < elements.Count; i++)
                    elements[i].element.OnKeyDown(this, args.Key, args.Modifiers);
        }

        private void OnKeyUp(object sender, IKeyEventArgs args)
        {
            if (args.Key != Key.None)
                for (int i = 0; i < elements.Count; i++)
                    elements[i].element.OnKeyUp(this, args.Key, args.Modifiers);
        }

        private void OnMouseDown(object sender, IMouseEventArgs args)
        {
            int x = (int)args.Position.X;
            int y = (int)args.Position.Y;

            if (args.Buttons != MouseButtons.None)
                for (int i = 0; i < elements.Count; i++)
                    elements[i].element.OnMouseDown(this, args.Buttons, Inbounds(elements[i].element, x, y));
        }

        private void OnMouseUp(object sender, IMouseEventArgs args)
        {
            int x = (int)args.Position.X;
            int y = (int)args.Position.Y;

            if (args.Buttons != MouseButtons.None)
                for (int i = 0; i < elements.Count; i++)
                    elements[i].element.OnMouseUp(this, args.Buttons, Inbounds(elements[i].element, x, y));
        }

        private bool Inbounds(IUIElement b, int x, int y)
        {
            return x >= b.X && x < b.X + b.Width && y >= b.Y && y < b.Y + b.Height;
        }

        public void Draw()
        {
            for (int i = 0; i < elements.Count; i++)
                if (elements[i].element.Active)
                    elements[i].element.Draw(engine, this);
        }

        public void Update(double deltaTime)
        {
            for (int i = 0; i < elements.Count; i++)
                elements[i].element.Update(engine, this, deltaTime);
        }

        public void ClearUI() => elements.Clear();

        public void ClearCaschedText()
        {
            foreach (KeyValuePair<CaschedTextInfo, DirectBitmap> item in cashedText)
                item.Value.Dispose();

            cashedText.Clear();
        }

        private uint NextValidId()
        {
            byte[] bytes = new byte[4];
            rng.NextBytes(bytes);
            uint id = BitConverter.ToUInt32(bytes, 0);
            for (int i = 0; i < elements.Count; i++)
                if (elements[i].id == id)
                    return NextValidId();

            return id;
        }

        public uint AddElement(IUIElement _element)
        {
            uint id = NextValidId();
            elements.Add((id, _element));
            return id;
        }

        public IUIElement GetElement(uint id)
        {
            for (int i = 0; i < elements.Count; i++)
                if (elements[i].id == id)
                    return elements[i].element;

            return null;
        }

        public void RemoveElement(uint id)
        {
            for (int i = 0; i < elements.Count; i++)
                if (elements[i].id == id)
                {
                    elements.RemoveAt(i);
                    return;
                }
        }

        public uint CreateText(string text, int x, int y, int size, int color)
        {
            return AddElement(new UIText(text, x, y, color, size));
        }

        public uint CreateTextCenterX(string text, int xOff, int y, int size, int color)
        {
            Size textSize = FontRender.GetTextSize(fonts[selectedFontIndex], text, size);
            return AddElement(new UIText(text, engine.WTD.Value.Buffer.Width / 2 - textSize.Width / 2 + xOff, y, color, size));
        }

        public uint CreateBtn(string text, int x, int y, int size, int padTop, int padBottom, int padLeft, int padRight, Action<UIManager, MouseButtons> onEvent)
        {
            Size textSize = FontRender.GetTextSize(fonts[selectedFontIndex], text, size);
            return AddElement(new UIButton(text, onEvent, x, y, textSize.Width + padLeft + padRight, textSize.Height + padTop + padBottom,
                padTop, padBottom, padLeft, padRight, size));
        }

        public uint CreateBtnCenterX(string text, int xOff, int y, int size, int padTop, int padBottom, int padLeft, int padRight, Action<UIManager, MouseButtons> onEvent)
        {
            Size textSize = FontRender.GetTextSize(fonts[selectedFontIndex], text, size);
            return AddElement(new UIButton(text, onEvent, engine.WTD.Value.Buffer.Width / 2 - textSize.Width / 2 + xOff, y, textSize.Width + padLeft + padRight, textSize.Height + padTop + padBottom,
                padTop, padBottom, padLeft, padRight, size));
        }

        public uint CreateInputField(int x, int y, int width, int height, int size, int maxVisible, int maxLength = 50, string defaultText = "Type...")
        {
            return AddElement(new UIInputField(x, y, width, height, size, maxVisible, maxLength, defaultText));
        }

        public uint CreateInputFieldCenterX(int xOff, int y, int width, int height, int size, int maxVisible, int maxLength = 50, string defaultText = "Type...")
        {
            return AddElement(new UIInputField(engine.WTD.Value.Buffer.Width / 2 - width / 2 + xOff, y, width, height, size, maxVisible, maxLength, defaultText));
        }

        public bool TryGetCaschedText(string text, int size, int color, out DirectBitmap renderedText)
        {
            if (cashedText.ContainsKey(new CaschedTextInfo(size, text, color, fonts[selectedFontIndex].Id)))
            {
                renderedText = cashedText[new CaschedTextInfo(size, text, color, fonts[selectedFontIndex].Id)];
                return true;
            } else {
                renderedText = null;
                return false;
            }
        }

        public void RenderAndCaschText(string text, int size, int color, int x, int y)
        {
            if (TryGetCaschedText(text, size, color, out DirectBitmap db))
                engine.DrawDbAInter(x, y, db);
            else
            {
                DirectBitmap rendered = new DirectBitmap(FontRender.GetTextSize(fonts[selectedFontIndex], text, size));
                rendered.Clear(Color.FromArgb(0, 0, 0, 0));
                FontRender.Render(fonts[selectedFontIndex], rendered, text, 0, 0, size, color);
                cashedText.Add(new CaschedTextInfo(size, text, color, fonts[selectedFontIndex].Id), rendered);
                engine.DrawDbAInter(x, y, rendered);
            }
        }
    }

    struct CaschedTextInfo
    {
        public int Size;
        public string Text;
        public int Color;
        public int FontId;

        public CaschedTextInfo(int size, string text, int color, int fontId)
        {
            Size = size;
            Text = text;
            Color = color;
            FontId = fontId;
        }

        public static bool operator ==(CaschedTextInfo a, CaschedTextInfo b) => a.Size == b.Size && a.Text == b.Text && a.Color == b.Color && a.FontId == b.FontId;
        public static bool operator !=(CaschedTextInfo a, CaschedTextInfo b) => a.Size != b.Size || a.Text != b.Text || a.Color != b.Color || a.FontId != b.FontId;

        public override bool Equals(object obj)
        {
            if (obj is CaschedTextInfo b)
                return this == b;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return Size ^ Color ^ FontId ^Text.GetHashCode();
        }
    }

    public delegate void UIIFDelegate(string typed);
}
