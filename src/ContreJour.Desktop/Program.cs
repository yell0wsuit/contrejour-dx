using System;

using ContreJour.Desktop.MonoGame;

namespace ContreJour.Desktop
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            // Content paths are relative to the install folder, as they were inside the appx package.
            Environment.CurrentDirectory = AppContext.BaseDirectory;
            using MonoGameApplication<ContreJourApplication> game = new();
            game.Run();
        }
    }
}
