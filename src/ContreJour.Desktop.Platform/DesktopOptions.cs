using System;
using System.Collections.Generic;
using System.Globalization;

namespace ContreJour.Desktop.Platform
{
    // The desktop command line. Switches this does not know are ignored: --log-level is read by
    // LoggingSetup, and macOS can add its own. A known switch with a bad or missing value throws
    // ArgumentException.
    public sealed class DesktopOptions
    {
        // Quit cleanly once this many frames have been presented: for scripted runs.
        public int? QuitAfterFrames { get; private init; }

        public static DesktopOptions Parse(IReadOnlyList<string> args)
        {
            ArgumentNullException.ThrowIfNull(args);
            int? quitAfterFrames = null;
            for (int i = 0; i < args.Count; i++)
            {
                string name = args[i];
                if (name == "--quit-after-frames")
                {
                    quitAfterFrames = ParseFrame(name, ValueAfter(args, ref i));
                }
            }
            return new DesktopOptions
            {
                QuitAfterFrames = quitAfterFrames,
            };
        }

        private static string ValueAfter(IReadOnlyList<string> args, ref int i)
        {
            if (i + 1 >= args.Count)
            {
                throw new ArgumentException($"{args[i]} needs a value.");
            }
            i++;
            return args[i];
        }

        private static int ParseFrame(string name, string value)
        {
            return int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out int frame) && frame >= 1
                ? frame
                : throw new ArgumentException($"{name} expects a frame number of 1 or more, not '{value}'.");
        }
    }
}
