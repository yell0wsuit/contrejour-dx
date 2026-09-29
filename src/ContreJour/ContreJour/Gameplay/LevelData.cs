using System;

namespace ContreJour.Gameplay;

[Serializable]
public class LevelData
{
    private int _score;

    private int _starsCount;

    public int Score
    {
        get => _score;
        set => _score = value;
    }

    public int StarsCount
    {
        get => _starsCount;
        set => _starsCount = value;
    }

    public LevelData()
    {
    }

    public LevelData(int score, int starsCount)
    {
        _score = score;
        _starsCount = starsCount;
    }
}
