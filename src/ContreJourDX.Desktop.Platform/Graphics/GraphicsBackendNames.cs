using System;

namespace ContreJourDX.Desktop.Platform.Graphics
{
    // The renderer names the command line accepts, in any case.
    public static class GraphicsBackendNames
    {
        public const string Expected = "metal, vulkan, angle, gl, opengl or software";

        public static bool TryParse(string name, out GraphicsBackendKind kind)
        {
            kind = default;
            if (string.Equals(name, "metal", StringComparison.OrdinalIgnoreCase))
            {
                kind = GraphicsBackendKind.Metal;
            }
            else if (string.Equals(name, "vulkan", StringComparison.OrdinalIgnoreCase))
            {
                kind = GraphicsBackendKind.Vulkan;
            }
            else if (string.Equals(name, "angle", StringComparison.OrdinalIgnoreCase))
            {
                kind = GraphicsBackendKind.Angle;
            }
            else if (string.Equals(name, "gl", StringComparison.OrdinalIgnoreCase) || string.Equals(name, "opengl", StringComparison.OrdinalIgnoreCase))
            {
                kind = GraphicsBackendKind.OpenGL;
            }
            else if (string.Equals(name, "software", StringComparison.OrdinalIgnoreCase))
            {
                kind = GraphicsBackendKind.Software;
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}
