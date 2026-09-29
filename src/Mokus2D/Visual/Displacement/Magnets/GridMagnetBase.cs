using System;

using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;
using Mokus2D.Visual.Displacement.Magnets.Actions;

namespace Mokus2D.Visual.Displacement.Magnets
{
    public abstract class GridMagnetBase : IGridMagnet, IUpdatable
    {
        public float Power { get; set; } = 1f;

        public float? MaxPower { get; set; }

        public float? MinPower { get; set; }

        public MagnetAction Action { get; set; }

        public bool HasRemove => Action != null && Action.Finished;

        public Rectangle Bounds { get; protected set; }

        public Vector2 Position { get; set; }

        public abstract Vector2 GetForce(Vector2 relativePosition);

        protected GridMagnetBase(float power = 1f)
        {
            Power = power;
        }

        protected GridMagnetBase()
        {
        }

        protected GridMagnetBase(Rectangle bounds)
        {
            Bounds = bounds;
        }

        protected GridMagnetBase(Vector2 size)
        {
            Bounds = new Rectangle(0, 0, (int)size.X, (int)size.Y);
        }

        public virtual void Update(float time)
        {
            Action?.Update(time);
        }

        protected float LimitPower(float currentPower)
        {
            if (MaxPower.HasValue)
            {
                currentPower = Math.Min(MaxPower.Value, currentPower);
            }
            if (MinPower.HasValue)
            {
                currentPower = Math.Max(MinPower.Value, currentPower);
            }
            return currentPower;
        }
    }
}
