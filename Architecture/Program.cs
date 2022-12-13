using System;
using System.Threading;
using System.Windows.Forms;

namespace Digger
{
    internal static class Program
    {
        private static void CreateNewGame()
        {
            var game = new Game();
            Application.Run(new DiggerWindow(game));
        }
        
        [STAThread]
        private static void Main()
        {
            var thread = new Thread(() => CreateNewGame());
            thread.Start();
            thread = new Thread(() => CreateNewGame());
            thread.Start();
        }
    }
}