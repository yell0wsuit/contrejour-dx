using System;
using ContreJour.Clips.common;
using ContreJour.Clips.common2;
using ContreJour.Clips.partial;
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
    private const float EYE_SCALE = 0.7f;

    private const float DEAD_EYE_RADIUS = 16f;

    protected IEatable hero;

    protected ISpikesView movie;

    protected FlowerEye eye;

    protected SpikesFlowerSprite drawing;

    protected Node container;

    private static readonly Vector2 EYE_POSITION = new Vector2(0f, 30f);

    public FlowerEye Eye => eye;

    public override Vector2 PositionVec => Body.Position;

    public SpikesFlowerBodyClip(ContreJourLevelBuilder _builder, object _body, Node _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
        if (base.Game.WhiteSide || base.Game.BonusChapter)
        {
            _clip = _builder.ReplaceClipWith(_clip, base.Game.Choose(null, null, "McSpikesViewWhite", null, "McSpikesView_6"));
            clip = _clip;
        }
        container = new Node();
        clip.AddChild(container, -1);
        Node node = new McSpikesFlowerShadow
        {
            Scale = clip.ScaleY
        };
        container.AddChild(node);
        movie = (ISpikesView)_clip;
        movie.left.Stoped = (movie.right.Stoped = true);
        movie.left.Speed = (movie.right.Speed = 1.5f);
        drawing = new SpikesFlowerSprite(this, clip.ScaleY);
        container.AddChild(drawing, -1);
    }

    private void CreateEye()
    {
        Vector2 point = container.LocalToNode(EYE_POSITION, base.Game.Root);
        eye = new FlowerEye(base.Game, _visible: true, builder.ToVec(point));
        eye.Scale = clip.ScaleY * 0.7f;
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
        if (body2.UserData is IEatable eatable && movie.left.Stoped && Maths.FuzzyEquals(movie.left.CurrentFrame, 0f) && eatable.CanDie())
        {
            hero = eatable;
            hero.EatSpeedPauseScaleTime(Body.Position, 0.5f, 1.3f, 0f, 0.2f);
            if (hero is HeroBodyClip)
            {
                UserData.Instance.FeedMonster++;
            }
            hero.Clip.Parent.ChangeChildLayer(hero.Clip, -1);
            movie.left.Stoped = (movie.right.Stoped = false);
            movie.left.Rewind = (movie.right.Rewind = false);
            movie.left.EndEvent += OnCloseEnd;
            eye.PositionProvider = this;
            SoundManager.PlaySound("deathByFlowerOut4", 0.5f);
        }
    }

    private void OnCloseEnd(IAnimatedNode animatedNode)
    {
        movie.left.EndEvent -= OnCloseEnd;
        movie.left.GotoAndStop(movie.left.TotalFrames - 1);
        movie.right.GotoAndStop(movie.right.TotalFrames - 1);
        Schedule(Open, 1f);
    }

    private void Open()
    {
        SoundManager.PlaySound("deathByFlowerOut10", 0.7f);
        movie.left.Rewind = (movie.right.Rewind = true);
        movie.left.Stoped = (movie.right.Stoped = false);
        movie.left.Repeat = (movie.right.Repeat = false);
        eye.PositionProvider = null;
        CreateDeadEye();
    }

    public void CreateDeadEye()
    {
        Body val = builder.World.CreateCircle(16f * builder.EngineConfig.SizeMultiplier * hero.DeadEyeScale(), Body.Position, 0f, builder.EngineConfig.Density, dynamic: true);
        Node node = ((base.Game.WhiteSide || base.Game.BonusChapter) ? ((Node)new McEyeDeadBlack()) : ((Node)new McEyeDead()));
        if (base.Game.BonusChapter)
        {
            node.Color = ContreJourConstants.GreenLightColor;
        }
        BodyClip bodyClip = new BodyClip(builder, val, node, null);
        builder.Add(node, 10);
        node.Scale = 0f;
        node.Tweener.StartSequence(0.3f).ScaleTo(hero.DeadEyeScale()).Next(2f)
            .FadeOut();
        Schedule(bodyClip.Destroy, 2.3f);
        val.ApplyLinearImpulse(VectorUtil.Rotate(new Vector2(Maths.Random(), 3f), base.BodyAngle) * (float)Math.Pow(hero.DeadEyeScale(), 2.0), val.WorldCenter);
    }
}
