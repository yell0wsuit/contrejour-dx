namespace ContreJour.Browser.Platform
{
    // The values host-events.js writes into a pointer record.
    public enum PointerPhase
    {
        Down = 0,
        Move = 1,
        Up = 2,
        Cancel = 3,
    }

    public enum PointerKind
    {
        Mouse = 0,
        Pen = 1,
        Touch = 2,
    }
}
