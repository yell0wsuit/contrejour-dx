namespace ContreJourMono.ContreJour.Game.Eyes
{
    public class EyeAnimation(string background, string eyeBall = null, bool lockY = false, bool lockX = false)
    {
        public string Background { get; } = background;

        public string EyeBall { get; } = eyeBall;

        public bool LockX { get; } = lockX;

        public bool LockY { get; } = lockY;

        public bool ReplaceBackground => Background != null;

        public bool ReplaceEye => EyeBall != null;
    }
}
