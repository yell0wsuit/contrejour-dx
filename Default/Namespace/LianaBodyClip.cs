using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Default.Namespace;

public class LianaBodyClip : ContreJourBodyClip
{
    protected LianaData data;

    protected List<object> parts;

    protected LianaSprite clipContent;

    public LianaBodyClip(LevelBuilderBase _builder, LianaData data, Node _clip, Hashtable _config)
        : base(_builder, data.Bodies[0], _clip, _config)
    {
        Color black = Color.Black;
        if (_config.ContainsKey("alpha"))
        {
            black.A = (byte)_config.GetInt("alpha");
        }
        clipContent = new LianaSprite(data, black);
        clip = clipContent;
        _builder.Add(clipContent, -3);
        parts = new List<object>();
        for (int i = 1; i < data.Bodies.Count - 1; i++)
        {
            LianaPart item = new LianaPart(data.Bodies[i]);
            parts.Add(item);
        }
    }

    public override void Update(float time)
    {
        base.Update(time);
        foreach (LianaPart part in parts)
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
