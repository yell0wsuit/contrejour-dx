using System;

using ContreJourMono.ContreJour.Game.Eyes;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D;
using Mokus2D.Input;
using Mokus2D.Sound;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;
using Mokus2D.Visual.Data;

namespace Default.Namespace;

public class BackSnotBodyClip : SnotBodyClipBase, IClickable
{
    private float force;

    private float forceProgress;

    private float forceStep;

    private bool stabilize;

    private bool stabilizeCalculated;

    public override Body EyeBody => Physics.EndBody;

    public bool DisableHeroFocus => true;

    public BackSnotBodyClip(LevelBuilderBase builder, SnotData body, Node clip, Hashtable config)
        : base(builder, body, clip, config)
    {
        force = 0.25f;
        forceProgress = Maths.Random(0f, (float)Math.PI * 2f);
        forceStep = Maths.Random(0.01f, 0.02f);
        stabilize = false;
        stabilizeCalculated = false;
        Vector2 vector = this.config.GetVector("scale");
        eye.Scale = vector.X / 10.24f;
        baseClip.Scale = eye.Scale;
        baseEndClip.Scale = eye.Scale;
        _ = Mokus2DGame.LoadResource<MovieClipData>("chapter1/McBackSnotEyeBlink");
    }

    public int Priority(Vector2 touchPosition)
    {
        return -1;
    }

    public bool AcceptFreeTouches()
    {
        return false;
    }

    public bool UseForZoom()
    {
        return true;
    }

    public bool TouchBegan(Touch touch)
    {
        Vector2 vector = Physics.GetWorldStartPoint() - Physics.EndBody.Position;
        vector *= 0.7f / vector.Length();
        Physics.EndBody.ApplyLinearImpulse(vector, Physics.EndBody.WorldCenter);
        eye.PlayAnimation(new EyeAnimation("McBackSnotEyeBlink"), force: false);
        eye.RandomPositionProvider = game.GetTouchProvider(touch);
        SoundManager.PlayRandomSound(Sounds.BackSnot, Maths.Random(0.3f, 0.5f));
        if (Maths.Random() < 0.5f)
        {
            SoundManager.PlaySound("backgroundEyeHit0", Maths.Random(0.3f, 0.5f));
        }
        return false;
    }

    public bool TouchMove(Touch touch)
    {
        return false;
    }

    public void TouchOut(Touch touch)
    {
    }

    public void TouchEnd(Touch touch)
    {
    }

    public override int Layer()
    {
        return -9;
    }

    public override string BaseClipName()
    {
        return "McBackSnotBase";
    }

    public override string BaseEndClipName()
    {
        return "McBackSnotBase";
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (eye != null)
        {
            _ = Mokus2DGame.LoadMovieClipData("chapter1/McBackSnotEyeBlink");
            if (!stabilizeCalculated)
            {
                stabilizeCalculated = true;
                stabilize = Maths.FuzzyNotEquals((float)Math.Ceiling(Maths.SimplifyAngleDegrees(rotationOffset, -180f) / 90f), 0f);
            }
            for (int i = 0; i < Physics.BodiesSize(); i++)
            {
                Physics.BodyAt(i).GravityScale = (!stabilize) ? 1 : 0;
            }
            Physics.EndBody.GravityScale = stabilize ? 0.2f : 0f;
            float module = (force / 4f * 3f) + (force * Maths.Cos(forceProgress) / 4f);
            forceProgress += forceStep;
            Physics.EndBody.ApplyForce(VectorUtil.ToVector(module, eye.ViewAngle), Physics.EndBody.WorldCenter);
        }
    }

    public override SnotSprite CreateClip()
    {
        Vector2 vector = config.GetVector("scale");
        BackSnotSprite backSnotSprite = new(this, 9f * vector.X * builder.EngineConfig.SizeMultiplier, 4f * vector.X * builder.EngineConfig.SizeMultiplier, 9f * vector.X * builder.EngineConfig.SizeMultiplier)
        {
            NeckColor = 3947580.ToRGBColor()
        };
        return backSnotSprite;
    }

    protected override MonsterEye CreateEye()
    {
        return new BackSnotEye((ContreJourGame)builder.Game, visible: true, Physics.EndBody.Position);
    }
}
