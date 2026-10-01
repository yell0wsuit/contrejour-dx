using System;
using System.Numerics;
using System.Runtime.InteropServices;

using ContreJour.Browser.Platform;

namespace ContreJour.Browser
{
    // Drives everything from requestAnimationFrame: drains the page's events, then runs a frame of the game once
    // the player has pressed Play. The loop runs from the end of boot, because it is what reads the event ring -
    // including the press of Play.
    internal static unsafe class GameLoop
    {
        // The ring holds fewer records than this many drains return, so a backlog clears in one frame.
        private const int MaxDrainPasses = 8;

        private static Vector2 _cssSize = Vector2.One;

        private static Vector2 _pixelSize = Vector2.One;

        private static bool _active = true;

        private static int _reportedDroppedEvents;

        private static BrowserGame _game;

        internal static SkiaSurface Surface { get; set; }

        // Builds the game when Play arrives, from the page's shape at that moment and the frame's timestamp.
        internal static Func<Vector2, Vector2, double, BrowserGame> CreateGame { get; set; }

        internal static void Start(int cssWidth, int cssHeight, int pixelWidth, int pixelHeight)
        {
            _cssSize = new Vector2(cssWidth, cssHeight);
            _pixelSize = new Vector2(pixelWidth, pixelHeight);
            HostShim.SetFrameCallback(&OnFrame);
            HostShim.RequestFrame();
        }

        // Nothing may escape: this is called from native code with no handler to unwind into, so an exception
        // would take the runtime down and stop the loop for good rather than drop one frame.
        [UnmanagedCallersOnly]
        private static void OnFrame(double timestampMs)
        {
            bool requestNext = true;
            try
            {
                if (HostShim.ContextLost() != 0)
                {
                    requestNext = false;
                    _game?.OnContextLost();
                    return;
                }
                DrainEvents(timestampMs);
                _game?.Frame(timestampMs);
            }
            catch (Exception failure)
            {
                Console.WriteLine($"cj-frame-error: {failure}");
            }
            finally
            {
                if (requestNext)
                {
                    try
                    {
                        HostShim.RequestFrame();
                    }
                    catch (Exception failure)
                    {
                        Console.WriteLine($"cj-frame-request-error: {failure}");
                    }
                }
            }
        }

        private static void DrainEvents(double timestampMs)
        {
            for (int pass = 0; pass < MaxDrainPasses; pass++)
            {
                ReadOnlySpan<HostEvent> batch = HostEventQueue.Drain();
                if (batch.IsEmpty)
                {
                    break;
                }
                foreach (HostEvent value in batch)
                {
                    Handle(value, timestampMs);
                }
            }
            int dropped = HostEventQueue.DroppedCount();
            if (dropped != _reportedDroppedEvents)
            {
                _reportedDroppedEvents = dropped;
                Console.WriteLine($"cj-host-events-dropped: total={dropped}");
            }
        }

        private static void Handle(in HostEvent value, double timestampMs)
        {
            switch (value.Kind)
            {
                case HostEventKind.Active:
                    _active = value.Word0 != 0;
                    _game?.SetActive(_active);
                    break;
                case HostEventKind.Resize:
                    ApplyResize(
                        BitConverter.Int32BitsToSingle(value.Word0),
                        BitConverter.Int32BitsToSingle(value.Word1),
                        BitConverter.Int32BitsToSingle(value.Word2));
                    break;
                case HostEventKind.Start:
                    if (_game == null)
                    {
                        _game = CreateGame(_cssSize, _pixelSize, timestampMs);
                        _game.SetActive(_active);
                    }
                    break;
                case HostEventKind.Pointer:
                    _game?.Input.HandlePointer(
                        (PointerPhase)(value.Word0 & 0xFF),
                        (PointerKind)(value.Word0 >> 8),
                        value.Word1,
                        new Vector2(BitConverter.Int32BitsToSingle(value.Word2), BitConverter.Int32BitsToSingle(value.Word3)),
                        value.Word4);
                    break;
                case HostEventKind.Key:
                    _game?.Input.HandleKey(value.Word1, value.Word0 != 0);
                    break;
                case HostEventKind.Wheel:
                    _game?.Input.AddWheel(value.Word0);
                    break;
                case HostEventKind.None:
                default:
                    break;
            }
        }

        // The page reports the canvas box; sizing the backing store to it is this side's job.
        private static void ApplyResize(float cssWidth, float cssHeight, float ratio)
        {
            float clamped = BrowserCanvas.PixelRatio(ratio);
            int width = Math.Max(1, (int)Math.Round(cssWidth * clamped));
            int height = Math.Max(1, (int)Math.Round(cssHeight * clamped));
            _cssSize = new Vector2(Math.Max(1f, cssWidth), Math.Max(1f, cssHeight));
            _pixelSize = new Vector2(width, height);
            if (width != Surface.Width || height != Surface.Height)
            {
                _ = HostShim.ResizeCanvas(width, height);
                Surface.Resize(width, height);
            }
            _game?.Resize(_cssSize, _pixelSize);
        }
    }
}
