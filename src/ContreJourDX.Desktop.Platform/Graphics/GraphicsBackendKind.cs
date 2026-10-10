namespace ContreJourDX.Desktop.Platform.Graphics
{
    // A desktop renderer. The names are what the window title, the log and the renderer marker print.
    public enum GraphicsBackendKind
    {
        Vulkan,
        Metal,
        OpenGL,
        // OpenGL ES through ANGLE's Direct3D translation, from libraries shipped beside the game.
        Angle,
        // Skia drawing on the CPU, presented through SDL's software renderer.
        Software,
    }
}
