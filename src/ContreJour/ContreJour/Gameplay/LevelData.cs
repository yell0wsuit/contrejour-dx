namespace ContreJour.Gameplay
{
    public class LevelData
    {
        public LevelData()
        {
        }

        public LevelData(int score, int starsCount)
        {
            Score = score;
            StarsCount = starsCount;
        }

        public int Score { get; set; }

        public int StarsCount { get; set; }
    }
}
