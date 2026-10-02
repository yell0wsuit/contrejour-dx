using System;

namespace ContreJour.Desktop.Platform.Graphics
{
    // Which flavor of GL a context asks for, and which libraries it comes from (from cuttherope-dx's
    // GlContextProfile).
    public sealed class GlContextProfile
    {
        // SDL's core profile mask.
        public const int CoreMask = 1;

        // SDL's OpenGL ES profile mask.
        public const int EsMask = 4;

        private GlContextProfile(int profileMask, int major, int minor, string eglLibrary, string glesLibrary)
        {
            ProfileMask = profileMask;
            Major = major;
            Minor = minor;
            EglLibrary = eglLibrary;
            GlesLibrary = glesLibrary;
        }

        public int ProfileMask { get; }

        public int Major { get; }

        public int Minor { get; }

        // Absolute paths to ANGLE's libraries, or null to let SDL load the system driver.
        public string EglLibrary { get; }

        public string GlesLibrary { get; }

        public bool UsesAngle => EglLibrary != null;

        // Core 3.2 from whichever driver the system provides.
        public static GlContextProfile DesktopCore { get; } = new(CoreMask, 3, 2, null, null);

        // ANGLE reports ES 2.0 rather than 3.0 on Direct3D feature level 10_0, because the feature that
        // would lift it is off and SDL cannot turn it on. That DX10-generation hardware is exactly what
        // ANGLE is here for, so a refused ES 3.0 context is worth one attempt at ES 2.0. Null when there
        // is nothing lower worth asking for.
        public GlContextProfile Retry => UsesAngle && Major == 3 ? new(EsMask, 2, 0, EglLibrary, GlesLibrary) : null;

        // OpenGL ES 3.0 from the given ANGLE libraries.
        public static GlContextProfile Angle(string eglLibrary, string glesLibrary)
        {
            return new(EsMask, 3, 0, eglLibrary, glesLibrary);
        }

        // start builds and initializes one device for a profile. Only a context SDL refused earns the
        // Retry; any other failure, an injected fault included, rejects the candidate as it is.
        public T Start<T>(Func<GlContextProfile, T> start)
        {
            ArgumentNullException.ThrowIfNull(start);
            try
            {
                return start(this);
            }
            catch (GlContextRefusedException) when (Retry != null)
            {
                return start(Retry);
            }
        }
    }
}
