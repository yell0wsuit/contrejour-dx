using System;
using System.Globalization;
using System.IO;

using ContreJour.Saving;

using Mokus2D.Sound;

namespace ContreJour.Regression
{
    // Plays every level and opens every chapter menu with no input on a fixed timestep, and writes a
    // fingerprint of the physics world and scene graph for each, hashes of two rendered frames (once
    // settled, and at the end), plus of the save file format. Run it
    // before and after a change and diff the outputs to catch any behaviour change (physics, level
    // loading, reflection/serialization by name, save compatibility).
    //
    // Usage: dotnet run --project src/ContreJour.Regression -- <output file> [first] [count]
    // where first/count select a range of the playable level list (default: all of them).
    // Set CJ_REGRESSION_TRACE=<file> to also dump every recorded frame's bodies and nodes.
    // Set CJ_REGRESSION_OLD_SAVE=<save file> to also check that an older save still loads the same.
    // Set CJ_REGRESSION_PIXELS=<folder> to also save every captured frame there as a PNG.
    // Set CJ_REGRESSION_SIZE=<width>x<height> to check a different viewport.
    // Set CJ_REGRESSION_NEW_FRIEND_INTERACTIONS=1 to exercise companion input/lifecycle in level300 and flowers in level306.
    // Set CJ_REGRESSION_MANGO_INTERACTIONS=1 to exercise anchor movement, restart, pause, and the Mango planet.
    // Set CJ_REGRESSION_MANGO_LOCKS=1 to verify Mango menu locks and its banner at every chapter gate.
    // Set CJ_REGRESSION_FINISH=1 to open each selected level's finish screen and verify its menu portal.
    // Set CJ_REGRESSION_CONTENT_ROOT=<desktop output directory> to check that build's deployed assets.
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

            string pixelsPath = Environment.GetEnvironmentVariable("CJ_REGRESSION_PIXELS");
            if (!string.IsNullOrEmpty(pixelsPath))
            {
                RegressionApplication.PixelsPath = Path.GetFullPath(pixelsPath);
            }

            // Content paths are relative to the install folder, as in the game.
            Environment.CurrentDirectory = Environment.GetEnvironmentVariable("CJ_REGRESSION_CONTENT_ROOT") ?? AppContext.BaseDirectory;

            // Start from a fresh save every run, and never touch the player's real one.
            string dataDirectory = Path.Combine(Path.GetTempPath(), "ContreJourRegression");
            if (Directory.Exists(dataDirectory))
            {
                Directory.Delete(dataDirectory, recursive: true);
            }
            Preferences.SaveDirectory = dataDirectory;

            // The harness never plays audio.
            using NullAudioBackend audio = new();
            using HeadlessApplication<RegressionApplication> game = new(audio);
            RegressionApplication.CaptureFrame = game.CaptureFrame;
            game.Run();
            return RegressionApplication.Failed ? 1 : 0;
        }
    }
}
