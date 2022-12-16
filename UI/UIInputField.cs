using GameEngine.Inputs;
using System;
using System.Drawing;
using Clipboard = System.Windows.Forms.Clipboard;

namespace GameEngine.UI
{
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
}
