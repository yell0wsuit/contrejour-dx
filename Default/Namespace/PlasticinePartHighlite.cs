using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Interfaces;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace Default.Namespace;

public class PlasticinePartHighlite : IUpdatable
{
    protected PlasticinePartBodyClip plasticine;

    protected LevelBuilderBase builder;

    protected ContreJourGame game;

    protected PlasticineHighliteBorder parent;

    protected int index;

    protected bool dirty;

    protected float lightLength;

    protected Vector2 lightBottom;

    protected bool hasLight;

    protected bool highliteSet;

    protected Color noLightBorderOut;

    private static readonly Color NO_LIGHT_BORDER_OUT_BLUE = ContreJourConstants.BLUE_LIGHT_COLOR.ChangeAlpha(0);

    public static readonly Color NO_LIGHT_BORDER_OUT = new(0, 0, 0, 0);

    public float LightLength
    {
        get => lightLength;
        set => lightLength = value;
    }

    public Vector2 LightBottom
    {
        get => lightBottom;
        set => lightBottom = value;
    }

    public bool HasLight
    {
        get => hasLight;
        set => hasLight = value;
    }

    private bool MirrorLight => game.WhiteSide || game.BlackSide || game.BonusChapter;

    public PlasticinePartHighlite(PlasticinePartBodyClip plasticine, PlasticineHighliteBorder parent, int index)
    {
        this.plasticine = plasticine;
        builder = this.plasticine.Builder;
        game = (ContreJourGame)builder.Game;
        plasticine.Highlite = this;
        this.parent = parent;
        this.index = index;
        noLightBorderOut = game.BlackSide ? NO_LIGHT_BORDER_OUT_BLUE : NO_LIGHT_BORDER_OUT;
        noLightBorderOut = NO_LIGHT_BORDER_OUT;
    }

    public void SetDirty()
    {
        dirty = true;
    }

    private void RefreshPositions()
    {
        float num = Math.Abs((VectorUtil.Atan2(NextBodyClip().Body.Position, game.LightPoint) - ((float)Math.PI / 2f)).SimplifyAngle(NextBodyClip().Body.Rotation - (float)Math.PI) - NextBodyClip().Body.Rotation);
        hasLight = num < 1.3463969f;
        if (MirrorLight && num > (float)Math.PI / 2f)
        {
            num = (float)Math.PI - num;
            hasLight = num < 1.3463969f;
        }
        lightLength = 0f;
        if (hasLight)
        {
            lightLength = 1.2f * Math.Max(1f - (num / 1.3463969f), 0f);
        }
        if (MirrorLight)
        {
            lightLength = Math.Max(lightLength, 0.1f);
            hasLight = true;
        }
        Vector2 vector = new(0f, 0f - lightLength + (7f / 12f));
        Vector2 worldPoint = NextBodyClip().Body.GetWorldPoint(vector);
        lightBottom = builder.ToPoint(worldPoint);
    }

    public void Update(float time)
    {
        if (dirty)
        {
            RefreshPositions();
        }
    }

    public void TryRefresh()
    {
        bool flag = !highliteSet || game.LightPowerChanged;
        if (dirty || flag)
        {
            RefreshBorderColors();
            highliteSet = true;
        }
        if (dirty)
        {
            Refresh();
            dirty = false;
        }
    }

    public void RefreshBorderColors()
    {
        bool flag = PreviousHighlite().HasLight;
        VertexPositionColor[] vertices = parent.Vertices;
        VertexPositionColor[] array = parent.OutBorder();
        Color mainColor = parent.MainColor;
        LightColor lightColor = game.LightColor;
        if (plasticine.Index == 0)
        {
            vertices[0].Color = array[0].Color = flag ? lightColor.LightOutColor : mainColor;
            vertices[1].Color = flag ? lightColor.LightInColor : mainColor;
            array[1].Color = flag ? lightColor.LightBorderColor : noLightBorderOut;
        }
        vertices[index].Color = array[index].Color = flag ? lightColor.LightOutColor : mainColor;
        vertices[index + 1].Color = flag ? lightColor.LightInColor : mainColor;
        array[index + 1].Color = flag ? lightColor.LightBorderColor : noLightBorderOut;
        vertices[index + 2].Color = array[index + 2].Color = hasLight ? lightColor.LightOutColor : mainColor;
        vertices[index + 3].Color = hasLight ? lightColor.LightInColor : mainColor;
        array[index + 3].Color = hasLight ? lightColor.LightBorderColor : noLightBorderOut;
    }

    public PlasticinePartHighlite PreviousHighlite()
    {
        return Previous().BodyClip.Highlite;
    }

    public PlasticinePartHighlite NextHighlite()
    {
        return plasticine.Item.NextItem.BodyClip.Highlite;
    }

    public PlasticineItem Previous()
    {
        return plasticine.Item.PreviousItem;
    }

    public PlasticineItem Next()
    {
        return plasticine.Item.NextItem;
    }

    public PlasticinePartBodyClip NextBodyClip()
    {
        return plasticine.Item.NextItem.BodyClip;
    }

    public void Refresh()
    {
        VertexPositionColor[] vertices = parent.Vertices;
        VertexPositionColor[] inBorder = parent.InBorder;
        bool flag = PreviousHighlite().HasLight;
        if (plasticine.Index == 0)
        {
            vertices[0].Position = inBorder[0].Position;
            vertices[1].Position = flag ? PreviousHighlite().LightBottom.ToVector3() : vertices[0].Position;
        }
        vertices[index].Position = inBorder[index].Position;
        vertices[index + 2].Position = inBorder[index + 2].Position;
        vertices[index + 1].Position = flag ? PreviousHighlite().LightBottom.Middle(lightBottom).ToVector3() : vertices[index].Position;
        vertices[index + 3].Position = hasLight ? lightBottom.ToVector3() : vertices[index + 2].Position;
    }
}
