using System;
using System.Collections.Generic;
using System.Globalization;

namespace ContreJour.Desktop.Platform.Graphics
{
    // Failures injected at a device's named points, from --fail-renderer KIND:POINT[#N]: the Nth
    // device of that kind built in this run (the first by default) throws when it reaches POINT,
    // once. #1 tests startup fallback; #2 tests a replacement after a device loss.
    public sealed class FaultPlan
    {
        private static readonly string[] PointNames = ["after-device", "before-surface", "after-surface"];

        private readonly List<Fault> _faults;

        private readonly Dictionary<GraphicsBackendKind, int> _built = [];

        private FaultPlan(List<Fault> faults)
        {
            _faults = faults;
        }

        public static FaultPlan None => new([]);

        public static FaultPlan Parse(IEnumerable<string> specs)
        {
            ArgumentNullException.ThrowIfNull(specs);
            List<Fault> faults = [];
            foreach (string spec in specs)
            {
                faults.Add(ParseOne(spec));
            }
            return new FaultPlan(faults);
        }

        // The hook for the next device of this kind.
        public Action<string> For(GraphicsBackendKind kind)
        {
            int instance = _built.GetValueOrDefault(kind) + 1;
            _built[kind] = instance;
            return point =>
            {
                foreach (Fault fault in _faults)
                {
                    if (!fault.Fired && fault.Kind == kind && fault.Instance == instance && fault.Point == point)
                    {
                        fault.Fired = true;
                        throw new InvalidOperationException($"Injected failure at {kind}:{point}#{instance}.");
                    }
                }
            };
        }

        private static Fault ParseOne(string spec)
        {
            string text = spec ?? string.Empty;
            int instance = 1;
            int hash = text.IndexOf('#', StringComparison.Ordinal);
            if (hash >= 0)
            {
                if (!int.TryParse(text.AsSpan(hash + 1), NumberStyles.None, CultureInfo.InvariantCulture, out instance) || instance < 1)
                {
                    throw Malformed(spec);
                }
                text = text[..hash];
            }
            int colon = text.IndexOf(':', StringComparison.Ordinal);
            if (colon < 0 || !GraphicsBackendNames.TryParse(text[..colon], out GraphicsBackendKind kind))
            {
                throw Malformed(spec);
            }
            string point = text[(colon + 1)..];
            return Array.IndexOf(PointNames, point) < 0 ? throw Malformed(spec) : new Fault(kind, point, instance);
        }

        private static ArgumentException Malformed(string spec)
        {
            return new ArgumentException($"Could not read the fault '{spec}'. Expected KIND:POINT or KIND:POINT#N, with KIND "
                + $"{GraphicsBackendNames.Expected} and POINT {string.Join(", ", PointNames)}.");
        }

        private sealed class Fault(GraphicsBackendKind kind, string point, int instance)
        {
            public GraphicsBackendKind Kind { get; } = kind;

            public string Point { get; } = point;

            public int Instance { get; } = instance;

            public bool Fired { get; set; }
        }
    }
}
