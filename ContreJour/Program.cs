using System;

using Mokus2D.Game;

namespace ContreJour;

public static class Program
{
    [STAThread]
    private static void Main()
    {
        // Content paths are relative to the install folder, as they were inside the appx package.
        Environment.CurrentDirectory = AppContext.BaseDirectory;
        using Mokus2DApplication<ContreJourApplication> game = new Mokus2DApplication<ContreJourApplication>();
        game.Run();
    }
}
