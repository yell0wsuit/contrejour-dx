using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D;
using Mokus2D.Util.MathUtils;

namespace ContreJour.Gameplay
{
    public class Portal : ParticleSystem
    {
        protected List<Satellite> Parts { get; set; }

        private float targetScale;

        private float itemsScale;

        public float TargetScale
        {
            get => targetScale;
            set
            {
                if (targetScale != value)
                {
                    targetScale = value;
                    if (targetScale != itemsScale)
                    {
                        Visible = true;
                    }
                }
            }
        }

        public float SpeedValue
        {
            get => Parts[0].SpeedValue;
            set
            {
                for (int i = 0; i < 5; i++)
                {
                    Parts[i].SpeedValue = value;
                }
            }
        }

        public float ItemsScale
        {
            get => itemsScale;
            set
            {
                if (Maths.FuzzyNotEquals(itemsScale, value))
                {
                    itemsScale = value;
                    targetScale = value;
                }
            }
        }

        public float ScaleStep { get; set; }

        public Portal(ContreJourGame game, Vector2 position)
            : this(game, position, "common/McFinishPart")
        {
        }

        public Portal(ContreJourGame game, Vector2 position, string textureName)
            : base(Mokus2DGame.LoadSpriteData(textureName))
        {
            Parts = [];
            Blend = BlendState.Additive;
            ScaleStep = 0.05f;
            for (int i = 0; i < 5; i++)
            {
                Satellite item = new(game, AddParticle(), null, (float)Math.PI * 2f / 5f * i, position);
                Parts.Add(item);
            }
            SpeedValue = 40f;
            itemsScale = 1f;
            targetScale = 2f;
        }

        public override void Update(float time)
        {
            base.Update(time);
            if (Maths.FuzzyNotEquals(itemsScale, targetScale))
            {
                itemsScale = Maths.StepTo(itemsScale, targetScale, ScaleStep * time * 30f);
            }
            Visible = itemsScale > 0f;
        }

        public override void UpdateParticleTime(Particle particle, float time)
        {
            base.UpdateParticleTime(particle, time);
            particle.Scale = itemsScale;
        }
    }
}
