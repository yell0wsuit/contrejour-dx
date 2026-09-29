using System.Numerics;

using ContreJour.Clips.common;

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;

using Mokus2D.Data;
using Mokus2D.Events;
using Mokus2D.Input;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public class DragableBodyClip : ContreJourBodyClip, IClickable, IRestartable, ISnotHolder
    {
        public const float RestartSpeed = 26.666666f;

        public const float MaxSpeed = 60f;

        public const float FixHeroSpeedX = 1f / 6f;

        public const float FixHeroSpeed = -1.3333334f;

        public const float AlphaStep = 5f;

        public const float MaxAlpha = 255f;

        public const float MinAlpha = 150f;

        public const float SpeedProportion = 0.5f;

        public const float MoveProportion = 0.5f;

        public const float FuzzyPrecission = 1f / 30f;

        public const float TouchDistanceIphone = 2.3333333f;

        public const float MaxTouchDistance = 1.6666666f;

        public const float OFFSET = 3.4f;

        private Vector2 axis;

        protected float CurrentAlpha { get; set; }
        private bool draging;

        private Vector2 initialDragOffset;

        private Vector2 initialMousePosition;

        protected Vector2 InitialPosition { get; set; }

        private bool limitSpeed;

        private readonly float lowerLimit;

        private McDragLimit middle;

        private Vector2 targetPosition;

        private Touch touch;

        private readonly float upperLimit;

        private readonly CircleShape _dragShape;

        private RectangleFloat _dragBounds;

        public EventSender DragStartEvent { get; }

        public SnotBodyClip Snot { get; set; }

        public bool DisableHeroFocus => false;

        public virtual Vector2 SnotPosition
        {
            get
            {
                Vector2 vector = VectorExtensions.Rotate(Builder.ToVec(new Vector2(0f, -60f)), 0f - RotationOffsetRadians);
                return Body.GetWorldPoint(vector);
            }
        }

        public DragableBodyClip(ContreJourLevelBuilder builder, object body, Node clip, Hashtable config)
            : base(builder, body, clip, config)
        {
            //IL_00f7: Unknown result type (might be due to invalid IL or missing references)
            //IL_0101: Expected O, but got Unknown
            clip = LevelBuilderBase.ReplaceClipWith(clip, ReplaceClipName(builder));
            Clip = clip;
            clip.Parent.ChangeChildLayer(clip, 2);
            DragStartEvent = new EventSender();
            float num = config.GetFloat("scaleX");
            InitialPosition = Body.Position;
            targetPosition = InitialPosition;
            upperLimit = 3.4f * num;
            lowerLimit = -3.4f * num;
            axis = VectorUtil.ToVector(1f, float.DegreesToRadians(0f - Config.GetFloat("rotation")));
            CreateBoundsClip(num);
            SetAlpha(150f);
            Body.BodyType = (BodyType)1;
            _dragShape = (CircleShape)Body.FixtureList.Find(f => !((CircleShape)f.Shape).Position.FuzzyEquals(Vector2.Zero, 0.1f)).Shape;
        }

        public bool UseForZoom()
        {
            return false;
        }

        public bool AcceptFreeTouches()
        {
            return true;
        }

        protected override void FirstUpdate()
        {
            _dragBounds = Game.LevelScreenPhysicsBounds;
            _dragBounds.Offset(-_dragShape.Position);
            _dragBounds.Extend(-2f / 3f);
        }

        public int Priority(Vector2 touchPosition)
        {
            return 1;
        }

        public bool TouchBegan(Touch touch)
        {
            if (this.touch == null && ProcessTouch(touch))
            {
                limitSpeed = false;
                this.touch = touch;
                DragStartEvent.SendEvent();
                ContreJourGame contreJourGame = (ContreJourGame)Builder.Game;
                contreJourGame.IncreaseZoomOut();
                Schedule(ContreJourGame.FocusOnHero, 0.05f);
                draging = true;
                initialMousePosition = Builder.TouchRootVec(this.touch);
                initialDragOffset = Body.Position - InitialPosition;
                return true;
            }
            return false;
        }

        public bool TouchMove(Touch touch)
        {
            return true;
        }

        public void TouchOut(Touch touch)
        {
        }

        public void TouchEnd(Touch touch)
        {
            draging = false;
            Builder.Game.DecreaseZoomOut();
            this.touch = null;
        }

        public void Restart()
        {
            targetPosition = InitialPosition;
            limitSpeed = true;
        }

        protected virtual string ReplaceClipName(ContreJourLevelBuilder builder)
        {
            return builder.ContreJour.ChooseSide(null, "McDragViewWhite", "McDragView_5", "McDragView_5");
        }

        protected virtual void CreateBoundsClip(float scale)
        {
            middle = new McDragLimit();
            Vector2 vector = VectorUtil.ToVector(upperLimit / (1f / 30f), float.DegreesToRadians(Clip.RotationDegrees));
            Vector2 position = Clip.Position;
            Builder.AddChildBefore(middle, Clip);
            float rotationDegrees = Clip.RotationDegrees;
            middle.Position = position;
            middle.RotationDegrees = rotationDegrees;
            middle.ScaleX = (vector.Length() + 40f) * 2f / middle.Size.X;
        }

        protected virtual Vector2 TouchOffset()
        {
            return Vector2.Zero;
        }

        public void SetAlpha(float value)
        {
            if (Maths.FuzzyNotEquals(CurrentAlpha, value))
            {
                CurrentAlpha = value;
                RefreshObjectsAlpha();
            }
        }

        protected virtual void RefreshObjectsAlpha()
        {
            middle.OpacityByte = (int)CurrentAlpha;
        }

        public bool ProcessTouch(Touch touch)
        {
            return Vector2.Distance(Builder.TouchRootVec(touch), Body.Position + TouchOffset()) < 2.3333333f;
        }

        public void UpdateTouchPosition()
        {
            targetPosition = GetDragPosition(Builder.TouchRootVec(touch) - initialMousePosition + initialDragOffset);
            targetPosition = _dragBounds.ClampToBounds(targetPosition);
        }

        private void MoveToTarget(float time)
        {
            if (Maths.FuzzyNotEquals(time, 0f) && !targetPosition.FuzzyEquals(Body.Position, 1f / 30f))
            {
                Vector2 vec = targetPosition - Body.Position;
                _ = limitSpeed ? VectorUtil.ClampLength(ref vec, 26.666666f * time) : VectorUtil.ClampLength(ref vec, 60f * time);
                Vector2 linearVelocity = vec;
                linearVelocity *= 0.5f / time;
                vec *= 0.5f;
                Body.LinearVelocity = linearVelocity;
                Body.SetTransform(vec + Body.Position, Body.Rotation);
            }
        }

        protected virtual Vector2 GetDragPosition(Vector2 offset)
        {
            float num = Maths.Clamp(VectorUtil.Projection(offset, axis), lowerLimit, upperLimit);
            return (axis * num) + InitialPosition;
        }

        public override void Update(float time)
        {
            //IL_003e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0044: Invalid comparison between Unknown and I4
            Body.LinearVelocity = Vector2.Zero;
            if (touch != null)
            {
                UpdateTouchPosition();
            }
            MoveToTarget(time);
            if (Maths.FuzzyNotEquals(time, 0f) && (int)Body.BodyType == 2)
            {
                Body.BodyType = 0;
                Body.SetTransform(InitialPosition, Body.Rotation);
            }
            SetAlpha(Maths.StepTo(target: draging ? 255f : 150f, value: CurrentAlpha, maxStep: 5f));
            base.Update(time);
        }
    }
}
