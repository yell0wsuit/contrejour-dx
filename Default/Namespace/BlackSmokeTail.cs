using System;
using System.Collections.Generic;

using ContreJour.Content;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class BlackSmokeTail : IUpdatable
{
    private const float START_OPACITY = 150f;

    private const float MAX_DISTANCE = 2f;

    private const float MIN_SPEED = 0.1f;

    private const int OPACITY_DIFF = 8;

    private const float SCALE_DIFF = 0.05f;

    protected Body body;

    protected LevelBuilderBase builder;

    protected List<object> items;

    protected Vector2 previousPosition;

    protected bool initialized;

    protected float startScale;

    protected string clipName;

    public float StartScale
    {
        get
        {
            return startScale;
        }
        set
        {
            startScale = value;
        }
    }

    public string ClipName
    {
        get
        {
            return clipName;
        }
        set
        {
            clipName = value;
        }
    }

    public BlackSmokeTail(Body _body, LevelBuilderBase _builder)
    {
        body = _body;
        builder = _builder;
        items = new List<object>();
        startScale = 1f;
        clipName = "McTailPart";
    }

    public void Update(float time)
    {
        //IL_009d: Unknown result type (might be due to invalid IL or missing references)
        List<object> list = new List<object>();
        foreach (Sprite item in items)
        {
            item.Scale -= 0.05f * startScale;
            item.OpacityByte -= 8;
            if (item.OpacityByte <= 0 || item.Scale <= 0f)
            {
                list.Add(item);
                builder.RemoveChild(item);
            }
        }
        items.RemoveList(list);
        if ((int)body.BodyType != 0 && !(body.LinearVelocity.Length() > 0.1f))
        {
            return;
        }
        Vector2 vector = builder.ToPoint(body.Position);
        if (initialized)
        {
            float num = (vector - previousPosition).Length();
            int num2 = (int)Math.Min((float)Math.Ceiling(num / 2f), 15f);
            float num3 = num / (float)num2;
            for (int i = 0; i < num2; i++)
            {
                Node node = ClipTypesCache.CreateNewNode(clipName);
                node.Position = VectorUtil.StepTo(previousPosition, vector, num3 * (float)i);
                float num4 = 1f - (float)i / (float)num2;
                node.Scale *= startScale;
                node.Scale -= 0.05f * num4 * startScale;
                node.OpacityByte = (int)(150f - 8f * num4);
                builder.Add(node, 3);
                items.Add(node);
            }
        }
        previousPosition = vector;
        initialized = true;
    }
}
