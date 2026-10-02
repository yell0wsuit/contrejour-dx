using System;
using System.Collections.Generic;
using System.Globalization;

using ContreJour.Desktop.Platform.Graphics;

namespace ContreJour.Desktop.Platform
{
    // The desktop command line. Switches this does not know are ignored: --log-level is read by
    // LoggingSetup, and macOS can add its own. A known switch with a bad or missing value throws
    // ArgumentException.
    public sealed class DesktopOptions
    {
        // Quit cleanly once this many frames have been presented: for scripted runs.
        public int? QuitAfterFrames { get; private init; }

        // Null: pick automatically.
        public GraphicsBackendKind? Renderer { get; private init; }

        public IReadOnlyList<int> DeviceLosses { get; private init; } = [];

        public FaultPlan Faults { get; private init; } = FaultPlan.None;

        public static DesktopOptions Parse(IReadOnlyList<string> args)
        {
            ArgumentNullException.ThrowIfNull(args);
            int? quitAfterFrames = null;
            GraphicsBackendKind? renderer = null;
            List<string> faults = [];
            List<int> losses = [];
            for (int i = 0; i < args.Count; i++)
            {
                string name = args[i];
                if (name == "--quit-after-frames")
                {
                    quitAfterFrames = ParseFrame(name, ValueAfter(args, ref i));
                }
                else if (name == "--renderer")
                {
                    renderer = ParseRenderer(ValueAfter(args, ref i));
                }
                else if (name == "--fail-renderer")
                {
                    faults.Add(ValueAfter(args, ref i));
                }
                else if (name == "--lose-device")
                {
                    losses.Add(ParseFrame(name, ValueAfter(args, ref i)));
                }
            }
            losses.Sort();
            return new DesktopOptions
            {
                QuitAfterFrames = quitAfterFrames,
                DeviceLosses = losses,
                Renderer = renderer,
                Faults = FaultPlan.Parse(faults),
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

        private static GraphicsBackendKind? ParseRenderer(string value)
        {
            return string.Equals(value, "auto", StringComparison.OrdinalIgnoreCase)
                ? null
                : GraphicsBackendNames.TryParse(value, out GraphicsBackendKind kind)
                ? (GraphicsBackendKind?)kind
                : throw new ArgumentException($"Unknown --renderer '{value}'. Expected auto, {GraphicsBackendNames.Expected}.");
        }
    }
}
