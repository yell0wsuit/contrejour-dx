using System.Diagnostics.CodeAnalysis;
using System.Numerics;

using Mokus2D.Util.MathUtils;

namespace ContreJour.Gameplay
{
    public class ButterFly : FlyBase
    {
        private float horizontalStep;

        private readonly float step;

        [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Its random draws must run after the base constructor's, in this order.")]
        public ButterFly(Particle particle, float scale)
            : base(particle, scale)
        {
            step = Maths.Random(0.01f, 0.03f);
        }

        public override void Update(float time)
        {
            horizontalStep += step;
            TargetPosition = ChooseTarget();
            base.Update(time);
        }

        private Vector2 ChooseTarget()
        {
            return new Vector2(InitialPosition.X + (5f * (Maths.Sin(horizontalStep) + 1f)), InitialPosition.Y + (3.5f * Maths.Sin(VerticalStep)));
        }
    }
}
