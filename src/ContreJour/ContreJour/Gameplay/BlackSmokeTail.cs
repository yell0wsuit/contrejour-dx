using System;
using System.Collections.Generic;
using System.Linq;

using ContreJour.Content;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJour.Gameplay;

public class BlackSmokeTail(Body body, LevelBuilderBase builder) : IUpdatable
{
    private readonly Body body = body;

    private readonly LevelBuilderBase builder = builder;

    private readonly List<object> items = [];

    private Vector2 previousPosition;

    private bool initialized;

    public float StartScale { get; set; } = 1f;

    public string ClipName { get; set; } = "McTailPart";

    public void Update(float time)
    {
        //IL_009d: Unknown result type (might be due to invalid IL or missing references)
        List<object> list = [];
        foreach (Sprite item in items.Cast<Sprite>())
        {
            item.Scale -= 0.05f * StartScale;
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
                Node node = ClipTypesCache.CreateNewNode(ClipName);
                node.Position = VectorUtil.StepTo(previousPosition, vector, num3 * i);
                float num4 = 1f - (i / (float)num2);
                node.Scale *= StartScale;
                node.Scale -= 0.05f * num4 * StartScale;
                node.OpacityByte = (int)(150f - (8f * num4));
                builder.Add(node, 3);
                items.Add(node);
            }
        }
        previousPosition = vector;
        initialized = true;
    }
}
