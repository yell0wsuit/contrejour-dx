using System;
using System.IO;

using ContreJourMono.ContreJour.Game.Hero;

using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace ContreJourMono.ContreJour.Menu.LevelComplete
{
    public class FakeHero : Node
    {
        protected static readonly string TextureFolder = "fakeHero";
        private readonly Sprite shadow;

        public HeroTail Tail { get; }

        protected virtual Color TailColor => Color.Black;

        public Sprite Background { get; }

        public FakeHeroEye Eye { get; }

        public float Speed
        {
            get; set
            {
                field = value;
                Tail.Speed = value;
            }
        }

        public Sprite HotSpot { get; }

        public new Vector2 Position
        {
            get => base.Position; set => base.Position = value;
        }

        public Vector2 ViewTarget
        {
            set
            {
                Vector2 vector = Parent.LocalToNode(value, this);
                Eye.ViewAngle = (float)Math.Atan2(vector.Y, vector.X);
                Eye.ViewDistance = vector.Length() / 200f;
            }
        }

        public FakeHero()
        {
            Background = new Sprite(ProcessName("McFakeHeroBackground"));
            shadow = new Sprite(ProcessName("McFakeHeroShadow"));
            HotSpot = new Sprite(ProcessName("McFakeHeroHotspot"));
            Tail = new HeroTail(TailColor);
            AddChild(Tail);
            Tail.LimitAngles = true;
            Tail.Scale = 2f;
            Tail.Speed = 0f;
            AddChild(shadow);
            AddChild(Background);
            AddChild(HotSpot);
            Eye = CreateEye();
            AddChild(Eye);
        }

        public void LookAt(Node node)
        {
            ViewTarget = node.LocalToNode(Vector2.Zero, Parent);
        }

        protected virtual string ProcessName(string name)
        {
            return Path.Combine([TextureFolder, name]);
        }

        protected virtual FakeHeroEye CreateEye()
        {
            return new FakeHeroEye();
        }

        public void SetMoveAngle(float angle, float speed)
        {
            Tail.SetMovementDirection(angle);
            Speed = speed;
        }

        public void SetViewAngle(float angle, float ratio)
        {
            Eye.ViewAngle = angle;
            Eye.ViewDistance = ratio;
        }

        public override void Update(float time)
        {
            base.Update(time);
        }
    }
}
