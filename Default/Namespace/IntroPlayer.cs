using System.Collections.Generic;

using ContreJour.Clips.level1;
using ContreJour.Utils;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Text;

namespace Default.Namespace;

public class IntroPlayer : Node
{
    private static readonly Color LAST_COLOR = new(16, 16, 16);

    protected ContreJourGame game;

    protected List<string> messages;

    protected List<string> rightMessages;

    protected Vector2 textPosition;

    public IntroPlayer(ContreJourGame _game)
    {
        game = _game;
        textPosition = new Vector2(0.5f * game.LevelSize.X, 0.7f * game.LevelSize.Y);
        _ = this.Schedule(2f, PlayItem);
        messages = ["MUSIC_BY", "GRAPHICS_BY", "DIRECTED_BY"];
        rightMessages = ["DAVID_LEON", "MIHAI_MAKSYM", "BY_MAKSYM_HRYNIV"];
    }

    private void PlayItem()
    {
        if (messages.Count > 0)
        {
            ShowMessageRightMessageIndex(messages.Last(), rightMessages.Last(), 4 - messages.Count);
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
        _ = this.Schedule(6f, PlayLogo);
    }

    private void PlayLogo()
    {
        McIntroLogo sprite = new();
        AddChild(sprite);
        sprite.Position = textPosition;
        FadeItemShowTime(sprite, 6f);
    }

    public void ShowMessageRightMessageIndex(string message, string rightMessage, int index)
    {
        Color gREY_COLOR = ContreJourConstants.GREY_COLOR;
        Label label = ContreJourLabelUtil.CreateMultilineLabel(15f, message);
        label.AnchorX = 1f;
        label.Color = gREY_COLOR;
        label.Position = new Vector2(-10f, 0f);
        Label label2 = ContreJourLabelUtil.CreateMultilineLabel(15f, rightMessage);
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
