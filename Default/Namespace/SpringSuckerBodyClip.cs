using System;

using ContreJour.Clips.chapter5;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Microsoft.Xna.Framework;

using Mokus2D.Events;
using Mokus2D.Input;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class SpringSuckerBodyClip : SuckerBodyClip
{
    public readonly EventSender ContactEvent = new EventSender();

    protected Vector2 parallel;

    protected Vector2 normal;

    private float JUMP_IMPULSE = 1f;

    private float SNOT_JUMP_IMPULSE = 0.2f;

    private float SPEED_MULT = 0.75f;

    private bool touched;

    private bool autocreated;

    public bool Autocreated => autocreated;

    protected override float BounceVolume => 0.4f;

    protected override string BounceSound => "spring";

    private bool CanAutocreate
    {
        get
        {
            if (touch == null)
            {
                return config.GetBool("auto");
            }
            return false;
        }
    }

    public SpringSuckerBodyClip(LevelBuilderBase _builder, object _body, Node _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
        AutoCreate();
    }

    public override void Restart()
    {
        if (CanAutocreate)
        {
            if (touched)
            {
                base.Restart();
                AutoCreate();
            }
        }
        else
        {
            base.Restart();
        }
    }

    private void AutoCreate()
    {
        autocreated = CanAutocreate;
        if (autocreated)
        {
            ghostSprite.Visible = false;
            createPosition = Body.Position;
            createPosition -= new Vector2(maxDistance, 0f);
            CreateBodies();
        }
    }

    protected override SuckerNeckSprite CreateNeck()
    {
        return new SuckerNeckSprite();
    }

    public override Node CreatePimpa()
    {
        return new McSuckerBody();
    }

    public void CreateLegs()
    {
    }

    public override void CreateBodies()
    {
        base.CreateBodies();
        parallel = VectorUtil.ToVector(1f, bounceAngle);
        normal = parallel.Rotate90();
    }

    public override void StartDrag(Touch _touch)
    {
        base.StartDrag(_touch);
        touched = true;
    }

    public override void OnCollisionStartPoint(Body body2, Contact point)
    {
        base.OnCollisionStartPoint(body2, point);
        HeroBodyClip heroBodyClip = body2.UserData as HeroBodyClip;
        if (end != null && (point.FixtureA == middleFixture || point.FixtureB == middleFixture) && heroBodyClip != null)
        {
            Vector2 linearVelocity = heroBodyClip.Body.LinearVelocity;
            float num = VectorUtil.Atan2(linearVelocity);
            linearVelocity = VectorUtil.ToVector(angle: (float)Math.PI + bounceAngle - (num - bounceAngle - (float)Math.PI), module: linearVelocity.Length());
            Vector2 vector = VectorUtil.VectorProjection(linearVelocity, parallel);
            linearVelocity = VectorUtil.VectorProjection(linearVelocity, normal);
            linearVelocity *= SPEED_MULT;
            linearVelocity += vector;
            heroBodyClip.Body.LinearVelocity = linearVelocity;
            float num2 = (VectorUtil.Atan2(Body.Position, body2.Position) - bounceAngle).SimplifyAngle(-(float)Math.PI);
            float num3 = JUMP_IMPULSE + (float)heroBodyClip.SnotJoinedCount * SNOT_JUMP_IMPULSE;
            if (num2 < 0f)
            {
                num3 *= -1f;
            }
            body2.ApplyLinearImpulse(VectorUtil.Rotate(new Vector2(0f, num3), bounceAngle), body2.WorldCenter);
            neck.Bounce();
            ContactEvent.SendEvent();
            PlayBounceSound();
        }
    }
}
