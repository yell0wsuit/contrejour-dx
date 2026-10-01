using System.Runtime.InteropServices;

namespace ContreJour.Browser
{
    // Native/cjhost.cpp, which runs in the page's JavaScript scope: the canvas, the WebGL context and the
    // animation frame.
    internal static unsafe partial class HostShim
    {
        private const string Library = "cjhost";

        [LibraryImport(Library, EntryPoint = "cj_set_frame_callback")]
        internal static partial void SetFrameCallback(delegate* unmanaged<double, void> callback);

        [LibraryImport(Library, EntryPoint = "cj_request_frame")]
        internal static partial void RequestFrame();

        [LibraryImport(Library, EntryPoint = "cj_acquire_canvas")]
        internal static partial int AcquireCanvas();

        [LibraryImport(Library, EntryPoint = "cj_create_context")]
        internal static partial int CreateContext(int width, int height);

        [LibraryImport(Library, EntryPoint = "cj_resize_canvas")]
        internal static partial int ResizeCanvas(int width, int height);

        [LibraryImport(Library, EntryPoint = "cj_context_lost")]
        internal static partial int ContextLost();

        // Stable for the process, which lets the page keep writing to it across a memory growth that
        // replaces every typed-array view.
        [LibraryImport(Library, EntryPoint = "cj_event_buffer")]
        internal static partial nint EventBuffer(int bytes);
    }
}
