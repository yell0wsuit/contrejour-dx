using System;
using System.Numerics;

using ContreJour.Clips;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Mokus2D.Sound;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public class SpikesFlowerBodyClip : ContreJourBodyClip, IVectorPositionProvider
    {
        private IEatable hero;

        private readonly ISpikesView movie;
        private readonly SpikesFlowerSprite drawing;

        private readonly Node container;

        private static readonly Vector2 EyePosition = new(0f, 30f);

        public FlowerEye Eye { get; private set; }

        public override Vector2 PositionVec => Body.Position;

        public SpikesFlowerBodyClip(ContreJourLevelBuilder builder, object body, Node clip, Hashtable config)
            : base(builder, body, clip, config)
        {
            if (Game.WhiteSide || Game.BonusChapter)
            {
                clip = LevelBuilderBase.ReplaceClipWith(clip, Game.Choose(null, null, "McSpikesViewWhite", null, "McSpikesView_6"));
                Clip = clip;
            }
            container = new Node();
            Clip.AddChild(container, -1);
            Sprite node = new(ClipIds.Common.McSpikesFlowerShadow)
            {
                Scale = Clip.ScaleY
            };
            container.AddChild(node);
            movie = Game.BonusChapter ? new SingleMovieSpikesView((MovieClip)clip) : (ISpikesView)clip;
            movie.Left.Stoped = movie.Right.Stoped = true;
            movie.Left.Speed = movie.Right.Speed = 1.5f;
            drawing = new SpikesFlowerSprite(this, Clip.ScaleY);
            container.AddChild(drawing, -1);
        }

        // Mango exports the complete flower as one movie, so both sides use its player.
        private sealed class SingleMovieSpikesView(MovieClip clip) : ISpikesView
        {
            public MovieClip Left => clip;

            public MovieClip Right => clip;
        }

        private void CreateEye()
        {
            Vector2 point = container.LocalToNode(EyePosition, Game.Root);
            Eye = new FlowerEye(Game, visible: true, Builder.ToVec(point))
            {
                Scale = Clip.ScaleY * 0.7f
            };
            container.AddChild(Eye);
            Eye.RefreshRootAngle();
            Eye.Position = EyePosition;
        }

        public override void Update(float time)
        {
            base.Update(time);
            if (Eye == null && container.Root != null)
            {
                CreateEye();
            }
            Eye.UpdateNode(time);
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
                Eye.PositionProvider = this;
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
            Eye.PositionProvider = null;
            CreateDeadEye();
        }

        public void CreateDeadEye()
        {
            Body val = Builder.World.CreateCircle(16f * Builder.EngineConfig.SizeMultiplier * hero.DeadEyeScale(), Body.Position, 0f, Builder.EngineConfig.Density, dynamic: true);
            Sprite node = (Game.WhiteSide || Game.BonusChapter) ? new Sprite(ClipIds.Common2.McEyeDeadBlack) : new Sprite(ClipIds.Common.McEyeDead);
            if (Game.BonusChapter)
            {
                node.Color = ContreJourConstants.GreenLightColor;
            }
            BodyClip bodyClip = new(Builder, val, node, null);
            Builder.Add(node, 10);
            node.Scale = 0f;
            _ = node.Tweener.StartSequence(0.3f).ScaleTo(hero.DeadEyeScale()).Next(2f)
                .FadeOut();
            Schedule(bodyClip.Destroy, 2.3f);
            val.ApplyLinearImpulse(VectorUtil.Rotate(new Vector2(Maths.Random(), 3f), BodyAngle) * (float)Math.Pow(hero.DeadEyeScale(), 2.0), val.WorldCenter);
        }
    }
}
