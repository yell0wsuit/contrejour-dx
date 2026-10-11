namespace ContreJourDX.Desktop
{
    // The desktop host has already restored the player's window mode before game initialization.
    public sealed class DesktopContreJourDXApplication : ContreJourDXApplication
    {
        protected override bool StartFullScreen => ApplicationController.IsFullScreen;
    }
}
