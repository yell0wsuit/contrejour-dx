using Microsoft.Xna.Framework;

using Mokus2D.Data;
using Mokus2D.Input;
using Mokus2D.Visual;
using Mokus2D.Visual.Interfaces;

namespace Default.Namespace;

public class LevelItem : Button, IBoundsNode, ISizeNode
{
    public const float EffectTime = 0.1f;
    private readonly int index;

    private readonly int level;

    private readonly bool unlocked;

    public int Level => level;

    public int Index => index;

    public LevelItem(int level, bool unlocked, bool trialLocked)
        : base(unlocked ? "menu/McLevelItemBackground" : "menu/McLevelItemInactive", "menu/McLevelItemSelected", null)
    {
        LevelPosition levelPosition = LevelsMenu.GetLevelPosition(level);
        this.unlocked = unlocked;
        this.level = level;
        index = levelPosition.Index;
        if (!trialLocked)
        {
            CreateLabel(levelPosition);
        }
        else
        {
            OpacityFloat = 0.5f;
        }
        LevelData levelData = UserData.Instance.GetLevelData(levelPosition.GlobalPosition());
        Enabled = unlocked;
        if (unlocked)
        {
            for (int i = 0; i < 3; i++)
            {
                bool flag = levelData != null && i < levelData.StarsCount;
                AddChild(new Sprite(flag ? "menu/McLevelEnergy" : "menu/McLevelEnergyInactive")
                {
                    IgnoreParentColor = flag,
                    Position = new Vector2(36f, 22f * (-1f + i))
                });
            }
        }
        else if (levelPosition.Chapter == 1)
        {
            base.Color = Color.Lerp(Color.White, ContreJourConstants.BlueLightColor, 0.7f);
        }
    }

    private void CreateLabel(LevelPosition levelPosition)
    {
        bool flag = index < 9;
        int num = index + 1;
        Node node;
        if (flag)
        {
            node = CreateDigitChapter(num, levelPosition.Chapter);
        }
        else
        {
            node = new Node();
            Node node2 = CreateDigitChapter(num / 10, levelPosition.Chapter);
            node2.Position = new Vector2(-12f, 0f);
            node.AddChild(node2);
            node2 = CreateDigitChapter(num % 10, levelPosition.Chapter);
            node2.Position = new Vector2(12f, 0f);
            node.AddChild(node2);
        }
        AddChild(node);
        node.Position = new Vector2(-11f, 0f);
    }

    public Node CreateDigitChapter(int character, int chapter)
    {
        Sprite sprite = new($"menu/McLevels{character}");
        Color color = Color.Lerp(Color.White, ContreJourConstants.BlueLightColor, 0.5f);
        sprite.Color = (chapter == 1) ? color : ContreJourConstants.GreyColor;
        if (!unlocked)
        {
            sprite.OpacityByte = (chapter == 1) ? 150 : 80;
        }
        return sprite;
    }

    public override void TouchEnd(Touch touch)
    {
        if (unlocked)
        {
            base.TouchEnd(touch);
        }
    }

    protected override RectangleFloat CalculateBounds()
    {
        return new RectangleFloat(-50f, -40f, 100f, 80f);
    }
}
