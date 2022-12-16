using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderEngineExample
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Run(new Size(1080, 720), "Game");
        }
    }
}
