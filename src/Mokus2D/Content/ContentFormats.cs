namespace Mokus2D.Content
{
    // The extensions, with their leading dot, of the content the host ships. The engine names content without
    // them (or with a placeholder that Path.ChangeExtension swaps out), so the desktop can ship PNG, WAV and
    // FLAC while the browser ships WebP and Ogg.
    public sealed record ContentFormats(string Image, string Sound, string Song)
    {
        public static ContentFormats Desktop { get; } = new(".png", ".wav", ".flac");
    }
}
