using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

using Mokus2D;
using Mokus2D.Effects.Tweening;
using Mokus2D.Events;
using Mokus2D.Input;
using Mokus2D.Sound;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;
using Mokus2D.Visual.Particles.Util;

namespace ContreJour.Gameplay
{
    [SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "The level scene graph owns and disposes the attached visual nodes.")]
    public sealed class BaloonBodyClip : FurBodyClip, ISnotLinked, IEatable, ISpikesDestroyable, ILaunchable, ITeleportable, IClickable, IRestartable, IVectorPositionProvider
    {
        private readonly float initialScale;
        private readonly HeroEye eye;
        private readonly FurCircle baloonFur;
        private readonly BaloonTailSprite tailSprite;
        internal Portal SpawnPortal { get; }
        private RevoluteJoint heroJoint;
        private Fixture baloonFixture;
        private Touch touch;
        private Vector2 targetPosition;
        private bool movingToTarget;
        private float finishSpeed;
        private float linkDelay;
        private float upForce;
        private float airTime;
        private float heroTimer;
        private bool exploding;
        private bool restartPending;
        private bool levelCompleted;
        private bool respawnScheduled;

        internal Vector2 SpawnPosition { get; }

        public BaloonTail Tail { get; }
        public bool Linked { get; private set; }
        public EventSender DestroyEvent { get; } = new();
        public EventSender LinkEvent { get; } = new();
        public int SnotJoinedCount { get; set; }
        public bool SnotEnabled { get; set; } = true;
        public bool HitEnabled { get; set; } = true;
        public bool DisableHeroFocus => false;
        public override Vector2 PositionVec => Linked ? Body.Position : Tail.End.Position;
        protected override string BaseTexture => "newFriend/McRotatorBase";
        protected override bool WebFurMotion => true;

        public BaloonBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
            : base(builder, body, clip, config)
        {
            SpawnPosition = Body.Position;
            initialScale = Clip.Scale;
            Body.SetGroupIndex(-2);
            Body.SleepingAllowed = false;
            Fixture sensor = Body.CreateFixture(new CircleShape(1f, 0f));
            sensor.IsSensor = true;
            sensor.CollisionGroup = -2;
            Body.Mass = 0.659745f;
            Tail = new BaloonTail(World, Body);
            // The touch dispatcher queries physics fixtures; expose the tip as a draggable target.
            _ = new TailTouchClip(this);
            tailSprite = new BaloonTailSprite(Tail, builder);
            baloonFur = CreateFur();
            GrassSystem.IgnoreParentOpacity = true;
            baloonFur.IgnoreParentOpacity = true;
            // Web FurCircle.draw ignores its rotation property and draws the
            // particle transforms directly; preserve that visible behavior.
            baloonFur.Visible = false;
            Clip.AddChild(new Sprite("newFriend/McBaloonLegs") { Scale = 1.05f });
            // Match the web draw order: fur, legs, tail, then the eye.
            Clip.AddChild(tailSprite);
            eye = new HeroEye(Game) { Scale = 0.7f };
            Clip.AddChild(eye);
            // The fur clip is positioned after construction. Anchor the light
            // to the authored physics spawn, like the web's initialized transform.
            SpawnPortal = new Portal(Game, builder.ToPoint(SpawnPosition), "newFriendSpawn/McFinishPart") { ItemsScale = 0f, ScaleStep = 0.1f };
            builder.Add(SpawnPortal, 8);
            Game.Amie = this;
            Game.AddPositionProvider(new PositionProviderValue(this, 2f));
            Respawn();
        }

        public override FurCircle CreateFur()
        {
            FurCircle fur = new(Mokus2DGame.LoadMovieClipData(GrassTexture()), GrassCount(), GrassRadius());
            Clip.AddChild(fur);
            return fur;
        }

        public override string GrassTexture()
        {
            return "newFriend/McBaloonGrassAll";
        }
        public override int GrassCount()
        {
            return 20;
        }
        public override int GrassRadius()
        {
            return 13;
        }
        public override float Width()
        {
            return 30f;
        }
        public float Radius()
        {
            return 1f;
        }
        public float DeadEyeScale()
        {
            return 0.7f;
        }
        public bool CanDie()
        {
            return SnotEnabled && !exploding && !movingToTarget && !restartPending;
        }
        internal bool CanBeEaten()
        {
            // Web flowers consume characters even while attached. Spike and
            // teleport immunity still follows SnotEnabled through CanDie.
            return Body.Enabled && !exploding && !movingToTarget && !restartPending;
        }
        public bool CanLaunch()
        {
            return !Linked && CanDie();
        }
        public bool CanTeleport()
        {
            return CanDie();
        }
        public void SetSpeedLocked(bool value)
        {
        }
        public void ForceClipPosition()
        {
            UpdatePosition();
        }
        public void SetScaleTime(float scale, float time)
        {
            _ = Clip.ScaleTo(time, scale);
        }

        public void EatSpeedPauseScaleTime(Vector2 targetPosition, float finishSpeed, float pause, float scale, float time)
        {
            EndTailDrag();
            ReleaseHero();
            DestroyEvent.SendEvent();
            StopRespawnActions();
            this.targetPosition = targetPosition;
            this.finishSpeed = finishSpeed;
            movingToTarget = true;
            Body.BodyType = BodyType.Static;
            Body.Enabled = false;
            Body.LinearVelocity = Vector2.Zero;
            Body.SetSensor(true);
            SetScaleTime(scale, time);
        }

        public void Explode()
        {
            if (!CanDie())
            {
                return;
            }
            EndTailDrag();
            ReleaseHero();
            StopRespawnActions();
            exploding = true;
            DestroyEvent.SendEvent();
            Tail.Visible = false;
            Body.BodyType = BodyType.Static;
            eye.AnimationsAllowed = false;
            eye.SetDefaultView();
            SoundManager.PlayRandomSound(Sounds.DeathBySpikes, 0.7f);
            Sequence sequence = Clip.Tweener.StartSequence();
            for (int index = 0; index < 15; index++)
            {
                Vector2 position = Clip.Position + new Vector2(Maths.Random(-2f, 2f), Maths.Random(-2f, 2f));
                float scale = initialScale * (1f + (index / 75f) + (index % 2 == 0 ? -0.05f : 0.05f));
                sequence = sequence.Next(0.02f).ScaleTo(scale).MoveTo(position);
            }
            _ = sequence.OnComplete(DoExplode);
        }

        public void DoExplode()
        {
            Explosion explosion = new("common/McWhiteSmoke")
            {
                Position = Clip.Position,
                HorizontalPosition = new RandomRange(0f, 5f),
                VerticalPosition = new RandomRange(0f, 5f)
            };
            explosion.CreateOnStartPosition(14);
            Builder.Add(explosion, 10);
            Clip.Visible = false;
            RequestRestart(1f);
        }

        private void RequestRestart(float delay)
        {
            if (!levelCompleted && !restartPending)
            {
                restartPending = true;
                Game.Fail(delay);
            }
        }

        public void MarkLevelCompleted()
        {
            levelCompleted = true;
            EndTailDrag();
            ReleaseHero();
            DestroyEvent.SendEvent();
        }

        public void Restart()
        {
            if (respawnScheduled)
            {
                return;
            }
            respawnScheduled = true;
            StopRespawnActions();
            EndTailDrag();
            ReleaseHero();
            DestroyEvent.SendEvent();
            _ = Clip.FadeOut(0.2f);
            Schedule(Respawn, 0.2f);
        }

        public void Respawn()
        {
            // The web game overrides the authored zIndex on every respawn.
            Builder.ChangeChildLayer(Clip, 9);
            respawnScheduled = false;
            StopRespawnActions();
            Clip.Tweener.Stop();
            EndTailDrag();
            ReleaseHero();
            restartPending = false;
            levelCompleted = false;
            movingToTarget = false;
            exploding = false;
            SnotEnabled = true;
            airTime = 0f;
            linkDelay = 0f;
            eye.AnimationsAllowed = true;
            eye.MoveAllowed = true;
            Clip.Visible = true;
            Clip.OpacityFloat = 1f;
            Clip.Scale = 0f;
            Tail.Visible = true;
            Body.SetTransform(SpawnPosition, 0f);
            Body.LinearVelocity = Vector2.Zero;
            Body.AngularVelocity = 0f;
            Body.BodyType = BodyType.Static;
            Body.SetSensor(false);
            // The wide drag-selection fixture must remain a sensor after a respawn.
            foreach (Fixture fixture in Body.FixtureList)
            {
                if (fixture.Shape.Density == 0f)
                {
                    fixture.IsSensor = true;
                }
            }
            Tail.SetPositions(SpawnPosition);
            SpawnPortal.ItemsScale = 0f;
            SpawnPortal.TargetScale = 0f;
            Schedule(BeginRespawn, 0.2f);
        }

        private void StopRespawnActions()
        {
            UnSchedule(Respawn);
            UnSchedule(BeginRespawn);
            UnSchedule(ShowBody);
            UnSchedule(FinishRespawn);
            UnSchedule(HidePortal);
        }

        private void BeginRespawn()
        {
            SpawnPortal.TargetScale = 1f;
            Schedule(ShowBody, 0.2f);
        }

        private void ShowBody()
        {
            Body.Enabled = true;
            _ = Clip.ScaleTo(0.7f, initialScale);
            Schedule(FinishRespawn, 0.7f);
        }

        private void FinishRespawn()
        {
            Body.BodyType = BodyType.Dynamic;
            RefreshDamping();
            Schedule(HidePortal, 0.2f);
        }

        private void HidePortal()
        {
            SpawnPortal.TargetScale = 0f;
        }

        public void Teleport(BodyClip teleport)
        {
            EndTailDrag();
            ReleaseHero();
            DestroyEvent.SendEvent();
        }

        public void AfterTeleport()
        {
            Tail.SetPositions(Body.Position);
            Tail.SpringTail(Game.TotalTime);
        }

        public int Priority(Vector2 touchPosition)
        {
            return Linked ? 2 : 0;
        }
        public bool AcceptFreeTouches()
        {
            return !Linked || Tail.Dragging;
        }
        public bool UseForZoom()
        {
            return false;
        }
        public override float TouchDistance(Vector2 touchPosition)
        {
            return Vector2.Distance(PositionVec, touchPosition);
        }

        public bool TouchBegan(Touch touch)
        {
            if (exploding || movingToTarget || restartPending)
            {
                return false;
            }
            Vector2 position = Builder.TouchRootVec(touch);
            if (Linked && Vector2.Distance(position, Body.Position) < 1f)
            {
                Tail.SpringTail(Game.TotalTime);
                ReleaseHero();
                return false;
            }
            if (Linked || linkDelay > 0f || Vector2.Distance(position, Tail.End.Position) >= 4f / 3f)
            {
                return false;
            }
            SoundManager.PlaySound("leapOn1", 0.5f);
            this.touch = touch;
            Tail.TargetPosition = position;
            Tail.SetDragging(true, Game.TotalTime);
            return true;
        }

        public bool TouchMove(Touch touch)
        {
            return this.touch == touch;
        }
        public void TouchEnd(Touch touch)
        {
            if (this.touch == touch)
            {
                EndTailDrag();
            }
        }
        public void TouchOut(Touch touch)
        {
            TouchEnd(touch);
        }

        private void EndTailDrag()
        {
            Tail.SetDragging(false, Game.TotalTime);
            touch = null;
        }

        public void LinkToHero()
        {
            if (Linked || Game.Hero == null || !CanDie())
            {
                return;
            }
            SoundManager.PlayRandomSound(Sounds.LeapOn, 0.5f);
            Linked = true;
            SnotEnabled = false;
            Tail.SetLinked(true);
            Tail.End.Position = Game.Hero.Body.Position;
            heroJoint = FarseerUtil.CreateRevoluteJoint(World, Tail.End, Game.Hero.Body, Game.Hero.Body.Position);
            EndTailDrag();
            eye.MoveAllowed = false;
            eye.ViewDistance = 0.3f;
            Body.LinearDamping = Body.AngularDamping = 1f;
            upForce = 0f;
            Schedule(CreateBaloon, 0.2f);
            DestroyEvent.SendEvent();
            Game.Hero.TeleportEvent.AddListener(OnHeroTeleport);
            LinkEvent.SendEvent();
        }

        private void CreateBaloon()
        {
            if (Linked && baloonFixture == null)
            {
                baloonFixture = Body.CreateFixture(new CircleShape(1f, 0f));
                baloonFixture.CollisionGroup = -2;
            }
        }

        public void ReleaseHero()
        {
            if (!Linked)
            {
                return;
            }
            SoundManager.PlayRandomSound(Sounds.LeapOut, 0.5f);
            Linked = false;
            Game.Hero?.TeleportEvent.RemoveListener(OnHeroTeleport);
            Tail.SetDragging(false, Game.TotalTime);
            Tail.SetLinked(false);
            if (heroJoint != null)
            {
                World.RemoveJoint(heroJoint);
                heroJoint = null;
            }
            if (baloonFixture != null)
            {
                Body.DestroyFixture(baloonFixture);
                baloonFixture = null;
            }
            SnotEnabled = true;
            eye.MoveAllowed = true;
            touch = null;
            linkDelay = 0.5f;
            Schedule(RefreshDamping, 0.2f);
        }

        private void OnHeroTeleport()
        {
            ReleaseHero();
            Tail.SpringTail(Game.TotalTime);
        }

        private void RefreshDamping()
        {
            if (!Linked)
            {
                Body.LinearDamping = 0f;
                Body.AngularDamping = 0.1f;
            }
        }

        internal static float LiftAcceleration(int joinedSnots, float height, float worldHeight, bool nearRightEdge)
        {
            float ceilingDistance = worldHeight - height;
            float ceilingZone = 70f / 30f * (nearRightEdge ? 2f : 1f);
            float acceleration = 28f + (joinedSnots * 2f);
            if (ceilingDistance < ceilingZone)
            {
                acceleration -= (1f - (ceilingDistance / ceilingZone)) * 44f;
            }
            return Math.Max(acceleration, 22f / 3f);
        }

        public override void Update(float time)
        {
            if (movingToTarget)
            {
                Vector2 difference = targetPosition - Body.Position;
                Body.Position = difference.Length() <= finishSpeed ? targetPosition : Body.Position + (Vector2.Normalize(difference) * finishSpeed);
            }
            UpdateHeroSeparation(time);
            base.Update(time);
            if (!Linked)
            {
                airTime = FarseerUtil.IsTouching(Body) ? 0f : airTime + time;
                Body.AngularDamping = airTime > 1f ? 1f : 0.1f;
                linkDelay = Math.Max(0f, linkDelay - time);
            }
            Tail.Update(time, Game.TotalTime);
            tailSprite.RotationRadians = -Clip.RotationRadians;
            eye.RotationRadians = -Clip.RotationRadians;
            if (Linked && Game.Hero != null)
            {
                Vector2 difference = Game.Hero.Body.Position - Body.Position;
                eye.ViewAngle = MathF.Atan2(difference.Y, difference.X);
                eye.ViewDistance = difference.Length() / (130f / 30f);
            }
            else
            {
                eye.SetVelocity(Body.LinearVelocity);
            }
            if (touch != null)
            {
                Vector2 target = Builder.TouchRootVec(touch);
                Vector2 offset = target - Body.Position;
                if (offset.Length() > 5f)
                {
                    target = Body.Position + (Vector2.Normalize(offset) * 5f);
                }
                if (Game.Hero != null && Vector2.Distance(target, Game.Hero.Body.Position) < 1f && Vector2.Distance(Body.Position, Game.Hero.Body.Position) < 170f / 30f)
                {
                    target = Game.Hero.Body.Position;
                    LinkToHero();
                }
                Tail.TargetPosition = target;
            }
            if (Linked && !exploding)
            {
                Vector2 worldSize = Builder.ToVec(Game.LevelSize);
                float ceilingZone = 70f / 30f * (Body.Position.X > worldSize.X - (100f / 30f) ? 2f : 1f);
                float ceilingDistance = worldSize.Y - Body.Position.Y;
                Vector2 velocity = Body.LinearVelocity;
                velocity.Y = Math.Max(-4.2f, velocity.Y);
                if (ceilingDistance < ceilingZone && velocity.Y < -1f)
                {
                    velocity.Y = Maths.StepTo(velocity.Y, -1f, 1f);
                }
                Body.LinearVelocity = velocity;
                upForce = Maths.StepTo(upForce, LiftAcceleration(SnotJoinedCount + Game.Hero.SnotJoinedCount, Body.Position.Y, worldSize.Y, ceilingZone > 70f / 30f), 2f);
                Body.ApplyForce(new Vector2(0f, upForce * Body.Mass));
            }
            if (!exploding)
            {
                float radius = Maths.StepTo(GrassSystem.Radius, Linked ? 26f : 13f, 0.5f);
                GrassSystem.Radius = radius;
                baloonFur.Radius = radius;
                baloonFur.Visible = radius > 13f;
                eye.Scale = 0.7f + (0.35f * ((radius - 13f) / 13f));
                SetBaseWidth((2f * radius) + 4f);
            }
            float furOpacity = Math.Max(0f, 1f - (1.25f * (1f - Clip.OpacityFloat)));
            GrassSystem.OpacityFloat = furOpacity;
            baloonFur.OpacityFloat = (GrassSystem.Radius - 13f) / 13f * furOpacity;
            if (Body.Position.Y < -50f / 30f)
            {
                RequestRestart(0f);
            }
        }

        private void UpdateHeroSeparation(float time)
        {
            if (Linked || exploding || Game.Hero == null)
            {
                heroTimer = 0f;
                return;
            }
            Vector2 difference = Game.Hero.Body.Position - Body.Position;
            float distance = difference.Length();
            if (distance < 1f)
            {
                heroTimer += time;
                if (heroTimer >= 1.5f)
                {
                    Vector2 force = new(difference.X * (distance - 1f) * 40f, 0f);
                    Body.ApplyForce(force);
                    Game.Hero.Body.ApplyForce(-force);
                    heroTimer = 0f;
                }
            }
            else
            {
                heroTimer = 0f;
            }
        }

        public override void Clear()
        {
            EndTailDrag();
            ReleaseHero();
            UnSchedule(CreateBaloon);
            UnSchedule(RefreshDamping);
            UnSchedule(Respawn);
            UnSchedule(BeginRespawn);
            UnSchedule(ShowBody);
            UnSchedule(FinishRespawn);
            UnSchedule(HidePortal);
            UnSchedule(DoExplode);
            Tail.Clear();
            base.Clear();
        }
        // A separate touch target keeps the tail's sensor out of gameplay and restart queries.
        private sealed class TailTouchClip(BaloonBodyClip owner) : BodyClip(owner.Builder, owner.Tail.End, null, null), IClickable
        {
            public bool DisableHeroFocus => owner.DisableHeroFocus;
            public int Priority(Vector2 touchPosition)
            {
                return owner.Priority(touchPosition);
            }
            public float TouchDistance(Vector2 touchPosition)
            {
                return owner.TouchDistance(touchPosition);
            }
            public bool AcceptFreeTouches()
            {
                return owner.AcceptFreeTouches();
            }
            public bool UseForZoom()
            {
                return owner.UseForZoom();
            }
            public bool TouchBegan(Touch touch)
            {
                return owner.TouchBegan(touch);
            }
            public bool TouchMove(Touch touch)
            {
                return owner.TouchMove(touch);
            }
            public void TouchEnd(Touch touch)
            {
                owner.TouchEnd(touch);
            }
            public void TouchOut(Touch touch)
            {
                owner.TouchOut(touch);
            }
        }
    }
}
