namespace ContreJourMono.ContreJour.Game.Eyes;

public class EyeAnimation
{
    public string Background { get; }

    public string EyeBall { get; }

    public bool LockX { get; }

    public bool LockY { get; }

    public bool ReplaceBackground => Background != null;

    public bool ReplaceEye => EyeBall != null;

    public EyeAnimation(string background, string eyeBall = null, bool lockY = false, bool lockX = false)
    {
        Background = background;
        EyeBall = eyeBall;
        LockX = lockX;
        LockY = lockY;
    }
}
