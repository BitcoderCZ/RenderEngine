using GameEngine;
using GameEngine.Font;
using GameEngine.Inputs;
using GameEngine.Maths.Vectors;
using GameEngine.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RenderEngineExample
{
    public class Game : Engine
    {
        private int fontID;
        private UIManager ui;
        private uint textId;

        protected override void Initialize()
        {
            string fontPath = Environment.CurrentDirectory + "\\Roboto-Thin.ttf";
            fontID = fontPath.GetHashCode();
            FontLibrary.GetOrCreateFromFile(fontPath);
            ui = new UIManager(this, FontLibrary.Get(fontID));
            textId = ui.CreateTextCenterX("     Hello World\nSpace Pressed for\nx frames", 0, 200, 90, Color.Black.ToArgb());
        }

        protected override void drawInternal()
        {
            // Update
            float delta = FpsCounter.DeltaTimeF;
            GameWindow.Input.Update();

            // Render
            Clear(Color.White);

            FillTriangle(50, 600, 500, 50, 1030, 600, Color.Red);

            ui.RenderAndCaschText(FpsCounter.FpsString, 16, Color.Gray.ToArgb(), 50, 50);

            (ui.GetElement(textId) as UIText).Text = 
                $"    Hello World\nSpace Pressed for\n{GameWindow.Input.GetKey(Key.Space).pressedDuration} frames";
            ui.Draw();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ui.ClearCaschedText();
            ui.ClearUI();
            base.Exit(); // Disposes FontLibrary
            base.OnExit(e);
        }
    }
}
