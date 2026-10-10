using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ContreJourDX.Desktop.Platform.Graphics
{
    // Brings up renderers in preference order and keeps the first that passes its validation frame
    // (from cuttherope-dx).
    public static class BackendSelector
    {
        public static string CurrentPlatform()
        {
            return OperatingSystem.IsMacOS()
                ? "macos"
                : OperatingSystem.IsWindows()
                ? "windows"
                : OperatingSystem.IsLinux() ? "linux" : throw new PlatformNotSupportedException(RuntimeInformation.OSDescription);
        }

        // The renderers to try, best first; a forced one is the only candidate. Vulkan leads where
        // SkiaSharp ships it; ANGLE (Windows only) comes before the native GL driver, which is the
        // part of an old or broken Windows machine most likely to be wrong.
        public static GraphicsBackendKind[] PreferenceOrder(string platform, GraphicsBackendKind? forced)
        {
            return forced.HasValue
                ? [forced.Value]
                : platform switch
                {
                    "windows" => [GraphicsBackendKind.Vulkan, GraphicsBackendKind.Angle, GraphicsBackendKind.OpenGL, GraphicsBackendKind.Software],
                    "linux" => [GraphicsBackendKind.Vulkan, GraphicsBackendKind.OpenGL, GraphicsBackendKind.Software],
                    "macos" => [GraphicsBackendKind.Metal, GraphicsBackendKind.OpenGL, GraphicsBackendKind.Software],
                    _ => throw new PlatformNotSupportedException(platform),
                };
        }

        public static GraphicsSelection<T> Select<T>(string platform, GraphicsBackendKind? forced,
            Func<GraphicsBackendKind, CandidateLifetime, T> create, Action<T> validate)
            where T : IDisposable
        {
            return Attempt(PreferenceOrder(platform, forced), create, validate);
        }

        // create builds one candidate, registering what it acquires with the lifetime as it goes;
        // validate draws and presents a frame, throwing if it cannot. absolve runs once a rejected
        // candidate is fully released: it failed in a way that returned, not one that killed the
        // process.
        public static GraphicsSelection<T> Attempt<T>(IReadOnlyList<GraphicsBackendKind> order,
            Func<GraphicsBackendKind, CandidateLifetime, T> create, Action<T> validate, Action absolve = null)
            where T : IDisposable
        {
            ArgumentNullException.ThrowIfNull(order);
            ArgumentNullException.ThrowIfNull(create);
            ArgumentNullException.ThrowIfNull(validate);
            List<RendererFailure> failures = [];
            foreach (GraphicsBackendKind kind in order)
            {
                CandidateLifetime lifetime = new();
                try
                {
                    T device = create(kind, lifetime);
                    validate(device);
                    return new GraphicsSelection<T>(kind, device, lifetime, failures.AsReadOnly());
                }
                catch (Exception failure)
                {
                    failures.Add(new RendererFailure(kind, failure));
                    try
                    {
                        lifetime.Dispose();
                    }
                    catch (Exception cleanupFailure)
                    {
                        failures.Add(new RendererFailure(kind, cleanupFailure));
                    }
                    absolve?.Invoke();
                }
            }
            // The failures need not name their renderer (an SDL error string says nothing about who
            // asked), so the message carries it: nothing downstream has a device to report against.
            throw new AggregateException(
                "No desktop renderer completed its validation frame. Passed over: "
                    + string.Join(", ", failures.Select(failure => $"{failure.Kind} ({failure.Failure.Message})")),
                failures.Select(failure => failure.Failure));
        }
    }

    // Why one renderer was passed over, and which one it was.
    public readonly record struct RendererFailure(GraphicsBackendKind Kind, Exception Failure);

    // A validated device and every dependency acquired with it.
    public sealed class GraphicsSelection<T> : IDisposable where T : IDisposable
    {
        private readonly CandidateLifetime _lifetime;

        internal GraphicsSelection(GraphicsBackendKind kind, T device, CandidateLifetime lifetime, IReadOnlyList<RendererFailure> failures)
        {
            Kind = kind;
            Device = device;
            _lifetime = lifetime;
            Failures = failures;
        }

        public GraphicsBackendKind Kind { get; }

        public T Device { get; }

        // Earlier candidates' failures, in attempt order.
        public IReadOnlyList<RendererFailure> Failures { get; }

        public void Dispose()
        {
            _lifetime.Dispose();
        }
    }
}
