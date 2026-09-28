using System.Diagnostics.CodeAnalysis;

namespace ContreJour.WinRT;

[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Windows 8 live tiles have no desktop equivalent; kept as a no-op.")]
public static class LiveTileUpdater
{
    // Windows 8 Start screen live tiles have no desktop equivalent.
    public static void UpdateTiles(int stars)
    {
    }
}
