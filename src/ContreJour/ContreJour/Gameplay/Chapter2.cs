using System;

using ContreJour.Clips.planets;

using Microsoft.Xna.Framework;

using Mokus2D;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Particles.Util;

namespace ContreJour.Gameplay
{
    public class Chapter2(int index, MainMenu menu) : ChapterItem(index, menu)
    {
        private WhiteSmoke springSmoke;

        protected override void CreateSprites()
        {
            Background = new McPlanet2Background();
            BlurBackground = new McChapter2Blur();
            _ = CreateBouncingSprite("planets/McPlanetSpringBack", 45, new Vector2(-69f, 26f), 0.8f);
            _ = CreateBouncingSprite("planets/McPlanetSpringBack", -150, new Vector2(26f, -73f), 0.7f);
            Container.AddChild(Background);
            ParticleSystem particleSystem = new(Mokus2DGame.LoadSpriteData("common/McEnergyBall"));
            Container.AddChild(particleSystem);
            particleSystem.Scale = 0.55f;
            AddUpdating(new PlanetEnergy(particleSystem, new Vector2(-136f, -92f)));
            AlphaItems.Add(particleSystem);
            CreateSmoke();
            _ = CreateBouncingSprite("planets/McPlanetSpringView", -50, new Vector2(70f, 35f), 0.9f);
            _ = CreateBouncingSprite("planets/McPlanetSpringView", -190, new Vector2(-14f, -74f), 0.9f);
            _ = CreateBouncingSprite("planets/McPlanetSpringView", 90, new Vector2(-78f, -3f), 0.8f);
            PlanetSatellite planetSatellite = new();
            Container.AddChild(planetSatellite);
            AddUpdating(planetSatellite);
            AlphaItems.Add(planetSatellite);
        }

        protected BouncingSprite CreateBouncingSprite(string spriteName, int rotation, Vector2 position, float scale)
        {
            BouncingSprite bouncingSprite = new(spriteName)
            {
                RotationDegrees = rotation,
                Position = position,
                Scale = scale
            };
            bouncingSprite.MaxBounceEvent += OnSpringSpit;
            Container.AddChild(bouncingSprite);
            AddUpdating(bouncingSprite);
            return bouncingSprite;
        }

        public virtual string SmokeSprite()
        {
            return "common/McWhiteSmokeBlack";
        }

        public void CreateSmoke()
        {
            springSmoke = new WhiteSmoke(SmokeSprite());
            springSmoke.CreateOnStartPosition(20);
            springSmoke.HideAllParticles();
            springSmoke.OpacityStep = 70f;
            springSmoke.ScaleStep = 1f;
            springSmoke.MaxOpacity = 180f;
            springSmoke.Gravity = new Vector2(0f, 10f);
            springSmoke.ParticlesScale = new RandomRange(1.2f, 0.3f);
            AlphaItems.Add(springSmoke);
            Container.AddChild(springSmoke, 100);
        }

        public virtual Vector2 SmokeCoords()
        {
            return new Vector2(0f, 43f);
        }

        private void OnSpringSpit(BouncingSprite spring)
        {
            if (Maths.FuzzyEquals(Depth, 1f))
            {
                GravityParticle gravityParticle = (GravityParticle)springSmoke.AddOrGetInvisible();
                gravityParticle.Position = spring.LocalToNode(SmokeCoords(), this);
                gravityParticle.Speed = VectorUtil.ToVector(15f, XnaMath.ToRadians(spring.RotationDegrees) + ((float)Math.PI / 2f));
            }
        }
    }
}
