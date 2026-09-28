using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Util;
using Mokus2D.Util.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Interfaces;
using Mokus2D.Visual.Primitives;
using Mokus2D.Visual.Util;

namespace ContreJour.Primitives;

public abstract class LongNeckSprite : PrimitivesNode
{
    private bool created;

    protected VertexPositionColorTexture[] vertices;

    protected VertexPositionColorTexture[] border;

    private Color neckColor;

    private Color drawNeckColor;

    protected float borderWidth;

    protected int allPointsSize;

    private Rectangle textureRect = new(0, 0, 0, 0);

    private readonly TextureCoords textureCoords = new();

    private float textureStep = 1f;

    protected bool drawBorder = true;

    private readonly List<Vector2> first = new(64);

    private readonly List<Vector2> second = new(64);

    private readonly List<Vector2> firstBezier = new(64);

    private readonly List<Vector2> secondBezier = new(64);

    private readonly List<Vector2> allPoints = [];

    private readonly List<Pair<Vector2>> cachedPairs = new(64);

    protected virtual bool HasRecalculateVertices => true;

    protected virtual bool OnScreen => true;

    public ISpriteData TextureData
    {
        set
        {
            Texture = value.Texture;
            textureRect = value.TextureRect;
            textureCoords.Refresh(Texture, value.TextureRect, Vector2.One);
            if (Texture != null)
            {
                NeckColor = Color.White;
            }
        }
    }

    public override Texture2D Texture
    {
        get => base.Texture;
        set
        {
            if (value != null)
            {
                textureRect = new Rectangle(0, 0, value.Width, value.Height);
            }
            base.Texture = value;
            textureCoords.Refresh(Texture, textureRect);
        }
    }

    public Color NeckColor
    {
        get => neckColor;
        set
        {
            neckColor = value;
            if (border != null && drawBorder)
            {
                SetBorderColors();
            }
            if (vertices != null)
            {
                SetNeckColors();
            }
        }
    }

    protected LongNeckSprite()
    {
        neckColor = new Color(0, 0, 0, 255);
        borderWidth = 2f;
    }

    public abstract void GetPairs(List<Pair<Vector2>> target);

    public override void Update(float time)
    {
        if (HasRecalculateVertices && OnScreen)
        {
            RecalculateVertices();
        }
    }

    private void RecalculateVertices()
    {
        cachedPairs.Clear();
        GetPairs(cachedPairs);
        if (cachedPairs.Count <= 2)
        {
            return;
        }
        first.Clear();
        second.Clear();
        foreach (Pair<Vector2> cachedPair in cachedPairs)
        {
            first.Add(cachedPair.First);
            second.Add(cachedPair.Second);
        }
        firstBezier.Clear();
        secondBezier.Clear();
        AddBezierPointsBezier(first, firstBezier);
        AddBezierPointsBezier(second, secondBezier);
        CreatePolygonsFirstBezierSecondBezier(firstBezier, secondBezier);
    }

    public virtual void AddBezierPointsBezier(List<Vector2> source, List<Vector2> bezier)
    {
        BezierUtil.AddBezierPoints(bezier, source, 6);
    }

    public void CreatePolygonsFirstBezierSecondBezier(List<Vector2> firstBezier, List<Vector2> secondBezier)
    {
        ProcessBezierSecond(firstBezier, secondBezier);
        for (int i = 0; i < firstBezier.Count - 1; i++)
        {
            int num = i * 6;
            vertices[num].Position = firstBezier[i].ToVector3();
            vertices[num + 1].Position = firstBezier[i + 1].ToVector3();
            vertices[num + 2].Position = secondBezier[i].ToVector3();
            vertices[num + 3].Position = secondBezier[i].ToVector3();
            vertices[num + 4].Position = secondBezier[i + 1].ToVector3();
            vertices[num + 5].Position = firstBezier[i + 1].ToVector3();
            if (Texture != null)
            {
                RefreshTextureCoords(i, num);
            }
        }
    }

    protected virtual void RefreshTextureCoords(int i, int start)
    {
        float num = i * textureStep;
        float num2 = (i + 1) * textureStep;
        if (num2 > 1f)
        {
            num = (float)((double)num - Math.Floor(num2));
            num2 = (float)((double)num2 - Math.Floor(num2));
        }
        if (num < 0f)
        {
            num += 1f;
            num2 += 1f;
        }
        Vector2 texturePosition = textureCoords.GetTexturePosition(new Vector2(num, 0f));
        Vector2 texturePosition2 = textureCoords.GetTexturePosition(new Vector2(num, 1f));
        Vector2 texturePosition3 = textureCoords.GetTexturePosition(new Vector2(num2, 0f));
        Vector2 texturePosition4 = textureCoords.GetTexturePosition(new Vector2(num2, 1f));
        vertices[start].TextureCoordinate = texturePosition;
        vertices[start + 1].TextureCoordinate = texturePosition3;
        vertices[start + 2].TextureCoordinate = texturePosition2;
        vertices[start + 3].TextureCoordinate = texturePosition2;
        vertices[start + 4].TextureCoordinate = texturePosition4;
        vertices[start + 5].TextureCoordinate = texturePosition3;
    }

    public void ProcessBezierSecond(List<Vector2> firstBezier, List<Vector2> secondBezier)
    {
        allPoints.Clear();
        allPoints.Capacity = firstBezier.Count + secondBezier.Count;
        allPoints.AddItemsNoGarbage(secondBezier);
        allPoints.AddItemsNoGarbage(firstBezier, firstBezier.Count - 1, 0);
        TryCreateVectors(allPoints);
        GraphUtil.CreateGradientBorderWidthVertices(allPoints, borderWidth, border);
    }

    public void TryCreateVectors(List<Vector2> allPoints)
    {
        if (!created)
        {
            CreateVectors(allPoints.Count);
            created = true;
        }
    }

    public virtual void CreateVectors(int allPointsSize)
    {
        vertices = new VertexPositionColorTexture[(allPointsSize - 2) * 3];
        this.allPointsSize = allPointsSize;
        border = new VertexPositionColorTexture[this.allPointsSize * 6];
        SetBorderColors();
        SetNeckColors();
    }

    protected virtual void SetNeckColors()
    {
        if (vertices != null)
        {
            GraphUtil.SetColor(vertices, drawNeckColor);
        }
    }

    public virtual void SetBorderColors()
    {
        if (border != null && drawBorder)
        {
            GraphUtil.CreateGradientColorsList(allPointsSize, drawNeckColor, EndColor(), border);
        }
    }

    public virtual Color EndColor()
    {
        return drawNeckColor.ChangeAlpha(0);
    }

    public override void Draw(VisualState state)
    {
        if (OnScreen)
        {
            Color color = neckColor.ChangeAlpha((byte)(neckColor.A * state.Opacity));
            if (color != drawNeckColor)
            {
                drawNeckColor = color;
                SetBorderColors();
                SetNeckColors();
            }
            base.Draw(state);
        }
    }

    protected override void DrawPrimitives()
    {
        DrawPolygons();
        DrawBorder();
    }

    public virtual void DrawBorder()
    {
        if (border != null && drawBorder)
        {
            GraphUtil.DrawTriangleList(border);
        }
    }

    public virtual void DrawPolygons()
    {
        if (vertices != null)
        {
            GraphUtil.DrawTriangleList(vertices);
        }
    }
}
