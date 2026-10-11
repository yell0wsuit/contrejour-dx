using System.Collections.Generic;
using System.Numerics;

using ContreJourDX.Clips;
using ContreJourDX.Utils;

using Mokus2D.Graphics;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Text;

namespace ContreJourDX.Gameplay
{
    public class IntroPlayer : Node
    {
        private readonly ContreJourDXGame game;

        private readonly List<string> messages;

        private readonly List<string> rightMessages;

        private Vector2 textPosition;

        public IntroPlayer(ContreJourDXGame game)
        {
            this.game = game;
            textPosition = new Vector2(0.5f * this.game.LevelSize.X, 0.7f * this.game.LevelSize.Y);
            _ = this.Schedule(2f, PlayItem);
            messages = ["MUSIC_BY", "GRAPHICS_BY", "DIRECTED_BY"];
            rightMessages = ["DAVID_LEON", "MIHAI_MAKSYM", "BY_MAKSYM_HRYNIV"];
        }

        private void PlayItem()
        {
            if (messages.Count > 0)
            {
                ShowMessageRightMessageIndex(messages[^1], rightMessages[^1]);
                _ = messages.RemoveLast();
                _ = rightMessages.RemoveLast();
                _ = this.Schedule(3.75f, PlayItem);
            }
            else
            {
                PlayInspired();
            }
        }

        public void PlayInspired()
        {
            Label label = ContreJourDXLabelUtil.CreateMultilineLabel(15f, "INSPIRED_BY");
            label.Color = Color.Black;
            label.Position = textPosition + new Vector2(-40f, 0f);
            AddChild(label);
            FadeItem(label);
            if (ContreJourDXLabelUtil.IsEnglish)
            {
                Sprite inspired = new(ClipIds.Level1.McInspired)
                {
                    Position = textPosition
                };
                AddChild(inspired);
                FadeItem(inspired);
            }
            _ = this.Schedule(6f, PlayLogo);
        }

        private void PlayLogo()
        {
            Sprite sprite = new(ClipIds.Level1.McIntroLogo);
            AddChild(sprite);
            sprite.Position = textPosition;
            FadeItemShowTime(sprite, 6f);
        }

        public void ShowMessageRightMessageIndex(string message, string rightMessage)
        {
            Color gREY_COLOR = ContreJourDXConstants.GreyColor;
            Label label = ContreJourDXLabelUtil.CreateMultilineLabel(15f, message);
            label.AnchorX = 1f;
            label.Color = gREY_COLOR;
            label.Position = new Vector2(-10f, 0f);
            Label label2 = ContreJourDXLabelUtil.CreateMultilineLabel(15f, rightMessage);
            label2.Position = new Vector2(10f, 0f);
            label2.Color = gREY_COLOR;
            label2.AnchorX = 0f;
            Node node = new();
            AddChild(node);
            node.Position = textPosition;
            node.AddChild(label);
            node.AddChild(label2);
            FadeItem(label);
            FadeItem(label2);
        }

        public static void FadeItemShowTime(Node item, float time)
        {
            item.OpacityFloat = 0f;
            _ = item.Tweener.StartSequence(0.8f).Tween(NodeValues.OpacityFloat, 1f).Next(time)
                .Next(0.8f)
                .Tween(NodeValues.OpacityFloat, 0f)
                .OnComplete(NodeValues.Hide);
        }

        public static void FadeItem(Node item)
        {
            FadeItemShowTime(item, 1.5f);
        }
    }
}
