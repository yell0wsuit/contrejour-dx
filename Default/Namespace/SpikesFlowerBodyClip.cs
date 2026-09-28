using System;

using ContreJour.Clips;
using ContreJour.Clips.common;
using ContreJour.Clips.common2;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Microsoft.Xna.Framework;

using Mokus2D.Sound;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class SpikesFlowerBodyClip : ContreJourBodyClip, IVectorPositionProvider
{
    protected IEatable hero;

    protected ISpikesView movie;

    protected FlowerEye eye;

    protected SpikesFlowerSprite drawing;

    protected Node container;

    private static readonly Vector2 EYE_POSITION = new(0f, 30f);

    public FlowerEye Eye => eye;

    public override Vector2 PositionVec => Body.Position;

    public SpikesFlowerBodyClip(ContreJourLevelBuilder builder, object body, Node clip, Hashtable config)
        : base(builder, body, clip, config)
    {
        if (Game.WhiteSide || Game.BonusChapter)
        {
            clip = LevelBuilderBase.ReplaceClipWith(clip, Game.Choose(null, null, "McSpikesViewWhite", null, "McSpikesView_6"));
            this.clip = clip;
        }
        container = new Node();
        this.clip.AddChild(container, -1);
        Node node = new McSpikesFlowerShadow
        {
            Scale = this.clip.ScaleY
        };
        container.AddChild(node);
        movie = (ISpikesView)clip;
        movie.Left.Stoped = movie.Right.Stoped = true;
        movie.Left.Speed = movie.Right.Speed = 1.5f;
        drawing = new SpikesFlowerSprite(this, this.clip.ScaleY);
        container.AddChild(drawing, -1);
    }

    private void CreateEye()
    {
        Vector2 point = container.LocalToNode(EYE_POSITION, Game.Root);
        eye = new FlowerEye(Game, visible: true, builder.ToVec(point))
        {
            Scale = clip.ScaleY * 0.7f
        };
        container.AddChild(eye);
        eye.RefreshRootAngle();
        eye.Position = EYE_POSITION;
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (eye == null && container.Root != null)
        {
            CreateEye();
        }
        eye.UpdateNode(time);
        drawing.Update(time);
    }

    public override void OnCollisionPoint(Body body2, Contact point)
    {
        if (body2.UserData is IEatable eatable && movie.Left.Stoped && Maths.FuzzyEquals(movie.Left.CurrentFrame, 0f) && eatable.CanDie())
        {
            hero = eatable;
            hero.EatSpeedPauseScaleTime(Body.Position, 0.5f, 1.3f, 0f, 0.2f);
            if (hero is HeroBodyClip)
            {
                UserData.Instance.FeedMonster++;
            }
            hero.Clip.Parent.ChangeChildLayer(hero.Clip, -1);
            movie.Left.Stoped = movie.Right.Stoped = false;
            movie.Left.Rewind = movie.Right.Rewind = false;
            movie.Left.EndEvent += OnCloseEnd;
            eye.PositionProvider = this;
            SoundManager.PlaySound("deathByFlowerOut4", 0.5f);
        }
    }

    private void OnCloseEnd(IAnimatedNode animatedNode)
    {
        movie.Left.EndEvent -= OnCloseEnd;
        movie.Left.GotoAndStop(movie.Left.TotalFrames - 1);
        movie.Right.GotoAndStop(movie.Right.TotalFrames - 1);
        Schedule(Open, 1f);
    }

    private void Open()
    {
        SoundManager.PlaySound("deathByFlowerOut10", 0.7f);
        movie.Left.Rewind = movie.Right.Rewind = true;
        movie.Left.Stoped = movie.Right.Stoped = false;
        movie.Left.Repeat = movie.Right.Repeat = false;
        eye.PositionProvider = null;
        CreateDeadEye();
    }

    public void CreateDeadEye()
    {
        Body val = builder.World.CreateCircle(16f * builder.EngineConfig.SizeMultiplier * hero.DeadEyeScale(), Body.Position, 0f, builder.EngineConfig.Density, dynamic: true);
        Node node = (Game.WhiteSide || Game.BonusChapter) ? new McEyeDeadBlack() : new McEyeDead();
        if (Game.BonusChapter)
        {
            node.Color = ContreJourConstants.GreenLightColor;
        }
        BodyClip bodyClip = new(builder, val, node, null);
        builder.Add(node, 10);
        node.Scale = 0f;
        _ = node.Tweener.StartSequence(0.3f).ScaleTo(hero.DeadEyeScale()).Next(2f)
            .FadeOut();
        Schedule(bodyClip.Destroy, 2.3f);
        val.ApplyLinearImpulse(VectorUtil.Rotate(new Vector2(Maths.Random(), 3f), BodyAngle) * (float)Math.Pow(hero.DeadEyeScale(), 2.0), val.WorldCenter);
    }
}
