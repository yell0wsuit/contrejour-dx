namespace ContreJour.Desktop.Platform.Graphics
{
    // A desktop renderer. The names are what the window title and the log print.
    public enum GraphicsBackendKind
    {
        Metal,
        OpenGL,
        // Skia drawing on the CPU, presented through SDL's software renderer.
        Software,
    }
}
