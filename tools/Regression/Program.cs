using System;
using System.Globalization;
using System.IO;

using Default.Namespace;

using Mokus2D.Game;

namespace ContreJour.Regression;

// Plays every level and opens every chapter menu with no input on a fixed timestep, and writes a
// fingerprint of the physics world and scene graph for each, plus of the save file format. Run it
// before and after a change and diff the outputs to catch any behaviour change (physics, level
// loading, reflection/serialization by name, save compatibility).
//
// Usage: dotnet run --project tools/Regression -- <output file> [first] [count]
// where first/count select a range of the playable level list (default: all of them).
// Set CJ_REGRESSION_TRACE=<file> to also dump every recorded frame's bodies and nodes.
// Set CJ_REGRESSION_OLD_SAVE=<save file> to also check that an older save still loads the same.
//
// Outputs are only comparable on the same machine: math library results can differ across OS/CPU.
public static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        RegressionApplication.OutputPath = Path.GetFullPath(args.Length > 0 ? args[0] : "regression.txt");
        if (args.Length > 1)
        {
            RegressionApplication.First = int.Parse(args[1], CultureInfo.InvariantCulture);
        }
        if (args.Length > 2)
        {
            RegressionApplication.Count = int.Parse(args[2], CultureInfo.InvariantCulture);
        }

        // Content paths are relative to the install folder, as in the game.
        Environment.CurrentDirectory = AppContext.BaseDirectory;

        // Start from a fresh save every run, and never touch the player's real one.
        string dataDirectory = Path.Combine(Path.GetTempPath(), "ContreJourRegression");
        if (Directory.Exists(dataDirectory))
        {
            Directory.Delete(dataDirectory, recursive: true);
        }
        UserData.DataDirectory = dataDirectory;

        using Mokus2DApplication<RegressionApplication> game = new();
        game.Run();
        return RegressionApplication.Failed ? 1 : 0;
    }
}
