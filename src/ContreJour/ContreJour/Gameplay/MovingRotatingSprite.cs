using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

using Mokus2D.Util.MathUtils;

namespace ContreJour.Gameplay
{
    public class MovingRotatingSprite : RotatingSprite
    {
        private readonly CosChanger changer;

        private Vector2 initialPosition;

        public override Vector2 Position
        {
            set
            {
                base.Position = value;
                initialPosition = value;
            }
        }

        [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Its random draws must run after the base constructor's, in this order.")]
        public MovingRotatingSprite(string filename)
            : base(filename)
        {
            changer = new CosChanger(0f, 0f);
        }

        public override void Update(float time)
        {
            base.Update(time);
            changer.Update(time);
            base.Position = initialPosition + new Vector2(changer.Value, 0f);
        }

        public void Initialize(float amplitude, float progress)
        {
            changer.MinValue = 0f - amplitude;
            changer.MaxValue = amplitude;
            changer.Step = (float)Math.PI;
            changer.Progress = progress;
        }
    }
}
