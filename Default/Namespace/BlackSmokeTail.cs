using System;
using System.Collections.Generic;
using System.Linq;

using ContreJour.Content;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class BlackSmokeTail(Body _body, LevelBuilderBase _builder) : IUpdatable
{
    private Body body = _body;

    private LevelBuilderBase builder = _builder;

    private List<object> items = [];

    private Vector2 previousPosition;

    private bool initialized;

    private float startScale = 1f;

    private string clipName = "McTailPart";

    public float StartScale
    {
        get => startScale;
        set => startScale = value;
    }

    public string ClipName
    {
        get => clipName;
        set => clipName = value;
    }

    public void Update(float time)
    {
        //IL_009d: Unknown result type (might be due to invalid IL or missing references)
        List<object> list = [];
        foreach (Sprite item in items.Cast<Sprite>())
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
        if (body.BodyType != 0 && !(body.LinearVelocity.Length() > 0.1f))
        {
            return;
        }
        Vector2 vector = builder.ToPoint(body.Position);
        if (initialized)
        {
            float num = (vector - previousPosition).Length();
            int num2 = (int)Math.Min((float)Math.Ceiling(num / 2f), 15f);
            float num3 = num / num2;
            for (int i = 0; i < num2; i++)
            {
                Node node = ClipTypesCache.CreateNewNode(clipName);
                node.Position = VectorUtil.StepTo(previousPosition, vector, num3 * i);
                float num4 = 1f - (i / (float)num2);
                node.Scale *= startScale;
                node.Scale -= 0.05f * num4 * startScale;
                node.OpacityByte = (int)(150f - (8f * num4));
                builder.Add(node, 3);
                items.Add(node);
            }
        }
        previousPosition = vector;
        initialized = true;
    }
}
