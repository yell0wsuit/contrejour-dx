using System;
using System.Numerics;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Mokus2D.Effects.Tween.Easing;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public class SimpleSpikesBodyClip : ContreJourBodyClip, IRestartable
    {
        private readonly bool floating;

        private float speed;

        private float direction;

        private float angleStep;

        private Vector2 initialPosition;

        private float prickTime;

        private bool actionsRunning;

        private readonly float initialScale;

        public SimpleSpikesBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
            : base(builder, body, clip, config)
        {
            ContreJourGame contreJourGame = (ContreJourGame)builder.Game;
            string text = config.GetString("viewType");
            floating = text.Contains("Circle");
            if (!contreJourGame.BlackSide)
            {
                Clip = LevelBuilderBase.ReplaceClipWith(clip, text + contreJourGame.ChooseSide(null, "White", "_5", "Black", "_6"));
            }
            Clip.UpdateEnabled = false;
            prickTime = -2f;
            initialPosition = Clip.Position;
            initialScale = Clip.ScaleX;
            RunActions();
        }

        public void RunActions()
        {
            if (!actionsRunning)
            {
                actionsRunning = true;
                if (floating)
                {
                    speed = Maths.Random(2f, 3f);
                    direction = Maths.Random(0f, (float)Math.PI * 2f);
                    angleStep = Maths.Random(0.8f, 1.2f);
                    float scale = Maths.Random(0.95f, 0.98f) * initialScale;
                    float scale2 = Maths.Random(1.02f, 1.05f) * initialScale;
                    float seconds = Maths.Random(2f, 3f);
                    _ = Clip.Tweener.RepeatSequenceForever(seconds).ScaleTo(scale, Cubic.EaseInOut).Next(seconds)
                        .ScaleTo(scale2, Cubic.EaseInOut);
                }
            }
        }

        public void Restart()
        {
            prickTime = -2f;
        }

        public override void Update(float time)
        {
            base.Update(time);
            if (!actionsRunning && Game.TotalTime - prickTime >= 2f)
            {
                RunActions();
                MovieClip movieClip = (MovieClip)Clip;
                movieClip.Rewind = true;
                movieClip.Stoped = false;
            }
            if (floating && actionsRunning)
            {
                float num = Math.Min(time, 1f / 30f);
                Vector2 vector = initialPosition - Clip.Position;
                direction = Maths.StepTo(target: Maths.Atan2(vector.Y, vector.X).SimplifyAngle(direction - (float)Math.PI), maxStep: angleStep * num, value: direction);
                Vector2 vector2 = VectorUtil.ToVector(speed * num, direction);
                Clip.Position = Clip.Position + vector2;
            }
        }

        public override void UpdatePosition()
        {
            if (!floating)
            {
                base.UpdatePosition();
            }
        }

        public override void OnCollisionStartPoint(Body body2, Contact point)
        {
            if (body2.UserData is ISpikesDestroyable spikesDestroyable && !point.IsSensor() && spikesDestroyable.CanDie())
            {
                OnHeroHitPoint(spikesDestroyable);
                MovieClip movieClip = (MovieClip)Clip;
                movieClip.UpdateEnabled = true;
                movieClip.Repeat = false;
                movieClip.Rewind = false;
                movieClip.Stoped = false;
                Clip.Tweener.Stop();
                prickTime = Game.TotalTime;
                actionsRunning = false;
            }
        }

        public static void OnHeroHitPoint(ISpikesDestroyable hero)
        {
            hero.Explode();
        }
    }
}
