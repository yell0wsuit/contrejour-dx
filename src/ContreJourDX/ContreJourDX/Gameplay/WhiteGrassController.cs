using System;
using System.Numerics;

using Mokus2D.Util.MathUtils;

namespace ContreJourDX.Gameplay
{
    public class WhiteGrassController(PlasticinePartBodyClip plasticine) : GrassController(plasticine)
    {
        private bool borderUpdated;

        public override float SmallGrassScale => 0.7f;

        public override int GrassFrame => Maths.Random(3);

        public override int SmallGrassFrame => GrassFrame;

        public override float WindAngle => (float)Math.PI / 8f;

        public override float TrampleAngle => (float)Math.PI / 12f;

        public override float SmallGrassStep => base.SmallGrassStep / 2f;

        public override float GrassStep => base.GrassStep / 2f;

        public float SmallGrassOffset(int index)
        {
            return ((index * 2) - 1) * Plasticine.Width / 3f;
        }

        public override void Update(float time)
        {
            base.Update(time);
            if (!borderUpdated)
            {
                if (Plasticine.Item.PreviousItem.BodyClip.GrassController == null)
                {
                    GrassAndPosition grassAndPosition = SmallGrasses[0];
                    grassAndPosition.Position = new Vector2(grassAndPosition.Position.X, -7f * Builder.EngineConfig.SizeMultiplier);
                    grassAndPosition.Particle.Scale = 0.4f;
                }
                if (Plasticine.Item.NextItem.BodyClip.GrassController == null)
                {
                    GrassAndPosition grassAndPosition2 = SmallGrasses[1];
                    grassAndPosition2.Position = new Vector2(grassAndPosition2.Position.X, -7f * Builder.EngineConfig.SizeMultiplier);
                    grassAndPosition2.Particle.Scale = 0.4f;
                }
                borderUpdated = true;
            }
        }

        public override float GetSmallGrassOffset(int index)
        {
            return index != 0 ? Plasticine.Width / 2f : (0f - Plasticine.Width) / 2f;
        }
    }
}
