namespace Default.Namespace;

public class LevelPosition
{
    private readonly int chapter;

    private int index;

    public int Index
    {
        get
        {
            return index;
        }
        set
        {
            index = value;
        }
    }

    public int Chapter => chapter;

    public bool IsEndGame => index == -1;

    public int MenuChapter
    {
        get
        {
            if (!IsEndGame)
            {
                return chapter;
            }
            return Constants.NormalChaptersCount - 1;
        }
    }

    public static LevelPosition EndGame => new LevelPosition(0, -1);

    public bool SkipAvailable
    {
        get
        {
            if (Chapter == 5 && Index >= (UserData.Instance.UnlockedChapters - 1) * LevelsMenu.COLUMNS - 1)
            {
                return false;
            }
            return true;
        }
    }

    public LevelPosition()
    {
        chapter = -1;
        index = -1;
    }

    public LevelPosition(int chapter, int index)
    {
        this.chapter = chapter;
        this.index = index;
    }

    public int GlobalPosition()
    {
        return chapter * 20 + index;
    }
}
