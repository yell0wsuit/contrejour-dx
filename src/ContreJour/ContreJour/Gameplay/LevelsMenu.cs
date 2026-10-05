using System;
using System.Collections.Generic;
using System.Numerics;

using ContreJour.Clips;
using ContreJour.Config;
using ContreJour.Utils;

using Mokus2D.Events;
using Mokus2D.Graphics;
using Mokus2D.Util;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;
using Mokus2D.Visual.Text;

namespace ContreJour.Gameplay
{
    public class LevelsMenu : ClickableLayer
    {
        public static readonly int ROWS = 4;

        public static readonly int COLUMNS = 20 / ROWS;

        public static readonly Vector2 BorderOffsetIphone = new Vector2(120f, 80f) * 2f;

        public static readonly Vector2 BorderOffset = new(200f, 220f);

        private static readonly Vector2 GetMorePosition = new(0f, -120f);

        public static readonly int[,] LEVELS = new int[7, 20]
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
                300, 301, 302, 303, 304, 305, 306, 307, 308, 309,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            },
            {
                188, 171, 170, 172, 190, 196, 191, 176, 186, 174,
                194, 178, 195, 182, 189, 197, 198, 183, 179, 199
            }
        };

        public EventSender GetMoreEvent { get; } = new();

        public float InitialScale { get; set; } = 1f;

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

        public static int GetLevelCount(int chapter)
        {
            return chapter == Constants.NewFriendChapter ? 10 : Constants.LevelsInChapter;
        }

        public static List<List<int>> LevelsList
        {
            get
            {
                if (field.Count == 0)
                {
                    for (int i = 0; i < Constants.ChaptersCount; i++)
                    {
                        field.Add([]);
                        for (int j = 0; j < GetLevelCount(i); j++)
                        {
                            field[i].Add(LEVELS[i, j]);
                        }
                    }
                }
                return field;
            }
        } = [];

        public event Action<int> SelectLevelEvent;

        public LevelsMenu(int chapter, Vector2 initialPosition)
        {
            this.initialPosition = initialPosition;
            Position = initialPosition;
            List<int> list = LevelsList[chapter];
            Vector2 w7FromIPhoneSize = ScreenConstants.W7FromIPhoneSize;
            Vector2 bORDER_OFFSET_IPHONE = BorderOffsetIphone;
            Vector2 vector = new Vector2(bORDER_OFFSET_IPHONE.X, w7FromIPhoneSize.Y - bORDER_OFFSET_IPHONE.Y) - ScreenConstants.W7FromIPhoneScreenCenter;
            Vector2 vector2 = new((w7FromIPhoneSize.X - (bORDER_OFFSET_IPHONE.X * 2f)) / (COLUMNS - 1), (w7FromIPhoneSize.Y - (bORDER_OFFSET_IPHONE.Y * 2f)) / (ROWS - 1));
            if (chapter == Constants.NewFriendChapter)
            {
                int rows = (list.Count + COLUMNS - 1) / COLUMNS;
                vector.Y = vector2.Y * (rows - 1) / 2f;
            }
            if (!Constants.IsTrial && UserData.Instance.LastLevelOpen && chapter == 4)
            {
                vector = CreateRoseButton(vector, w7FromIPhoneSize, bORDER_OFFSET_IPHONE);
            }
            bool flag = chapter is Constants.NewFriendChapter or Constants.BonusChapter;
            for (int i = 0; i < list.Count; i++)
            {
                int level = list[i];
                LevelPosition levelPosition = GetLevelPosition(level);
                bool flag2 = UserData.Instance.GetUnlockedLevels(levelPosition.Chapter) >= levelPosition.Index;
                bool flag3 = (Constants.IsTrial && i >= 10) || levelPosition.LockedByChapterProgress;
                LevelItem levelItem = new(level, flag2 && !flag3, flag3)
                {
                    RealScale = 1.0925f
                };
                AddChild(levelItem);
                levelItem.Position = vector + new Vector2(vector2.X * (i % COLUMNS), (0f - vector2.Y) * (i / COLUMNS));
                levelItem.TouchEndEvent += OnLevelClick;
                if (flag)
                {
                    levelItem.Color = (chapter == Constants.BonusChapter
                        ? ContreJourConstants.GreenLightColor
                        : ContreJourConstants.NewFriendColor).LerpToWhite(0.5f);
                }
            }
            if (Constants.IsTrial)
            {
                adsButton = CreateGetMoreButton(chapter);
            }
            else if (chapter == Constants.BonusChapter && UserData.Instance.AvailableBonusLevels < list.Count)
            {
                adsButton = CreateUnlockButton(vector2.Y);
            }
        }

        public void Show()
        {
            InteractionsEnabled = true;
        }

        private Sprite CreateUnlockButton(float rowOffset)
        {
            int availableRows = UserData.Instance.AvailableBonusLevels / COLUMNS;
            adsButtonPosition = new Vector2(0f, -rowOffset * availableRows / 2f);
            Sprite sprite = new(ClipIds.Menu.McGetMoreLevelsButton)
            {
                Position = adsButtonPosition,
                Scale = 1.45f,
                Color = ContreJourConstants.GreenLightColor
            };
            AddChild(sprite);
            Label label = ContreJourLabelUtil.CreateLabel(20f, "COMPLETE_MORE_CHAPTERS");
            label.Scale *= 0.75f;
            label.Y = 6f;
            sprite.AddChild(label);
            return sprite;
        }

        private TouchSprite CreateGetMoreButton(int chapter)
        {
            TouchSprite touchSprite = new(ClipIds.Menu.McGetMoreLevelsButton);
            Node node;
            if (ContreJourLabelUtil.IsEnglish)
            {
                node = new Sprite(ClipIds.Menu.McGetMoreLevelsText);
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
                touchSprite.Color = Color.Lerp(Color.White, ContreJourConstants.BlueLightColor, 0.7f);
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
            Sprite node = new(ClipIds.Menu.McVenzel)
            {
                Position = button.Position + new Vector2(56f, 0f),
                Scale = 1.15f
            };
            AddChild(node);
            node = new Sprite(ClipIds.Menu.McVenzel)
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
}
