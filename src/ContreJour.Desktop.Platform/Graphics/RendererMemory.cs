using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ContreJour.Desktop.Platform.Diagnostics;

using Microsoft.Extensions.Logging;

namespace ContreJour.Desktop.Platform.Graphics
{
    // Remembers which renderer a previous launch died bringing up, so the next launch skips it. A
    // driver that faults natively inside its own initialization takes the process with it, past any
    // catch. A marker is written before each attempt and cleared once one works or fails in a way
    // that returns, so all a later launch can conclude is that an attempt never came back; a renderer
    // that works is tried afresh every time, so a newly installed driver is noticed at once.
    public sealed partial class RendererMemory(string statePath)
    {
        public const string FileName = "renderer.txt";

        private readonly string _statePath = statePath;

        public GraphicsBackendKind? Blamed { get; private set; } = Read(statePath);

        // The order without the blamed renderer, unless that would leave nothing or the order has a
        // single (forced) entry: a run told which renderer to use should fail in the open.
        public GraphicsBackendKind[] Filter(IReadOnlyList<GraphicsBackendKind> order)
        {
            ArgumentNullException.ThrowIfNull(order);
            if (Blamed is not { } blamed || order.Count <= 1)
            {
                return [.. order];
            }
            GraphicsBackendKind[] remaining = [.. order.Where(kind => kind != blamed)];
            return remaining.Length == 0 ? [.. order] : remaining;
        }

        // Must reach the disk before the attempt: the failure it guards against leaves no other trace.
        public void BeginAttempt(GraphicsBackendKind kind)
        {
            Write(kind.ToString());
        }

        public void RecordSuccess()
        {
            Blamed = null;
            Write(string.Empty);
        }

        // A candidate that failed and was released came back, so it is not what the marker is for.
        public void Absolve()
        {
            Blamed = null;
            Write(string.Empty);
        }

        private void Write(string value)
        {
            try
            {
                string directory = Path.GetDirectoryName(_statePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    _ = Directory.CreateDirectory(directory);
                }
                File.WriteAllText(_statePath, value);
            }
            catch (Exception failure) when (IsFileSystemFailure(failure))
            {
                // The game still runs; it only loses this protection on the next launch.
                ILogger logger = Log.For(LogCategories.Graphics);
                MarkerWriteFailed(logger, _statePath, failure);
            }
        }

        // The path comes from a save folder this class does not choose, so a malformed one arrives as
        // an argument failure rather than an I/O one.
        private static bool IsFileSystemFailure(Exception failure)
        {
            return failure is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException;
        }

        private static GraphicsBackendKind? Read(string path)
        {
            try
            {
                return File.Exists(path) && Enum.TryParse(File.ReadAllText(path).Trim(), out GraphicsBackendKind kind) && Enum.IsDefined(kind)
                    ? kind
                    : null;
            }
            catch (Exception failure) when (IsFileSystemFailure(failure))
            {
                ILogger logger = Log.For(LogCategories.Graphics);
                MarkerReadFailed(logger, path, failure);
                return null;
            }
        }

        [LoggerMessage(Level = LogLevel.Warning, Message = "Could not write the renderer marker '{Path}'")]
        private static partial void MarkerWriteFailed(ILogger logger, string path, Exception exception);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Could not read the renderer marker '{Path}'")]
        private static partial void MarkerReadFailed(ILogger logger, string path, Exception exception);
    }
}
