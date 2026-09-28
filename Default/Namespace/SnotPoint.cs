using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Events;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class SnotPoint : ContreJourBodyClip
{
    private static readonly float Radius = 20f;
    public bool Enabled = true;

    public readonly EventSender UnuseEvent = new();

    public bool Used
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                if (!field)
                {
                    UnuseEvent.SendEvent();
                }
            }
        }
    }

    public SnotPoint(LevelBuilderBase builder, object body, Node clip, Hashtable config)
        : base(builder, body, clip, config)
    {
        Body val = Body;
        this.builder.World.RemoveBody(Body);
        Create(val.Position);
    }

    public SnotPoint(LevelBuilderBase builder, Vector2 position, Node clip, Hashtable config)
        : base(builder, null, clip, config)
    {
        Create(position);
    }

    public override void Update(float time)
    {
        base.Update(time);
        clip.OpacityFloat = clip.OpacityFloat.StepTo(Enabled ? 1f : 0.2f, 0.05f);
    }

    private void Create(Vector2 position)
    {
        Body = builder.World.CreateCircle(Radius * builder.SizeMult, position);
        Body.SetSensor(value: true);
        Game.SnotPoints.Add(this);
        clip = new Sprite("chapter6/McSnotPoint");
        _ = builder.AddChild(clip, 3);
    }
}
