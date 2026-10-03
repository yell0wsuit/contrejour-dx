namespace ContreJour.Gameplay
{
    public class LevelPosition
    {
        public int Index { get; set; }

        public int Chapter { get; }

        public bool IsEndGame => Index == -1;

        public int MenuChapter => !IsEndGame ? Chapter : Constants.RoseChapter;

        public static LevelPosition EndGame => new(0, -1);

        public bool SkipAvailable => !IsEndGame;

        public LevelPosition()
        {
            Chapter = -1;
            Index = -1;
        }

        public LevelPosition(int chapter, int index)
        {
            Chapter = chapter;
            Index = index;
        }

        public int GlobalPosition()
        {
            return (Chapter * 20) + Index;
        }
    }
}
