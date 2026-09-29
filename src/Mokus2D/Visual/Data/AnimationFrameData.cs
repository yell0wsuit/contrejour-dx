using System;

using Microsoft.Xna.Framework;

namespace Mokus2D.Visual.Data
{
    [Serializable]
    public class AnimationFrameData
    {
        public string Id { get; set; }

        public Vector2 Position { get; set; }

        public float Rotation { get; set; }

        private Vector2 scale;

        public ref Vector2 Scale => ref scale;

        public float Alpha { get; set; }

        public Color Color { get; set; }

        public float ColorRatio { get; set; }

        public bool Visible { get; set; }
    }
}
