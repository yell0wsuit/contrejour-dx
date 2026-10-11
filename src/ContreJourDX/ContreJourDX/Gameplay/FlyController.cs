using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

using Mokus2D.Util.MathUtils;

namespace ContreJourDX.Gameplay
{
    public class FlyController : FlyBase
    {
        protected const float MinVerticalOffset = -5f;

        protected const float MaxVerticalOffset = 20f;

        protected const float MaxHorizontalOffset = 20f;

        private readonly IWindManager windProvider;

        private readonly IGrassControllerContainer grassControllerContainer;

        private readonly float horizontalOffset;

        private float initialGroundY;

        private Vector2 scareOffset;

        private float scareTime;

        private int scared;

        private readonly float verticalOffset;

        private readonly float windOffset;

        [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Its random draws must run after the base constructor's, in this order.")]
        public FlyController(IWindManager windProvider, IGrassControllerContainer grassControllerContainer, Particle particle, float? scale = null, float? windOffsetRange = null)
            : base(particle, scale ?? Maths.Random(0.8f, 1.2f))
        {
            this.windProvider = windProvider;
            initialGroundY = -1f;
            this.grassControllerContainer = grassControllerContainer;
            scareOffset = new Vector2(Maths.Random(10f, 30f), Maths.Random(30f, 50f));
            scared = 0;
            scareTime = 0f;
            windOffset = Maths.Random((0f - windOffsetRange) ?? (-0.5f), windOffsetRange ?? 0.5f);
            horizontalOffset = Maths.Random(10f, 20f);
            // The value is unused, but the draw keeps the shared random sequence unchanged.
            _ = Maths.Random(1f, 2f);
            verticalOffset = Maths.Random(-5f, 20f);
        }

        public void Scare(int direction)
        {
            if (scared == 0)
            {
                scared = direction;
            }
            scareTime = Maths.Random(0.5f, 2.5f);
        }

        public override void Update(float time)
        {
            if (initialGroundY == -1f)
            {
                initialGroundY = grassControllerContainer.GrassController.Y;
            }
            TargetPosition = ChooseTarget();
            if (scared != 0)
            {
                TargetPosition.X += scared * scareOffset.X;
                TargetPosition.Y += scareOffset.Y;
                StepY = Math.Abs(Particle.Position.Y - TargetPosition.Y) / scareOffset.Y;
                scareTime -= time;
                if (scareTime <= 0f)
                {
                    Unscare();
                }
            }
            base.Update(time);
        }

        private Vector2 ChooseTarget()
        {
            return new Vector2(InitialPosition.X + (horizontalOffset * windProvider.WindManager.GetWind(windOffset)), InitialPosition.Y + (verticalOffset * Maths.Sin(VerticalStep)) + (grassControllerContainer.GrassController.Y - initialGroundY));
        }

        public void Unscare()
        {
            scared = 0;
        }
    }
}
