using System.Numerics;

namespace ContreJour.Gameplay
{
    public class GrassAndPosition(Particle particle, Vector2 position)
    {
        private Vector2 position = position;

        public Particle Particle { get; } = particle;

        public Vector2 Position
        {
            get => position;
            set => position = value;
        }
    }
}
