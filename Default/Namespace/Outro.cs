using System;

using ContreJour.Utils;

using Microsoft.Xna.Framework;

using Mokus2D;
using Mokus2D.Input;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Text;

namespace Default.Namespace;

public class Outro : Node, ITouchListener, IDisposable
{
    private static readonly float SPEED = ContreJourLabelUtil.IsAsian ? 30 : 15;

    private readonly LayerColor background;

    private Vector2 creditsPosition;

    private Touch currentTouch;

    private readonly float margins;

    private readonly float maxY;

    private readonly float minY;

    private float onEndTime;

    private readonly Node text;

    protected Vector2 textPosition;

    private bool textVisible;

    private Vector2 touchPosition;

    private float touchSpeed;

    public bool TextVisible
    {
        get => textVisible;
        set
        {
            if (textVisible != value)
            {
                textVisible = value;
                text.Tweener.Stop();
                _ = text.FadeTo(2f, value ? 1f : 0f);
            }
        }
    }

    public Outro(bool success)
    {
        Mokus2DGame.Instance.TouchController.AddListener(this);
        minY = -150f;
        margins = 20f;
        maxY = 760f;
        background = new LayerColor(Color.Black, "menu/whitePixel")
        {
            OpacityByte = 0
        };
        _ = background.FadeTo(2f, 0.5882353f);
        AddChild(background);
        text = new Node();
        textPosition = ScreenConstants.W7FromIPhoneScreenCenter;
        textPosition.Y = minY;
        text.Position = textPosition;
        AddChild(text);
        Label label = ContreJourLabelUtil.CreateMultilineLabel(22f, success ? "YOU_DID_IT" : "YOU_TRIED_HARD");
        label.Position = new Vector2(0f, 160f);
        label.Color = Color.White;
        text.AddChild(label);
        Label label2 = ContreJourLabelUtil.CreateMultilineLabel(16f, "CREDITS");
        label2.Position = new Vector2(0f, -348f);
        label2.Color = Color.White;
        text.AddChild(label2);
        creditsPosition = new Vector2(0f, -390f);
        AddCreditsRight("GRAPHIC_ARTISTS", "MIHAI_TYMOSHENKO");
        AddCreditsRight(null, "ANDRIY_SHVUREV");
        AddCreditsRight("COMPOSER", "DAVID_LEON");
        AddCreditsRight("SFX_BY", "IHOR_PRYSHLIAK");
        AddCreditsRight("PRODUCER", "TOM_KINNINBUGRH");
        AddCreditsRight("IDEA", "ANTON_MYKHAYLETS");
        AddCreditsRight("EVERYTHING_ELSE", "MAKSYM_HRYNIV");
        Label label3 = ContreJourLabelUtil.CreateLabel(16f, "STARS");
        label3.Position = creditsPosition;
        label3.Y += 10f;
        label3.Color = Color.White;
        text.AddChild(label3);
        textVisible = true;
        onEndTime = 0f;
    }

    public void AddCreditsRight(string left, string right)
    {
        if (left != null)
        {
            Label label = ContreJourLabelUtil.CreateLabel(16f, left);
            label.Color = Color.White;
            label.Anchor = new Vector2(1f, 0.5f);
            label.Position = creditsPosition + new Vector2(0f - margins, 0f);
            text.AddChild(label);
        }
        if (right != null)
        {
            Label label2 = ContreJourLabelUtil.CreateLabel(16f, right);
            label2.Color = Color.White;
            label2.Anchor = new Vector2(0f, 0.5f);
            label2.Position = creditsPosition + new Vector2(margins, 0f);
            text.AddChild(label2);
        }
        NextLine();
    }

    public bool TouchBegin(Touch touch)
    {
        if (currentTouch != null)
        {
            return false;
        }
        touchSpeed = 0f;
        currentTouch = touch;
        touchPosition = GlobalToLocal(touch.Position);
        return true;
    }

    public bool TouchMove(Touch touch)
    {
        RefreshTouchPosition(touch);
        return true;
    }

    public void RefreshTouchPosition(Touch touch)
    {
        if (touch == currentTouch)
        {
            Vector2 vector = GlobalToLocal(currentTouch.Position);
            touchSpeed = Maths.Clamp(vector.Y - touchPosition.Y, -100f, 100f);
            textPosition.Y += vector.Y - touchPosition.Y;
            touchPosition = vector;
        }
    }

    public void TouchEnd(Touch touch)
    {
        if (touch == currentTouch)
        {
            currentTouch = null;
        }
    }

    public void NextLine()
    {
        creditsPosition += new Vector2(0f, -30f);
    }

    public void BackLine()
    {
        creditsPosition += new Vector2(0f, 30f);
    }

    public override void Update(float time)
    {
        if (currentTouch == null)
        {
            if (textPosition.Y < maxY)
            {
                textPosition += new Vector2(0f, SPEED * time);
            }
            textPosition.Y += touchSpeed * time * 30f;
            touchSpeed = Maths.StepTo(touchSpeed, 0f, Math.Max(1f, Math.Abs(touchSpeed / 20f)));
            float num = 0f;
            if (textPosition.Y < minY)
            {
                num = (minY - textPosition.Y) / 5f;
            }
            if (textPosition.Y > maxY)
            {
                num = (maxY - textPosition.Y) / 5f;
            }
            if (textPosition.Y >= maxY)
            {
                onEndTime += time;
            }
            if (num != 0f)
            {
                textPosition.Y += num;
                touchSpeed = Maths.StepTo(touchSpeed, 0f, Math.Max(1f, Math.Abs(touchSpeed / 20f)));
            }
        }
        else
        {
            onEndTime = 0f;
        }
        TextVisible = onEndTime <= 5f;
        text.Position = textPosition;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Mokus2DGame.Instance.TouchController.RemoveListener(this);
    }
}
