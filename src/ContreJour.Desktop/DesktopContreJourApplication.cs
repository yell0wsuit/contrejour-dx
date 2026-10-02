namespace ContreJour.Desktop
{
    // The desktop host has already restored the player's window mode before game initialization.
    public sealed class DesktopContreJourApplication : ContreJourApplication
    {
        protected override bool StartFullScreen => ApplicationController.IsFullScreen;
    }
}
