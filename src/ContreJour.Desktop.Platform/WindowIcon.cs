using System;
using System.IO;

using SDL3;

namespace ContreJour.Desktop.Platform
{
    // The window's icon, from a 32-bit BMP with alpha (distribution/icons/ContreJourDXIcon.bmp, which the
    // host embeds as Icon.bmp). MonoGame used to find that resource on its own; SDL has to be handed it,
    // once per window, since recovery replaces the window.
    public static class WindowIcon
    {
        public static void Apply(nint window, byte[] bmp)
        {
            nint surface = Load(bmp);
            try
            {
                if (!SDL.SetWindowIcon(window, surface))
                {
                    throw new InvalidOperationException($"Could not set the window icon: {SDL.GetError()}");
                }
            }
            finally
            {
                SDL.DestroySurface(surface);
            }
        }

        // An SDL surface the caller destroys. Throws InvalidDataException when SDL cannot read the bytes.
        public static unsafe nint Load(byte[] bmp)
        {
            ArgumentNullException.ThrowIfNull(bmp);
            // The surface owns a copy of the pixels, so the buffer only has to stay pinned for the call.
            fixed (byte* data = bmp)
            {
                nint stream = SDL.IOFromConstMem((nint)data, (nuint)bmp.Length);
                if (stream == 0)
                {
                    throw new InvalidDataException($"Could not open the icon: {SDL.GetError()}");
                }
                nint surface = SDL.LoadBMPIO(stream, true);
                return surface != 0 ? surface : throw new InvalidDataException($"Could not read the icon: {SDL.GetError()}");
            }
        }
    }
}
