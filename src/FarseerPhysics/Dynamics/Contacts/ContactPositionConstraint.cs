using FarseerPhysics.Collision;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Contacts
{
    public sealed class ContactPositionConstraint
    {
        public Vector2[] LocalPoints { get; set; } = new Vector2[2];

        public Vector2 LocalNormal { get; set; }

        public Vector2 LocalPoint { get; set; }

        public int IndexA { get; set; }

        public int IndexB { get; set; }

        public float InvMassA { get; set; }

        public float InvMassB { get; set; }

        public Vector2 LocalCenterA { get; set; }

        public Vector2 LocalCenterB { get; set; }

        public float InvIA { get; set; }

        public float InvIB { get; set; }

        public ManifoldType Type { get; set; }

        public float RadiusA { get; set; }

        public float RadiusB { get; set; }

        public int PointCount { get; set; }
    }
}
