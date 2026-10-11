using SDL3;

namespace ContreJourDX.Desktop.Platform
{
    public static class WindowEvents
    {
        // True for a window event (SDL numbers them in one block, WindowShown to
        // WindowHDRStateChanged) that names a different window: after a device is replaced, the old
        // window's queued focus and size events must not reach the new one.
        public static bool IsForOtherWindow(in SDL.Event e, uint windowId)
        {
            SDL.EventType type = (SDL.EventType)e.Type;
            return type >= SDL.EventType.WindowShown && type <= SDL.EventType.WindowHDRStateChanged && e.Window.WindowID != windowId;
        }
    }
}
