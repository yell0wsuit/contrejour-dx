using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Mokus2D.Events;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class SnotPoint : ContreJourBodyClip
{
    private static float Radius = 20f;

    private bool used;

    public bool Enabled = true;

    public readonly EventSender UnuseEvent = new EventSender();

    public bool Used
    {
        get
        {
            return used;
        }
        set
        {
            if (used != value)
            {
                used = value;
                if (!used)
                {
                    UnuseEvent.SendEvent();
                }
            }
        }
    }

    public SnotPoint(LevelBuilderBase _builder, object _body, Node _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
        Body val = Body;
        builder.World.RemoveBody(Body);
        Create(val.Position);
    }

    public SnotPoint(LevelBuilderBase _builder, Vector2 position, Node _clip, Hashtable _config)
        : base(_builder, null, _clip, _config)
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
        base.Game.SnotPoints.Add(this);
        clip = new Sprite("chapter6/McSnotPoint");
        builder.AddChild(clip, 3);
    }
}
