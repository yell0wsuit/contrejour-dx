using System;
using System.Linq;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Microsoft.Xna.Framework;

using Mokus2D.Input;
using Mokus2D.Sound;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;
using Mokus2D.Visual.Particles.Util;

namespace ContreJour.Gameplay
{
    public class SpringBodyClip : ContreJourBodyClip, IClickable, IRestartable
    {
        private static readonly Vector2 BodyCenter = new(0f, 20f);

        private static readonly Vector2 DefaultSmokePoint = new(0f, 60f);

        private static readonly Vector2 SuckPoint = new(0f, 40f);

        protected Vector2 BodyCenterVec { get; set; }

        private readonly CosChanger breatheChanger;

        private float launchTime;

        private readonly MovieClip movie;
        private Vector2 relativeStickedPosition;
        private readonly WhiteSmoke smoke;

        private readonly float startScale;

        protected ILaunchable Sticked { get; set; }

        private readonly float suckDistance;

        private Vector2 suckPoint;

        private float timeToSmoke;

        protected virtual Vector2 SmokePoint => DefaultSmokePoint;

        public Vector2 WorldSuckPoint => VectorUtil.Rotate(suckPoint, BodyAngle) + Body.Position;

        public bool DisableHeroFocus => true;

        public SpringBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
            : base(builder, body, clip, config)
        {
            if (!Game.BlackSide)
            {
                clip = LevelBuilderBase.ReplaceClipWith(clip, GetClipName());
                Clip = clip;
            }
            Clip.Parent.ChangeChildLayer(clip, 2);
            Body.IsBullet = true;
            Config["noShadow"] = "true";
            Config["noSound"] = "true";
            movie = (MovieClip)Clip;
            movie.Stoped = true;
            movie.Repeat = false;
            movie.MinFrame = 7f;
            CreateShadow();
            startScale = Clip.ScaleX;
            suckPoint = Builder.ToVec(SuckPoint * Clip.ScaleX);
            suckDistance = 150f * Clip.ScaleX * Builder.SizeMult;
            BodyCenterVec = VectorUtil.Rotate(Builder.ToVec(BodyCenter * Clip.ScaleX), InitialBodyAngle);
            breatheChanger = new CosChanger(0.06f, 0.07f)
            {
                MinValue = 0.95f,
                MaxValue = 1.04f
            };
            foreach (Fixture fixture in Body.FixtureList)
            {
                fixture.Friction = 1f;
                if (IsSticky(fixture))
                {
                    fixture.IsSensor = true;
                }
            }
            launchTime = -0.2f;
            smoke = new WhiteSmoke(Game.BlackSide ? "common/McWhiteSmokeBlack" : "common/McWhiteSmoke");
            if (Game.BonusChapter)
            {
                smoke.Color = ContreJourConstants.GreenLightColor;
            }
            smoke.ScaleDownOnDestroy = false;
            RefreshSmokeAngle();
            Builder.Add(smoke, 11);
            CreateSmoke();
            timeToSmoke = Maths.Random(1.5f, 2.3f);
            Clip.AddedToStageEvent += RefreshPoints;
            SetThinSmokeRange();
        }

        public bool UseForZoom()
        {
            return false;
        }

        public virtual int Priority(Vector2 touchPosition)
        {
            return Sticked == null || !IsTouchDistance(touchPosition) ? -10 : 2;
        }

        public bool AcceptFreeTouches()
        {
            return false;
        }

        public virtual bool TouchBegan(Touch touch)
        {
            //IL_0027: Unknown result type (might be due to invalid IL or missing references)
            //IL_002d: Invalid comparison between Unknown and I4
            if (IsTouchDistance(Builder.TouchRootVec(touch)))
            {
                if (Sticked != null && (int)Sticked.Body.BodyType == 2)
                {
                    return false;
                }
                LaunchTouching();
                if (Sticked != null)
                {
                    Launch();
                }
                else if (Maths.FuzzyEquals(movie.CurrentFrame, 7f) && movie.Stoped)
                {
                    SoundManager.PlaySound("perdelkaOutEmpty1", 0.7f);
                    Spit();
                }
                return true;
            }
            return false;
        }

        public virtual bool TouchMove(Touch touch)
        {
            return true;
        }

        public virtual void TouchOut(Touch touch)
        {
        }

        public virtual void TouchEnd(Touch touch)
        {
        }

        public virtual void Restart()
        {
            launchTime = -0.2f;
        }

        protected virtual string GetClipName()
        {
            return Game.Choose("McSpringView_5", null, "McSpringViewWhite", null, "McSpringView_6");
        }

        protected virtual void SetSticked(ILaunchable value)
        {
            Sticked?.DestroyEvent.RemoveListener(OnTeleport);
            Sticked = value;
            Sticked?.DestroyEvent.AddListener(OnTeleport);
        }

        public void RefreshSmokeAngle()
        {
            smoke.Angle = new RandomRange(Clip.RotationDegrees + 90f, 20f);
        }

        protected virtual void CreateShadow()
        {
            Sprite node = new(Game.ChooseSide("common/McSpringShadow", "chapter4/McSpringShadowWhite", "common/McSpringShadow_5"));
            Builder.AddChildBefore(node, Clip);
            node.Position = Clip.Position;
            node.RotationRadians = Clip.RotationRadians;
            node.ScaleX = Clip.ScaleX;
            node.ScaleY = Clip.ScaleY;
        }

        protected virtual void RefreshPoints()
        {
            smoke.SmokePosition = Clip.LocalToNode(SmokePoint, Builder.GameRoot);
        }

        private static float OpacityStep()
        {
            return 200f;
        }

        public void SetThinSmokeRange()
        {
            SetSmokeRange(0f);
            smoke.MaxOpacity = 75f;
            smoke.OpacityStep = OpacityStep() / 6f;
            smoke.ScaleStep = 1.1666666f;
        }

        public void SetWhideSmokeRange()
        {
            SetSmokeRange(20f);
            smoke.MaxOpacity = 120f;
            smoke.OpacityStep = OpacityStep();
            smoke.ScaleStep = 7f;
        }

        public void SetSmokeRange(float range)
        {
            smoke.HorizontalPosition = new RandomRange(smoke.SmokePosition.X, range);
            smoke.VerticalPosition = new RandomRange(smoke.SmokePosition.Y, range);
        }

        public void CreateSmoke()
        {
            for (int i = 0; i < 10; i++)
            {
                smoke.AddParticle(Clip.Position).Visible = false;
            }
        }

        private void ShowSmoke()
        {
            smoke.ShowAllParticles();
        }

        public static bool CanLaunch(object bodyClip)
        {
            return bodyClip is ILaunchable launchable && launchable.CanLaunch();
        }

        private bool IsTouchDistance(Vector2 touchPosition)
        {
            return GetTouchDistance(touchPosition) < 1.1666666f;
        }

        private float GetTouchDistance(Vector2 touchPosition)
        {
            return Vector2.Distance(Body.GetWorldPoint(BodyCenterVec), touchPosition);
        }

        public void LaunchTouching()
        {
            for (ContactEdge val = Body.ContactList; val != null; val = val.Next)
            {
                if ((!val.Contact.FixtureA.IsSensor || !val.Contact.FixtureB.IsSensor) && (IsSticky(val.Contact.FixtureA) || IsSticky(val.Contact.FixtureB)) && val.Contact.IsTouching)
                {
                    object userData = val.Other.UserData;
                    if (userData != Sticked && CanLaunch(userData))
                    {
                        ApplyImpulseTo((ILaunchable)userData);
                    }
                }
            }
        }

        public void Launch()
        {
            Sticked.SetSpeedLocked(value: false);
            Sticked.HitEnabled = true;
            ContreJourGame.FocusOnHero();
            Sticked.Body.BodyType = (BodyType)2;
            ApplyImpulseTo(Sticked);
            launchTime = Game.TotalTime;
            SetSticked(null);
            SoundManager.PlaySound("perdelkaOut0", 0.7f);
            Spit();
            UserData.Instance.SpringShot++;
        }

        public void ApplyImpulseTo(ILaunchable bodyClip)
        {
            //IL_000c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0012: Invalid comparison between Unknown and I4
            float num = 1f;
            if ((int)bodyClip.Body.BodyType == 2)
            {
                float num2 = Vector2.Distance(WorldSuckPoint, bodyClip.Body.Position) - bodyClip.Radius();
                if (num2 > 0f)
                {
                    num = Math.Max(0f, (6.6666665f - num2) / 6.6666665f);
                }
            }
            ApplyImpulseTo(25f * num, bodyClip.Body);
        }

        public void ApplyImpulseTo(float impulse, Body body)
        {
            impulse = impulse * body.Mass * startScale;
            body.LinearVelocity = Vector2.Zero;
            body.ApplyLinearImpulse(VectorUtil.ToVector(impulse, MathHelper.ToRadians(Clip.RotationDegrees + 90f)), body.WorldCenter);
        }

        public void Spit()
        {
            SetWhideSmokeRange();
            ShowSmoke();
            movie.Rewind = true;
            movie.MinFrame = 0f;
            movie.MaxFrame = movie.TotalFrames;
            movie.Stoped = false;
        }

        public static float JointStep()
        {
            return 2f / 3f;
        }

        protected virtual void UpdateSticked()
        {
            //IL_000b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0011: Invalid comparison between Unknown and I4
            if ((int)Sticked.Body.BodyType == 2)
            {
                Vector2 worldSuckPoint = WorldSuckPoint;
                Vector2 vector = worldSuckPoint - Sticked.Body.Position;
                float num = vector.Length();
                vector.Normalize();
                vector *= Sticked.Body.Mass;
                vector *= Math.Min((suckDistance - num) * 100f, 200f);
                Sticked.Body.ApplyForce(vector, Sticked.Body.WorldCenter);
                Vector2 vector2 = VectorUtil.Rotate(new Vector2(1f, 0f), BodyAngle);
                float num2 = VectorUtil.Projection(Sticked.Body.LinearVelocity, vector2);
                float num3 = VectorUtil.Projection(worldSuckPoint - Sticked.Body.Position, vector2);
                if (num2 * num3 < 0f || (Math.Abs(num2) < 10f && Math.Abs(num3) > 1f))
                {
                    Vector2 vector3 = vector2;
                    vector3 *= num3;
                    vector3 *= Sticked.Body.Mass;
                    vector3 *= 200f;
                    Sticked.Body.ApplyForce(vector3, Sticked.Body.WorldCenter);
                }
                relativeStickedPosition = Sticked.Body.Position - Body.Position;
                relativeStickedPosition = VectorUtil.Rotate(relativeStickedPosition, 0f - BodyAngle);
                FixClosePosition(relativeStickedPosition);
                CheckFixed(num3);
            }
        }

        private void ApplyRelativePosition(Vector2 relativePosition)
        {
            Vector2 vector = VectorUtil.Rotate(relativePosition, BodyAngle);
            vector += Body.Position;
            Sticked.Body.SetTransform(vector, Sticked.Body.Rotation);
        }

        public void FixClosePosition(Vector2 relativePosition)
        {
            float num = relativePosition.Y - 2.2f;
            if (num > 0f && Math.Abs(relativePosition.X) * 2f > num)
            {
                relativePosition.X = Maths.StepTo(relativePosition.X, Math.Sign(relativePosition.X) * num / 2f, 0.3f);
                ApplyRelativePosition(relativePosition);
            }
            else if (num < 0f)
            {
                relativePosition.X = Maths.StepTo(relativePosition.X, 0f, 0.3f);
                ApplyRelativePosition(relativePosition);
            }
        }

        public void CheckFixed(float positionProjection)
        {
            if (Math.Abs(positionProjection) < 0.1f && CheckStickedBodyContacts("base"))
            {
                Sticked.Body.BodyType = (BodyType)1;
                Sticked.Body.LinearVelocity = Vector2.Zero;
                Sticked.Body.AngularVelocity = 0f;
            }
        }

        public override void Update(float time)
        {
            //IL_001a: Unknown result type (might be due to invalid IL or missing references)
            //IL_0020: Invalid comparison between Unknown and I4
            base.Update(time);
            if (Sticked != null)
            {
                if ((int)Sticked.Body.BodyType == 1 || CheckBodyContactsKeyCount(Sticked.Body, "sticky", 1))
                {
                    UpdateSticked();
                    UpdateSuckingClip();
                }
                else
                {
                    FixAnimation();
                    SetSticked(null);
                }
            }
            else if (movie.Stoped && Maths.FuzzyEquals(movie.CurrentFrame, 7f))
            {
                breatheChanger.Update(time);
                Clip.ScaleY = breatheChanger.Value * startScale;
                Clip.ScaleX = (2f - breatheChanger.Value) * startScale;
                if (timeToSmoke < 0f)
                {
                    timeToSmoke = Maths.Random(1.5f, 2.3f);
                    int num = 0;
                    while (smoke.Particles.Count < 10 && num < 3)
                    {
                        SetThinSmokeRange();
                        ((GravityParticle)smoke.AddOrGetInvisible()).Speed *= 1f / 6f;
                        num++;
                    }
                }
            }
            if (Maths.FuzzyEquals(movie.CurrentFrame, 0f))
            {
                movie.CurrentFrame = 7f;
                movie.Stoped = true;
            }
            timeToSmoke -= time;
        }

        private void ReverseSmoke()
        {
            smoke.ScaleStep = -10.5f;
            foreach (GravityParticle particle in smoke.Particles.Cast<GravityParticle>())
            {
                Vector2 speed = particle.Speed * -6f;
                float num = speed.Length();
                if (num > 300f)
                {
                    speed *= 300f / num;
                }
                particle.Speed = speed;
            }
        }

        public void UpdateSuckingClip()
        {
            //IL_000b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0011: Invalid comparison between Unknown and I4
            float num = ((int)Sticked.Body.BodyType == 2) ? Math.Min(1f, relativeStickedPosition.Y / suckDistance) : 0f;
            movie.CurrentFrame = 7f + Math.Max((5f * (1f - num)) - 1f, 0f);
            if (Maths.FuzzyEquals(num, 0f))
            {
                movie.Stoped = true;
            }
        }

        private void ProcessCollisionPoint(Body body2, Contact point)
        {
            if (Sticked == null && !(Game.TotalTime - launchTime < 0.2f))
            {
                object userData = body2.UserData;
                if (point.IsTouching && (!point.FixtureA.IsSensor || !point.FixtureB.IsSensor) && CanLaunch(userData) && (IsSticky(point.FixtureA) || IsSticky(point.FixtureB) || CheckBodyContactsKeyCount(body2, "base", 2)))
                {
                    ReverseSmoke();
                    ILaunchable launchable = (ILaunchable)userData;
                    SetSticked(launchable);
                    launchable.HitEnabled = false;
                    SoundManager.PlaySound("perdelkaOut0", 0.7f);
                }
            }
        }

        public override void OnCollisionStartPoint(Body body2, Contact point)
        {
            ProcessCollisionPoint(body2, point);
        }

        public override void OnCollisionPoint(Body body2, Contact point)
        {
            ProcessCollisionPoint(body2, point);
        }

        private void OnTeleport()
        {
            //IL_0013: Unknown result type (might be due to invalid IL or missing references)
            //IL_0019: Invalid comparison between Unknown and I4
            if (Sticked != null)
            {
                if ((int)Sticked.Body.BodyType == 1)
                {
                    Sticked.Body.BodyType = (BodyType)2;
                }
                Spit();
                SetSticked(null);
            }
        }

        public override void OnCollisionEndPoint(Body body2, Contact point)
        {
            if (Sticked != null && body2 == Sticked.Body && !Body.IsTouching(Sticked.Body))
            {
                FixAnimation();
                SetSticked(null);
            }
        }

        public void FixAnimation()
        {
            if (Maths.FuzzyNotEquals(movie.CurrentFrame, 7f))
            {
                movie.MinFrame = 7f;
                movie.Rewind = true;
                movie.Stoped = false;
            }
        }

        public static bool CheckBodyContactsKeyCount(Body target, string fixtureKey, int count)
        {
            ContactEdge val = target.ContactList;
            int num = 0;
            while (val != null)
            {
                if (val.Contact.IsTouching && (CheckFixtureKey(val.Contact.FixtureA, fixtureKey) || CheckFixtureKey(val.Contact.FixtureB, fixtureKey)))
                {
                    num++;
                }
                if (num == count)
                {
                    return true;
                }
                val = val.Next;
            }
            return false;
        }

        public bool CheckStickedBodyContacts(string fixtureKey)
        {
            return CheckBodyContactsKeyCount(Sticked.Body, fixtureKey, 2);
        }

        public static bool CheckFixtureKey(Fixture fixture, string key)
        {
            return fixture.UserData is Hashtable hashtable && hashtable.ContainsKey(key);
        }

        public static bool IsSticky(Fixture fixture)
        {
            return CheckFixtureKey(fixture, "sticky");
        }
    }
}
