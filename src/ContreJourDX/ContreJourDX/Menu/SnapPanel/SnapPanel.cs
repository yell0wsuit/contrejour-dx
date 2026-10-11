using System;
using System.Collections.Generic;
using System.Numerics;

using ContreJourDX.Gameplay;

using Mokus2D;
using Mokus2D.Graphics;
using Mokus2D.Input;
using Mokus2D.Visual;
using Mokus2D.Visual.Particles.Util;

namespace ContreJourDX.Menu.SnapPanel
{
    public class SnapPanel : AccelerometerNode, ITouchListener
    {
        private readonly Sprite background;

        private readonly Dictionary<Node, Tuple<float, Vector2>> accelerometerChildren = [];

        public Vector2 Size => background.Size * 0.8f;

        public SnapPanel()
        {
            MaxAccOffset = new Vector2(1f, 0.6f);
            background = AddAccelerometerChild("Win8Background", 70f, Vector2.Zero);
        }

        public void Initialize(Vector2 visibleSize)
        {
            _ = AddAccelerometerChild("Win8Background1", 35f, new Vector2(40f, -360f));
            AddAccelerometerChild("Win8Background2", 29f, new Vector2(0f, (visibleSize.Y / 2f) - 40f)).Anchor = new Vector2(0.5f, 0f);
            AddAccelerometerChild(new SnapParticles(50f), 20f, new Vector2(-100f, -460f));
            _ = AddAccelerometerChild("Win8Petit", 18f, new Vector2(30f, -144f));
            AddAccelerometerChild(new SnapParticles(30f), 20f, new Vector2(-130f, -500f));
            SidePanelParticles sidePanelParticles = new();
            AddAccelerometerChild(sidePanelParticles, -10f, Vector2.Zero);
            sidePanelParticles.BottomLeftBound = ((-visibleSize) / 2f) - new Vector2(150f, 200f);
            sidePanelParticles.TopRightBound = (visibleSize / 2f) + new Vector2(50f, 200f);
            sidePanelParticles.HorizontalPosition = new RandomRange(0f, 0.5f * visibleSize.X);
            sidePanelParticles.VerticalPosition = new RandomRange((visibleSize.Y / 2f) + 30f, 0f);
            sidePanelParticles.CreateBetweenBounds(40);
            AddAccelerometerChild("Win8TopLeafs", -20f, new Vector2(0f, (visibleSize.Y / 2f) + 30f)).Anchor = new Vector2(0.5f, 0f);
            AddAccelerometerChild("Win8BottomLeafs", -20f, new Vector2(0f, ((0f - visibleSize.Y) / 2f) - 30f)).Anchor = new Vector2(0.5f, 1f);
            Sprite topLeft = CreateCorner(visibleSize, new Vector2(-1f, 1f), 0f);
            _ = CreateCorner(visibleSize, new Vector2(1f, 1f), 270f);
            _ = CreateCorner(visibleSize, new Vector2(1f, -1f), 180f);
            _ = CreateCorner(visibleSize, new Vector2(-1f, -1f), 90f);
            _ = CreateBorder(visibleSize, new Vector2(0f, 1f), topLeft, 0f, visibleSize.X);
            _ = CreateBorder(visibleSize, new Vector2(1f, 0f), topLeft, 270f, visibleSize.Y);
            _ = CreateBorder(visibleSize, new Vector2(0f, -1f), topLeft, 180f, visibleSize.X);
            _ = CreateBorder(visibleSize, new Vector2(-1f, 0f), topLeft, 90f, visibleSize.Y);
        }

        private Sprite CreateBorder(Vector2 visibleSize, Vector2 relativePosition, Sprite topLeft, float rotation, float sideSize)
        {
            Sprite sprite = new("Win8FramePart")
            {
                Position = visibleSize * relativePosition / 2f,
                Anchor = new Vector2(0.5f, 0f),
                RotationDegrees = rotation
            };
            sprite.ScaleX = (sideSize - (topLeft.Size.X * 2f)) / sprite.Size.X;
            AddChild(sprite);
            return sprite;
        }

        private Sprite CreateCorner(Vector2 visibleSize, Vector2 relativePosition, float rotation)
        {
            Sprite sprite = new("Win8FrameAngle")
            {
                Anchor = Vector2.Zero,
                Position = visibleSize * relativePosition / 2f,
                RotationDegrees = rotation,
                Color = Color.Red
            };
            AddChild(sprite);
            return sprite;
        }

        private Sprite AddAccelerometerChild(string name, float offset, Vector2 position)
        {
            Sprite sprite = new(name);
            AddAccelerometerChild(sprite, offset, position);
            return sprite;
        }

        private void AddAccelerometerChild(Node child, float offset, Vector2 position)
        {
            AddChild(child);
            accelerometerChildren[child] = new Tuple<float, Vector2>(offset, position);
            child.Position = position;
        }

        protected override void OnAddedToStage()
        {
            base.OnAddedToStage();
            Mokus2DGame.Instance.TouchController.AddListener(this);
        }

        protected override void OnRemovedFromStage()
        {
            base.OnRemovedFromStage();
            Mokus2DGame.Instance.TouchController.RemoveListener(this);
        }

        public override void Update(float time)
        {
            base.Update(time);
            foreach (KeyValuePair<Node, Tuple<float, Vector2>> accelerometerChild in accelerometerChildren)
            {
                accelerometerChild.Key.Position = accelerometerChild.Value.Item2 + (accelerometerChild.Value.Item1 * AccelerometerOffset);
            }
        }

        public bool TouchBegin(Touch touch)
        {
            return true;
        }

        public bool TouchMove(Touch touch)
        {
            return true;
        }

        public void TouchEnd(Touch touch)
        {
            Visible = false;
        }
    }
}
