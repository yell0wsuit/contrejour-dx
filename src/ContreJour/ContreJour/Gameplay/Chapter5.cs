using System;
using System.Collections.Generic;
using System.Numerics;

using ContreJour.Clips.common;
using ContreJour.Clips.menu2;
using ContreJour.Config;

using Mokus2D;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public class Chapter5(int index, MainMenu menu) : ChapterItem(index, menu)
    {
        private Node foreground;

        private Node foregroundContainer;

        private readonly List<CosPropertyChanger> changers = [];

        private Node planetForeground;

        private Sprite ocean;

        protected override void CreateSprites()
        {
            float scale = 2.5f;
            Background = new McPlanet5Background();
            Container.AddChild(Background);
            Background.Scale = scale;
            ocean = new McPlanet5Ocean();
            Container.AddChild(ocean);
            ocean.Scale = scale;
            CreateLianas();
            planetForeground = new McPlanet5Foreground();
            Container.AddChild(planetForeground);
            BlurBackground = new McPlanet5Blur();
            for (int i = 1; i <= 4; i++)
            {
                Node node = new Sprite($"menu2/McHole{i}");
                Container.AddChild(node);
                CosOpacityChanger item = new(node, 0f, 1f, Maths.Random(0.02f, 0.07f) / 255f);
                changers.Add(item);
            }
            ParticleSystem particleSystem = new(Mokus2DGame.LoadSpriteData("common/McEnergyBall"));
            Container.AddChild(particleSystem);
            AddUpdating(new PlanetEnergy(particleSystem, new Vector2(35f, 120f)));
            particleSystem.Scale = 0.55f;
            AlphaItems.Add(particleSystem);
            CreateForegrounds();
        }

        public void CreateLianas()
        {
            AddLianaMiddleEnd(new Vector2(-25f, -103f), new Vector2(15f, -139f), new Vector2(57f, -64f));
            AddLianaMiddleEndReduce(new Vector2(-116f, 37f), new Vector2(-114f, 10f), new Vector2(-72f, 0f), reduce: true);
            AddLianaMiddleEndReduce(new Vector2(-109f, 27f), new Vector2(-103f, -7f), new Vector2(-79f, -21f), reduce: true);
            AddLianaMiddleEnd(new Vector2(26f, -99f), new Vector2(60f, -111f), new Vector2(77f, -29f));
            AddLianaMiddleEndReduce(new Vector2(-90f, 45f), new Vector2(-64f, 78f), new Vector2(-43f, 66f), reduce: true);
            AddLianaMiddleEnd(new Vector2(-37f, -123f), new Vector2(-6f, -165f), new Vector2(30f, -92f));
        }

        public void CreateForegrounds()
        {
            Vector2 rootSize = ContreJourConfig.RootSize;
            foreground = new Node();
            foregroundContainer = new Node
            {
                Visible = false
            };
            foregroundContainer.AddChild(foreground);
            foreground.Position = (-rootSize) / 2f;
            foregroundContainer.Position = -foreground.Position;
            Menu.AddForeground(foregroundContainer);
            _ = AddForegroundPositionScaleAngle(new McLeafView4(), new Vector2(rootSize.X - 1024f + 624f, rootSize.Y - 27f), new Vector2(1.72f, 1.29f), 171f);
            _ = AddForegroundPositionScaleAngle(new McLeafView3(), new Vector2(rootSize.X - 1024f + 731f, rootSize.Y - 54f), new Vector2(2.37f, 2.37f), -172f);
            _ = AddForegroundPositionScaleAngle(new McLeafView5(), new Vector2(rootSize.X - 1024f + 1008f, rootSize.Y - 55f), new Vector2(3.31f, 3.31f), -22f);
            _ = AddForegroundPositionScaleAngleRotationOffset(new McLeafView1(), new Vector2(rootSize.X - 1024f + 1095f, rootSize.Y - 124f), new Vector2(2.71f, 2.71f), 0f, 5f);
            _ = AddForegroundPositionScaleAngleRotationOffset(new McLeafView0(), new Vector2(rootSize.X - 1024f + 1113f, rootSize.Y - 193f), new Vector2(2.38f, 2.38f), -30f, -3f);
            _ = AddForegroundPositionScaleAngleRotationOffset(new McLeafView0(), new Vector2(rootSize.X - 1024f + 1072f, 168f), new Vector2(2.45f, 2.45f), -52f, 1f);
            _ = AddForegroundPositionScaleAngleRotationOffset(new McLeafView2(), new Vector2(rootSize.X - 1024f + 1092f, 207f), new Vector2(2.17f, 2.17f), -14f, -3f);
            _ = AddForegroundPositionScaleAngle(new McLeafView4(), new Vector2(409f, 42f), new Vector2(-1.7f, 1.4f), 0f);
            _ = AddForegroundPositionScaleAngle(new McLeafView3(), new Vector2(340f, 48f), new Vector2(-2.68f, 2.68f), 4f);
            _ = AddForegroundPositionScaleAngle(new McLeafView5(), new Vector2(262f, 45f), new Vector2(-2.13f, 2.13f), 0f);
            _ = AddForegroundPositionScaleAngleRotationOffset(new McLeafView2(), new Vector2(-124f, rootSize.Y - 402f), new Vector2(-2.97f, 2.97f), 0f, 4f);
            _ = AddForegroundPositionScaleAngleRotationOffset(new McLeafView1(), new Vector2(-182f, rootSize.Y - 364f), new Vector2(-2.76f, 2.76f), -12f, -3f);
        }

        public void AddLianaMiddleEndReduce(Vector2 start, Vector2 middle, Vector2 end, bool reduce)
        {
            PlanetLiana planetLiana = new(start, middle, end);
            Container.AddChild(planetLiana);
            AddUpdating(planetLiana);
            if (reduce)
            {
                planetLiana.ReduceRange();
            }
            AddAlphaItem(planetLiana.Sprite);
        }

        public void AddLianaMiddleEnd(Vector2 start, Vector2 middle, Vector2 end)
        {
            AddLianaMiddleEndReduce(start, middle, end, reduce: false);
        }

        protected override void RefreshDepth()
        {
            base.RefreshDepth();
            foreground.OpacityFloat = Math.Max((Depth - 0.8f) * 5f, 0f);
            foregroundContainer.Visible = foreground.OpacityByte > 0;
            if (foregroundContainer.Visible)
            {
                foregroundContainer.Scale = 10f - ((Depth - 0.8f) * 5f * 9f);
            }
        }

        public override void Update(float time)
        {
            base.Update(time);
            foreach (CosPropertyChanger changer in changers)
            {
                changer.Update(time);
            }
            ocean.RotationDegrees += time * 11f;
            Background.RotationDegrees -= time * 7f;
        }

        private Node AddForegroundPositionScaleAngleRotationOffset(Node node, Vector2 position, Vector2 scale, float angle, float _offset)
        {
            _ = AddForegroundPositionScaleAngle(node, position, scale, angle);
            CosRotationChanger item = new(node, _offset, Maths.Random(0.005f, 0.01f));
            changers.Add(item);
            return node;
        }

        public Node AddForegroundPositionScaleAngle(Node node, Vector2 position, Vector2 scale, float angle)
        {
            node.Position = position;
            node.ScaleVec = scale;
            node.RotationDegrees = angle;
            foreground.AddChild(node);
            return node;
        }
    }
}
