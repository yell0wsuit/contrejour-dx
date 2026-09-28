using System;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

using Microsoft.Xna.Framework;

using Mokus2D.Input;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class StrongSnotBodyClip : SnotBodyClip
{
    private const float JOIN_DISTANCE = 1.3333334f;

    private const float COLOR_STEP = 20f;

    private const float RELEASE_TIME = 0.6f;

    private const float MAX_STRETCHING = 30f;

    private const float MAX_DISTANCE_MULT = 1.25f;

    private const float MOUSE_FORCE = 100f;

    private FixedMouseJoint dragJoint;

    protected float extremeSnotDistance;

    protected float maxSnotDistance;

    protected float normalDistance;

    protected float targetColor;

    protected float timeToRelease;

    public float NormalDistance => normalDistance;

    public new Vector2 Position => Physics.FirstBody.Position;

    public StrongSnotBodyClip(LevelBuilderBase _builder, SnotData _body, Node _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
        normalDistance = CurrentDistance();
        maxSnotDistance = Math.Max(normalDistance * 1.25f, normalDistance + (30f * builder.EngineConfig.SizeMultiplier));
        extremeSnotDistance = normalDistance * 2f;
        targetColor = 255f;
    }

    public override string[] OnSound()
    {
        return Sounds.ROPE_ON;
    }

    public override float JoinDistance()
    {
        return 1.3333334f;
    }

    public override void CreateTail()
    {
    }

    public override void CreateHighlite(ContreJourGame _game)
    {
    }

    public override void InitSizes()
    {
        base.InitSizes();
        centerWidth = 10f * builder.EngineConfig.SizeMultiplier;
    }

    public void EnsureSpeedY(float value)
    {
        for (int i = 0; i < Physics.BodiesSize(); i++)
        {
            Body val = Physics.BodyAt(i);
            if (val.LinearVelocity.Y > value)
            {
                val.LinearVelocity = new Vector2(val.LinearVelocity.X, value);
            }
        }
    }

    public override string BaseEndClipName()
    {
        return game.ChooseSide("McStrongSnotEndBlack", "McStrongSnotEndWhite", "McStrongSnotEnd", "McStrongSnotEnd", "McSnotEnd_6");
    }

    public override SnotSprite CreateClip()
    {
        return new TextureSnotSprite((ContreJourGame)builder.Game, this, startWidth, centerWidth, endWidth);
    }

    public float CurrentDistance()
    {
        return (Physics.FirstBody.Position - Physics.EndBody.Position).Length();
    }

    public override float JoinedDamping()
    {
        return 0f;
    }

    protected override void UpdateDragBodyPosition(BodyAndPoint target, float time)
    {
        base.UpdateDragBodyPosition(target, time);
        Body val = Physics.EndBody;
        int num = Physics.BodiesSize() - 2;
        while (num >= 0)
        {
            Body val2 = Physics.BodyAt(num);
            if ((val.Position - val2.Position).Length() > Physics.Metrics.PartSize * 1.2f)
            {
                val2.Position = val2.Position.ClampDistance(val.Position, Physics.Metrics.PartSize * 1.2f);
                val = val2;
                num--;
                continue;
            }
            break;
        }
    }

    public override bool TouchBegan(Touch touch)
    {
        //IL_0025: Unknown result type (might be due to invalid IL or missing references)
        //IL_002f: Expected O, but got Unknown
        if (base.TouchBegan(touch))
        {
            dragJoint = new FixedMouseJoint(Physics.EndBody, Physics.EndBody.Position)
            {
                MaxForce = 100f,
                Frequency = 100f,
                WorldAnchorB = GetDragTarget().Point
            };
            return true;
        }
        return false;
    }

    public override void EndDrag()
    {
        base.EndDrag();
        if (dragJoint != null)
        {
            builder.World.RemoveJoint((Joint)(object)dragJoint);
            dragJoint = null;
        }
    }

    public override void Update(float time)
    {
        base.Update(time);
        float b = targetColor;
        float num = CurrentDistance();
        bool flag = CanRelease();
        if (stickyJoint != null && num > maxSnotDistance && flag)
        {
            targetColor = Maths.StepTo(targetColor, 0f, 20f);
            if (timeToRelease > 0.6f || num > extremeSnotDistance)
            {
                ReleaseSnot();
            }
            else
            {
                timeToRelease += time;
            }
        }
        else
        {
            float num2 = 0f;
            if (flag && num > normalDistance)
            {
                num2 = (num - normalDistance) / normalDistance * 200f;
            }
            targetColor = Maths.StepTo(targetColor, 255f - num2, 20f);
            timeToRelease = 0f;
        }
        if (Maths.FuzzyNotEquals(targetColor, b))
        {
            ((TextureSnotSprite)clipContent).TextureColor = new Color(255, (int)targetColor, (int)targetColor);
        }
    }

    public bool CanRelease()
    {
        return stickyJoint != null && (linked is not HeroBodyClip || ((HeroBodyClip)linked).OnGround() || linked.SnotJoinedCount > 1);
    }

    public override void SetDamping(float value)
    {
    }

    public override float DragDistanceMultiplier()
    {
        return 1f;
    }

    public override void ApplyDisconnectForce()
    {
    }
}
