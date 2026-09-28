namespace Default.Namespace;

public class LevelPosition
{
    private readonly int chapter;

    private int index;

    public int Index
    {
        get => index;
        set => index = value;
    }

    public int Chapter => chapter;

    public bool IsEndGame => index == -1;

    public int MenuChapter => !IsEndGame ? chapter : Constants.NormalChaptersCount - 1;

    public static LevelPosition EndGame => new(0, -1);

    public bool SkipAvailable => Chapter != 5 || Index < ((UserData.Instance.UnlockedChapters - 1) * LevelsMenu.COLUMNS) - 1;

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
        return (chapter * 20) + index;
    }
}
