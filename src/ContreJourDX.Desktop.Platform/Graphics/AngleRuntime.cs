using System.IO;

namespace ContreJourDX.Desktop.Platform.Graphics
{
    // Finds the ANGLE libraries the Windows release ships beside the executable (from cuttherope-dx's
    // AngleRuntime). A build without them (a development build, or any non-Windows one) reports
    // nothing here, so the ANGLE candidate fails at once instead of half-starting on its way to the
    // same answer.
    public static class AngleRuntime
    {
        public const string DirectoryName = "angle";

        // Both absolute paths, or false and nulls when either library is missing.
        public static bool TryLocate(string baseDirectory, out string eglLibrary, out string glesLibrary)
        {
            eglLibrary = null;
            glesLibrary = null;
            if (string.IsNullOrEmpty(baseDirectory))
            {
                return false;
            }
            string directory = Path.Combine(baseDirectory, DirectoryName);
            string egl = Path.Combine(directory, "libEGL.dll");
            string gles = Path.Combine(directory, "libGLESv2.dll");
            if (!File.Exists(egl) || !File.Exists(gles))
            {
                return false;
            }
            eglLibrary = egl;
            glesLibrary = gles;
            return true;
        }
    }
}
