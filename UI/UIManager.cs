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
using Clipboard = System.Windows.Forms.Clipboard;

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
            rng = new Random(DateTime.Now.Millisecond * 60 + DateTime.Now.Second);

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
            Size textSize = FontRender.GetTextSize(fonts[selectedFontIndex], text, size);
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

    public interface IUIElement
    {
        int X { get; set; }
        int Y { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        bool Active { get; set; }

        void Draw(Engine engine, UIManager manager);
        void Update(Engine engine, UIManager manager, double deltaTime);

        void OnKeyDown(UIManager manager, Key key, Modifiers modifiers);
        void OnKeyUp(UIManager manager, Key key, Modifiers modifiers);

        void OnMouseDown(UIManager manager, MouseButtons btn, bool onElement);
        void OnMouseUp(UIManager manager, MouseButtons btn, bool onElement);
    }

    public abstract class UIElement : IUIElement
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool Active { get; set; } = true;

        public abstract void Draw(Engine engine, UIManager manager);
        public virtual void Update(Engine engine, UIManager manager, double deltaTime) { }

        public virtual void OnKeyDown(UIManager manager, Key key, Modifiers modifiers) { }
        public virtual void OnKeyUp(UIManager manager, Key key, Modifiers modifiers) { }

        public virtual void OnMouseDown(UIManager manager, MouseButtons btn, bool onElement) { }
        public virtual void OnMouseUp(UIManager manager, MouseButtons btn, bool onElement) { }
    }

    public class UIInputField : UIElement
    {
        public double cursorSwitchTime = 0.5d;
        private double cursorTimer;

        public char CursorChar;

        public bool Selected;

        public string DefaultText;
        public int DefaultColor;

        public string TypedText;
        public int TypedColor;

        public int Size;

        public int MaxLength;
        public int MaxVisible;

        public int padTop;
        public int padLeft;

        public UIIFDelegate OnChange;
        public UIIFDelegate OnConfirm;

        private char cursor;

        public UIInputField(int x, int y, int width, int height, int size, int maxVisible, int maxLength = 50, string defaultText = "Type...")
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;

            Size = size;
            MaxLength = maxLength;
            MaxVisible = maxVisible;
            DefaultColor = Color.Gray.ToArgb();
            TypedColor = Color.Black.ToArgb();
            DefaultText = defaultText;
            TypedText = "";
            CursorChar = '_';
            cursor = CursorChar;
            cursorTimer = 0d;
        }

        public void Clear(bool deselect = true)
        {
            TypedText = "";
            if (deselect)
                Selected = false;
        }

        public override void Draw(Engine engine, UIManager manager)
        {
            engine.FillInter(X, Y, X + Width, Y + Height, Color.White.ToArgb());
            engine.DrawRectInter(X, Y, Width, Height, Color.Black.ToArgb());
            engine.DrawRectInter(X + 1, Y + 1, Width - 2, Height - 2, Color.Black.ToArgb());
            engine.DrawRectInter(X + 2, Y + 2, Width - 4, Height - 4, Color.Black.ToArgb());

            string finalText = TypedText.Length > 0 | Selected ? TypedText : DefaultText;

            if (finalText.Length > MaxVisible)
                finalText = finalText.Substring(finalText.Length - MaxVisible);

            finalText += (Selected && TypedText.Length < MaxLength ? (cursor == '\0' ? "" : cursor.ToString()) : "");

            manager.RenderAndCaschText(finalText, Size, TypedText.Length > 0 | Selected ? TypedColor : DefaultColor, X + padLeft, Y + padTop);
        }

        public override void Update(Engine engine, UIManager manager, double deltaTime)
        {
            cursorTimer += deltaTime;

            if (cursorTimer >= cursorSwitchTime)
            {
                cursorTimer = 0;
                if (cursor == '\0')
                    cursor = CursorChar;
                else
                    cursor = '\0';
            }
        }

        public override void OnMouseDown(UIManager manager, MouseButtons btn, bool onElement)
        {
            if (btn == MouseButtons.Left && onElement)
                Selected = true;
            else if (btn == MouseButtons.Right || (btn == MouseButtons.Left && !onElement))
                Selected = false;
        }

        public override void OnKeyDown(UIManager manager, Key key, Modifiers modifiers)
        {
            if (!Selected)
                return;

            if (key == Key.Enter)
            {
                Selected = false;
                OnConfirm?.Invoke(TypedText);
            } else if (key == Key.Back && TypedText.Length > 0)
            {
                if (TypedText.Length == 1)
                    TypedText = "";
                else
                    TypedText = TypedText.Substring(0, TypedText.Length - 1);

                OnChange?.Invoke(TypedText);
            } else if (key == Key.Space)
            {
                TypedText += ' ';
            } 
            else if (key == Key.V && modifiers == Modifiers.Control)
            {
                if (Clipboard.ContainsText())
                {
                    string clipText = Clipboard.GetText();
                    if (TypedText.Length + clipText.Length <= MaxLength)
                    {
                        TypedText += clipText;
                        OnChange?.Invoke(TypedText);
                    }
                }
            }
            else if (key == Key.Escape)
                Selected = false;
            else
            {
                char ch = MapKey(key, modifiers);
                if (ch != '\0' && TypedText.Length < MaxLength)
                {
                    TypedText += ch;
                    OnChange?.Invoke(TypedText);
                }
            }
        }

        protected virtual char MapKey(Key key, Modifiers modifiers)
        {
            string enumKey = Enum.GetName(typeof(Key), key);

            if (enumKey.Length == 1 && char.IsLetter(enumKey[0]))
            {
                if ((modifiers & Modifiers.Shift) == Modifiers.None)
                    return char.ToLower(enumKey[0]);
                else
                    return char.ToUpper(enumKey[0]);
            }
            else if (enumKey.Length == 7 && enumKey.StartsWith("NumPad") && int.TryParse(enumKey[6].ToString(), out int _))
                return enumKey[6];
            else if (enumKey.Length == 2 && enumKey[0] == 'D')
            {
                if ((modifiers & Modifiers.Shift) == Modifiers.None)
                    return enumKey[1];
                else
                {
                    switch (enumKey[1])
                    {
                        case '0':
                            return ')';
                        case '1':
                            return '!';
                        case '2':
                            return '@';
                        case '3':
                            return '#';
                        case '4':
                            return '$';
                        case '5':
                            return '%';
                        case '6':
                            return '^';
                        case '7':
                            return '&';
                        case '8':
                            return '*';
                        case '9':
                            return '(';
                        default:
                            return '\0';
                    }
                }
            }
            else
            {
                switch (key)
                {
                    case Key.Add:
                        return '+';
                    case Key.Subtract:
                        return '-';
                    case Key.Multiply:
                        return '*';
                    case Key.Divide:
                        return '/';
                    case Key.Oem3:
                        return ';';
                    default:
                        return '\0';
                }
            }
        }
    }

    public delegate void UIIFDelegate(string typed);

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
