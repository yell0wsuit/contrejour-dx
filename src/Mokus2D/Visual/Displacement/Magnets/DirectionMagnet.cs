using Microsoft.Xna.Framework;

namespace Mokus2D.Visual.Displacement.Magnets
{
    public class DirectionMagnet(Vector2 size) : GridMagnetBase(size)
    {
        private Vector2 Direction = new(1f);

        public override Vector2 GetForce(Vector2 relativePosition)
        {
            return Power * Direction;
        }
    }
}
