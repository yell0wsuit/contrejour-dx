using System;
using System.Collections.Generic;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Interfaces;
using Mokus2D.Util;
using Mokus2D.Util.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Util;

namespace Default.Namespace;

public class BlackTail : PrimitivesNode, IUpdatable
{
    protected VertexPositionColorTexture[] vertices = [];

    private readonly List<Vector2> bezierPoints = [];
    private Vector2 target;

    private readonly LevelBuilderBase builder;

    private Vector2 currentPosition;

    private Vector2 previousPosition;

    private Vector2 previousCenter;

    private Vector2 currentCenter;

    private readonly List<int> removeFrames = [];

    private int currentFrame;
    private bool opacityDirty;

    public float Width { get; set; }

    public int Frames { get; set; }

    public Body Body { get; set; }

    public Vector2 Target
    {
        get => target;
        set => target = value;
    }

    public override float OpacityFloat
    {
        set
        {
            if (value != OpacityFloat)
            {
                base.OpacityFloat = value;
                opacityDirty = true;
            }
        }
    }

    public bool Moving
    {
        get; set
        {
            if (field != value)
            {
                field = value;
            }
        }
    }

    public int Length => bezierPoints.Count;

    public BlackTail(Body body, ContreJourLevelBuilder builder)
        : this(body, builder, builder.ContreJour.BonusChapter ? "McTailTextureGreen" : "McTailTexture")
    {
    }

    public BlackTail(Body body, LevelBuilderBase builder, string textureFile)
    {
        UpdateEnabled = false;
        Body = body;
        this.builder = builder;
        Frames = 40;
        previousPosition = builder.ToPoint(Body.Position);
        currentFrame = 0;
        Width = 40f;
        Texture = ClipFactory.GetTexture(textureFile);
    }

    public BlackTail(BodyClip clip, string textureFile)
        : this(clip.Body, clip.Builder, textureFile)
    {
    }

    public BlackTail(ContreJourBodyClip clip)
        : this(clip.Body, (ContreJourLevelBuilder)clip.Builder)
    {
    }

    public override void Update(float time)
    {
        currentPosition = builder.ToPoint((Body != null) ? Body.Position : target);
        bool flag = true;
        if (previousPosition != Vector2.Zero)
        {
            currentCenter = currentPosition.Middle(previousPosition);
            if (previousCenter != Vector2.Zero)
            {
                float num = currentCenter.DistanceTo(previousCenter);
                flag = num > 1f;
                if (flag)
                {
                    int segments = (int)Math.Ceiling(num / 3f);
                    List<Vector2> list = [];
                    BezierUtil.GetBezierPoints(previousCenter, previousPosition, currentCenter, segments, insertLast: false, list);
                    for (int i = 0; i < list.Count; i++)
                    {
                        removeFrames.Insert(0, currentFrame);
                    }
                    if (bezierPoints.Count == 0)
                    {
                        bezierPoints.Resize(1);
                    }
                    bezierPoints[0] = currentPosition;
                    bezierPoints.InsertRange(1, new ReverseCollection<Vector2>(list));
                }
            }
            else
            {
                removeFrames.Insert(0, currentFrame);
                bezierPoints.Insert(0, previousPosition);
            }
            List<Pair<Vector2>> list2 = CreatePairs();
            Array.Resize(ref vertices, list2.Count * 2);
            if (list2.Count > 1)
            {
                for (int j = 0; j < list2.Count; j++)
                {
                    int num2 = j * 2;
                    vertices[num2].Position = list2[j].First.ToVector3();
                    vertices[num2].TextureCoordinate = new Vector2(j, 0f);
                    vertices[num2 + 1].Position = list2[j].Second.ToVector3();
                    vertices[num2 + 1].TextureCoordinate = new Vector2(j, 1f);
                    vertices[num2].Color = GetTailColor();
                    vertices[num2 + 1].Color = GetTailColor();
                }
            }
        }
        if (flag)
        {
            previousCenter = currentCenter;
            previousPosition = currentPosition;
        }
        RemoveTail();
        currentFrame++;
    }

    public int FramesToLive()
    {
        return Frames;
    }

    public void RemoveTail()
    {
        int num = currentFrame - FramesToLive();
        while (removeFrames.Count > 0 && removeFrames.Last() <= num)
        {
            _ = removeFrames.RemoveLast();
            _ = bezierPoints.RemoveLast();
        }
    }

    public List<Pair<Vector2>> CreatePairs()
    {
        List<Pair<Vector2>> list = [];
        int num = bezierPoints.Count + 1;
        for (int num2 = bezierPoints.Count - 1; num2 >= 0; num2--)
        {
            Vector2 start = (num2 == bezierPoints.Count - 1) ? bezierPoints[^1] : bezierPoints[num2 + 1];
            Vector2 end = (num2 == 0) ? currentPosition : bezierPoints[num2 - 1];
            float num3 = Width * (1f - ((num2 + 1) / (float)num));
            if (num3 > 1f)
            {
                list.Add(ContreDrawUtil.GetPointsPair(bezierPoints[num2], start, end, num3));
            }
        }
        return list;
    }

    protected override void DrawPrimitives()
    {
        if (vertices.Length > 3)
        {
            if (opacityDirty)
            {
                GraphUtil.SetColor(vertices, GetTailColor());
            }
            GraphUtil.DrawTriangleStrip(vertices);
        }
    }

    private Color GetTailColor()
    {
        return new Color(OpacityFloat, OpacityFloat, OpacityFloat, OpacityFloat);
    }
}
