using System;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace ContreJour.Gameplay
{
    public class PlasticineItem : LinkedListItem
    {
        private Vector2 initialPosition = Vector2.Zero;
        private Vector2 innerPosition;

        private Vector2 outerPosition;

        private Vector2 normalVec;

        private readonly float ortoAngle;

        private Vector2 ortogonalVec;

        public float InitialAngle { get; }

        public Vector2 InitialPosition => initialPosition;

        private float Width { get; }

        public PlasticineItem NextItem => (PlasticineItem)Next;

        public PlasticineItem PreviousItem => (PlasticineItem)Previous;

        public Body Body => BodyClip.Body;

        public PlasticinePartBodyClip BodyClip => (PlasticinePartBodyClip)Item;

        public PlasticineItem(PlasticinePartBodyClip bodyClip, float width)
            : base(bodyClip)
        {
            bodyClip.Item = this;
            bodyClip.SetDirty();
            Width = width;
            initialPosition = Body.Position;
            InitialAngle = Body.Rotation;
            innerPosition = GetBorderVec(-1f / 3f);
            outerPosition = GetBorderVec(1.3333334f);
            normalVec = outerPosition - innerPosition;
            ortoAngle = 0f - Maths.Atan2(normalVec.Y, normalVec.X) + ((float)Math.PI / 2f);
            ortogonalVec = normalVec.Rotate90();
        }

        public void UpdateTouchesBodyClipDistance(float offset, BodyClip objectP, float maxDistance)
        {
            BodyClip.OnTouchWith(offset, objectP);
            PlasticineItem previousItem = PreviousItem;
            float num;
            for (num = (Width / 2f) + offset + (previousItem.Width / 2f); num < maxDistance; num += previousItem.Width / 2f)
            {
                previousItem.BodyClip.OnTouchWith(num, objectP);
                num += previousItem.Width / 2f;
                previousItem = previousItem.PreviousItem;
            }
            previousItem = NextItem;
            num = offset - (Width / 2f) - (previousItem.Width / 2f);
            while (Math.Abs(num) < maxDistance)
            {
                previousItem.BodyClip.OnTouchWith(num, objectP);
                num -= previousItem.Width / 2f;
                previousItem = previousItem.NextItem;
                num -= previousItem.Width / 2f;
            }
        }

        public void SetTargetPointAngle(Vector2 touchPosition, float angle)
        {
            BodyClip.SetTargetPositionAngle(touchPosition, angle);
        }

        public Vector2 GetTargetPoint(Vector2 touchPosition)
        {
            Vector2 vector = touchPosition;
            vector -= initialPosition;
            float num = (!(VectorUtil.Projection(vector, BodyClip.Normal) < 0f)) ? 1.3333334f : (1f / 3f);
            float value = 0f - VectorUtil.Projection(vector, ortogonalVec);
            if (Math.Abs(value) > 0.4f)
            {
                vector = VectorUtil.Rotate(new Vector2(y: VectorUtil.Projection(vector, normalVec), x: Maths.Clamp(value, -0.4f, 0.4f)), 0f - ortoAngle);
            }
            if (vector.Length() <= num)
            {
                return initialPosition + vector;
            }
            vector.Normalize();
            return (vector * num) + initialPosition;
        }

        public Vector2 GetSurfaceCenterVec()
        {
            return BodyClip.GetSurfaceCenter();
        }

        public Vector2 GetLeftOffset(float offset)
        {
            return BodyClip.Builder.ToPoint(BodyClip.GetLeftOffset(offset));
        }

        public Vector2 GetRightOffset(float offset)
        {
            return BodyClip.Builder.ToPoint(BodyClip.GetRightOffset(offset));
        }

        public Vector2 GetCenterOffset(float offset)
        {
            return BodyClip.Builder.ToPoint(BodyClip.GetCenterOffset(offset));
        }

        public Vector2 GetSurfaceCenter()
        {
            Vector2 surfaceCenter = BodyClip.GetSurfaceCenter();
            return BodyClip.Builder.ToPoint(surfaceCenter);
        }

        public Vector2 GetLeft()
        {
            return Body.GetWorldPoint(new Vector2((0f - Width) / 2f, 0f));
        }

        public Vector2 GetRight()
        {
            return Body.GetWorldPoint(new Vector2(Width / 2f, 0f));
        }

        public Vector2 GetBorderVec(float offset)
        {
            return Body.GetWorldPoint(new Vector2(Width / 2f, (5f / 12f) + offset));
        }

        public Vector2 GetBorder(float offset)
        {
            return BodyClip.Builder.EngineConfig.ToPoint(GetBorderVec(offset));
        }

        public Vector2 GetSurfacePosition(float horizontalOffset)
        {
            return Body.GetWorldPoint(new Vector2(horizontalOffset, 7f / 12f));
        }

        public Vector2 GetLeftSurfacePosition()
        {
            return Body.GetWorldPoint(GetLeftSurfacePositionLocal());
        }

        public Vector2 GetNextAnchorPosition()
        {
            return Body.GetWorldPoint(new Vector2(Width * 2f, 0f));
        }

        public Vector2 GetPreviuosAnchorPosition()
        {
            return Body.GetWorldPoint(new Vector2((0f - Width) * 2f, 0f));
        }

        public Vector2 GetRightSurfacePosition()
        {
            return Body.GetWorldPoint(GetRightSurfacePositionLocal());
        }

        public Vector2 GetRightSurfacePositionLocal()
        {
            return new Vector2(Width / 2f, 7f / 12f);
        }

        public Vector2 GetLeftSurfacePositionLocal()
        {
            return new Vector2((0f - Width) / 2f, 7f / 12f);
        }
    }
}
