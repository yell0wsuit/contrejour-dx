namespace ContreJourMono.ContreJour.Game.Eyes;

public class EyeAnimation
{
    private string background;

    private string eyeBall;

    private bool lockX;

    private bool lockY;

    public string Background => background;

    public string EyeBall => eyeBall;

    public bool LockX => lockX;

    public bool LockY => lockY;

    public bool ReplaceBackground => background != null;

    public bool ReplaceEye => eyeBall != null;

    public EyeAnimation(string background, string eyeBall = null, bool lockY = false, bool lockX = false)
    {
        this.background = background;
        this.eyeBall = eyeBall;
        this.lockX = lockX;
        this.lockY = lockY;
    }
}
