using System.Numerics;

namespace ContreJour.Gameplay
{
    public class Box2DConfig
    {
        private Vector2 gravity;

        public Vector2 Gravity
        {
            get => gravity;
            set => gravity = value;
        }

        public int VelocityIterations { get; set; }

        public int PositionIterations { get; set; }

        public float SizeMultiplier { get; set; }

        public float Density { get; set; }

        public float Restitution { get; set; }

        public float Friction { get; set; }

        public static Box2DConfig DefaultConfig
        {
            get
            {
                field ??= new Box2DConfig();
                return field;
            }
        }

        public Box2DConfig()
        {
            gravity = new Vector2(0f, -10f);
            VelocityIterations = 20;
            PositionIterations = 20;
            SizeMultiplier = 1f / 30f;
            Density = 0.3f;
            Restitution = 0f;
            Friction = 1f;
        }

        public Vector2 ToPoint(Vector2 vec)
        {
            return vec / SizeMultiplier;
        }

        public Vector3 ToPoint(Vector3 vec)
        {
            return vec / SizeMultiplier;
        }

        public Vector2 ToVec(Vector2 point)
        {
            return point * SizeMultiplier;
        }
    }
}
