using Mokus2D.Input;

namespace ContreJour.Browser.Platform
{
    // The keys the page forwards, numbered so no strings cross the event ring. host-events.js holds the other
    // half of this table (KeyboardEvent.code -> id); the two must agree. The set is the desktop host's, except
    // that Q means Escape (the game's Back) and R means F5 (restart): a browser keeps Escape (it leaves full
    // screen) and F5 (reload) for itself, so the page forwards neither.
    public static class BrowserKeys
    {
        public static Key Map(int id)
        {
            return id switch
            {
                2 => Key.Escape,
                3 => Key.F5,
                4 => Key.Back,
                5 => Key.Enter,
                6 => Key.CapsLock,
                7 => Key.Left,
                8 => Key.Up,
                9 => Key.Right,
                10 => Key.Down,
                11 => Key.Delete,
                12 => Key.A,
                13 => Key.D,
                14 => Key.S,
                15 => Key.W,
                16 => Key.LeftShift,
                17 => Key.RightShift,
                _ => Key.None,
            };
        }
    }
}
