using System;
using System.Collections.Generic;

using ContreJour.Clips.menu;
using ContreJour.Utils;

using Microsoft.Xna.Framework;

using Mokus2D.Events;
using Mokus2D.Util;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;
using Mokus2D.Visual.Text;

namespace Default.Namespace;

public class LevelsMenu : ClickableLayer
{
    public static readonly int ROWS = 4;

    public static readonly int COLUMNS = 20 / ROWS;

    public static Vector2 BORDER_OFFSET_IPHONE = new Vector2(120f, 80f) * 2f;

    public static Vector2 BORDER_OFFSET = new(200f, 220f);

    private static readonly Vector2 GetMorePosition = new(0f, -120f);

    public static List<List<int>> LEVELS_LIST = [];

    private readonly float RowOffset = 120f;

    public static int[,] LEVELS = new int[6, 20]
    {
        {
            0, 45, 1, 4, 35, 3, 19, 26, 9, 47,
            27, 14, 61, 64, 118, 72, 7, 23, 6, 119
        },
        {
            33, 13, 121, 16, 90, 82, 86, 32, 124, 28,
            77, 57, 83, 31, 94, 67, 46, 84, 75, 78
        },
        {
            18, 51, 49, 120, 36, 56, 52, 95, 37, 81,
            79, 91, 85, 92, 93, 88, 74, 89, 80, 55
        },
        {
            98, 105, 97, 99, 108, 96, 107, 109, 111, 103,
            104, 100, 114, 102, 106, 112, 110, 101, 113, 116
        },
        {
            149, 147, 153, 148, 152, 157, 141, 132, 133, 139,
            151, 146, 128, 150, 154, 123, 156, 168, 155, 134
        },
        {
            188, 171, 170, 172, 190, 196, 191, 176, 186, 174,
            194, 178, 195, 182, 189, 197, 198, 183, 179, 199
        }
    };

    public readonly EventSender GetMoreEvent = new();

    public float InitialScale = 1f;

    private Vector2 initialPosition;

    private readonly Sprite adsButton;

    private Vector2 adsButtonPosition;

    public override Vector2 Position
    {
        set
        {
            base.Position = value;
            adsButton?.Position = adsButtonPosition + ((Position - initialPosition) * 0.7f);
        }
    }

    private static int LockedRows => Constants.NormalChaptersCount - UserData.Instance.UnlockedChapters;

    public static List<List<int>> LevelsList
    {
        get
        {
            if (LEVELS_LIST.Count == 0)
            {
                for (int i = 0; i < Constants.ChaptersCount; i++)
                {
                    LEVELS_LIST.Add([]);
                    for (int j = 0; j < 20; j++)
                    {
                        LEVELS_LIST[i].Add(LEVELS[i, j]);
                    }
                }
            }
            return LEVELS_LIST;
        }
    }

    public event Action<int> SelectLevelEvent;

    public LevelsMenu(int chapter, Vector2 initialPosition)
    {
        this.initialPosition = initialPosition;
        Position = initialPosition;
        List<int> list = LevelsList[chapter];
        Vector2 w7FromIPhoneSize = ScreenConstants.W7FromIPhoneSize;
        Vector2 bORDER_OFFSET_IPHONE = BORDER_OFFSET_IPHONE;
        Vector2 vector = new Vector2(bORDER_OFFSET_IPHONE.X, w7FromIPhoneSize.Y - bORDER_OFFSET_IPHONE.Y) - ScreenConstants.W7FromIPhoneScreenCenter;
        Vector2 vector2 = new((w7FromIPhoneSize.X - (bORDER_OFFSET_IPHONE.X * 2f)) / (COLUMNS - 1), (w7FromIPhoneSize.Y - (bORDER_OFFSET_IPHONE.Y * 2f)) / (ROWS - 1));
        if (!Constants.IsTrial && UserData.Instance.LastLevelOpen && chapter == 4)
        {
            vector = CreateRoseButton(vector, w7FromIPhoneSize, bORDER_OFFSET_IPHONE);
        }
        bool flag = chapter == 5;
        for (int i = 0; i < 20; i++)
        {
            int level = list[i];
            LevelPosition levelPosition = GetLevelPosition(level);
            bool flag2 = UserData.Instance.GetUnlockedLevels(levelPosition.Chapter) >= levelPosition.Index;
            bool flag3 = Constants.IsTrial && i >= 10;
            if (flag)
            {
                flag3 = i >= (ROWS - LockedRows) * COLUMNS;
            }
            LevelItem levelItem = new(level, flag2 && !flag3, flag3)
            {
                RealScale = 1.0925f
            };
            AddChild(levelItem);
            levelItem.Position = vector + new Vector2(vector2.X * (i % COLUMNS), (0f - vector2.Y) * (i / COLUMNS));
            levelItem.TouchEndEvent += OnLevelClick;
            if (flag)
            {
                levelItem.Color = ContreJourConstants.GreenLightColor.LerpToWhite(0.5f);
            }
        }
        if (Constants.IsTrial)
        {
            adsButton = CreateGetMoreButton(chapter);
        }
        else if (flag && LockedRows > 0)
        {
            adsButton = CreateUnlockButton();
        }
    }

    private Sprite CreateUnlockButton()
    {
        adsButtonPosition = new Vector2(0f, (0f - RowOffset) / 2f * (4 - LockedRows));
        Sprite sprite = new("McGetMoreLevelsButton");
        AddChild(sprite);
        sprite.Position = adsButtonPosition;
        sprite.Scale = 1.45f;
        sprite.Color = ContreJourConstants.GreenLightColor;
        Label label = ContreJourLabelUtil.CreateLabel(20f, "COMPLETE_MORE_CHAPTERS");
        label.Scale *= 0.75f;
        sprite.AddChild(label);
        label.Y = 6f;
        return sprite;
    }

    public void Show()
    {
        InteractionsEnabled = true;
    }

    private TouchSprite CreateGetMoreButton(int chapter)
    {
        TouchSprite touchSprite = new("McGetMoreLevelsButton");
        Node node;
        if (ContreJourLabelUtil.IsEnglish)
        {
            node = new Sprite("McGetMoreLevelsText");
        }
        else
        {
            node = ContreJourLabelUtil.CreateLabel(20f, "MORE_LEVELS_IN_FULL_VERSION");
            node.Scale = 0.8f;
        }
        touchSprite.AddChild(node);
        touchSprite.TouchEndEvent += delegate
        {
            GetMoreEvent.SendEvent();
        };
        node.Y = 10f;
        touchSprite.Scale = 1.45f;
        new ButtonSprite(touchSprite).TargetScale = touchSprite.Scale * 1.03f;
        AddChild(touchSprite);
        adsButtonPosition = GetMorePosition;
        touchSprite.Position = GetMorePosition;
        if (chapter == 1)
        {
            touchSprite.Color = Color.Lerp(Color.White, ContreJourConstants.BLUE_LIGHT_COLOR, 0.7f);
        }
        return touchSprite;
    }

    private Vector2 CreateRoseButton(Vector2 position, Vector2 winSize, Vector2 borderOffset)
    {
        Button button = new("menu/McRoseButton", null, null);
        AddChild(button);
        button.Position = new Vector2(winSize.X / 2f, borderOffset.Y - (button.Size.Y / 2f) - 15f) - ScreenConstants.W7FromIPhoneScreenCenter;
        button.TouchEndEvent += OnRoseClick;
        button.RealScale = 1.15f;
        position.Y += (button.Size.Y / 2f) - 15f;
        Scale = 0.85f;
        InitialScale = Scale;
        McVenzel node = new()
        {
            Position = button.Position + new Vector2(56f, 0f),
            Scale = 1.15f
        };
        AddChild(node);
        node = new McVenzel
        {
            ScaleX = -1f
        };
        node.ScaleVec *= 1.15f;
        node.Position = button.Position - new Vector2(56f, 0f);
        AddChild(node);
        return position;
    }

    public static int GetLevelIndex(LevelPosition position)
    {
        return LevelsList[position.Chapter][position.Index];
    }

    public static LevelPosition GetLevelPosition(int level)
    {
        if (level == 169)
        {
            return LevelPosition.EndGame;
        }
        for (int i = 0; i < Constants.ChaptersCount; i++)
        {
            int num = LevelsList[i].IndexOf(level);
            if (num != -1)
            {
                return new LevelPosition(i, num);
            }
        }
        throw new ArgumentOutOfRangeException(nameof(level), level, "Invalid level index.");
    }

    private void OnRoseClick(TouchArguments touchArguments)
    {
        if (Scale == InitialScale)
        {
            SelectLevelEvent.Dispatch(169);
        }
    }

    private void OnLevelClick(TouchArguments touchArguments)
    {
        InteractionsEnabled = false;
        if (Scale == InitialScale)
        {
            SelectLevelEvent.Dispatch(((LevelItem)touchArguments.Target).Level);
        }
    }
}
