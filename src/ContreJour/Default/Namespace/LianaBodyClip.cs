using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Default.Namespace;

public class LianaBodyClip : ContreJourBodyClip
{
    private readonly List<object> parts;

    private readonly LianaSprite clipContent;

    public LianaBodyClip(LevelBuilderBase builder, LianaData data, Node clip, Hashtable config)
        : base(builder, data.Bodies[0], clip, config)
    {
        Color black = Color.Black;
        if (config.ContainsKey("alpha"))
        {
            black.A = (byte)config.GetInt("alpha");
        }
        clipContent = new LianaSprite(data, black);
        Clip = clipContent;
        builder.Add(clipContent, -3);
        parts = [];
        for (int i = 1; i < data.Bodies.Count - 1; i++)
        {
            LianaPart item = new(data.Bodies[i]);
            parts.Add(item);
        }
    }

    public override void Update(float time)
    {
        base.Update(time);
        foreach (LianaPart part in parts.Cast<LianaPart>())
        {
            part.Update(time);
        }
        clipContent.Update(time);
    }

    public override void UpdatePosition()
    {
    }

    public override void UpdateRotation()
    {
    }
}
