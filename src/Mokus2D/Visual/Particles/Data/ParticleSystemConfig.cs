using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.Serialization;

using Mokus2D.Graphics;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Particles.Util;

namespace Mokus2D.Visual.Particles.Data
{
    [DataContract]
    public class ParticleSystemConfig
    {
        [DataMember]
        public bool Test { get; set; }

        [DataMember]
        public bool BlendAdditive { get; set; }

        [DataMember]
        public List<string> ParticleIds { get; } = [];

        [DataMember]
        public float UpdateSpeed { get; set; } = 1f;

        [DataMember]
        public bool ReuseParticles { get; set; } = true;

        [field: NonSerialized]
        [DataMember]
        public Vector2 ParticlesPosition { get; set; }

        [field: NonSerialized]
        [DataMember]
        public Vector2 Gravity { get; set; } = Vector2.Zero;

        [field: NonSerialized]
        [DataMember]
        public Vector2 PositionRange { get; set; } = Vector2.Zero;

        [DataMember]
        public bool RadialPosition { get; set; } = true;

        [DataMember]
        public RandomRange StartSpeed { get; set; } = new(100f, 0f);

        [DataMember]
        public RandomRange StartSpeedAngle { get; set; } = new(0f, 0f);

        [DataMember]
        public bool EndSpeedEnabled { get; set; } = true;

        [DataMember]
        public RandomRange EndSpeed { get; set; } = new(0f, 0f);

        [DataMember]
        public RandomRange EndSpeedAngle { get; set; } = new(0f, 0f);

        [DataMember]
        public bool LockRotationToSpeed { get; set; }

        [DataMember]
        public RandomRange FadeInTime { get; set; } = new(0.1f, 0f);

        [DataMember]
        public RandomRange FadeOutTime { get; set; } = new(0.1f, 0f);

        [DataMember]
        public RandomRange LifeTime { get; set; }

        [DataMember]
        public RandomRange? LifeDistance { get; set; }

        [DataMember]
        public RandomRange StartScale { get; set; } = new(1f, 0f);

        [DataMember]
        public RandomRange EndScale { get; set; } = new(0.5f, 0f);

        [DataMember]
        public RandomRange StartOpacity { get; set; } = new(1f, 0f);

        [DataMember]
        public RandomRange EndOpacity { get; set; } = new(0.5f, 0f);

        [DataMember]
        public RandomRange CreateDelay { get; set; } = new(1f, 0f);

        [DataMember]
        public RandomRange StartRotation { get; set; } = new(0f, 0f);

        [DataMember]
        public RandomRange RotationSpeed { get; set; } = new(0f, 0f);

        [DataMember]
        public bool CanFlipX { get; set; }

        [DataMember]
        public bool CanFlipY { get; set; }

        // XmlSerializer reads this type and rejects a property with a private setter, so FadeColor stays a
        // get-only property over this field (the serializer skips get-only properties).
        [SuppressMessage("Style", "IDE0032:Use auto property", Justification = "XmlSerializer rejects a private setter.")]
        private bool _fadeColor;

        [NonSerialized]
        private Color _startColor = Color.White;

        [NonSerialized]
        private Color _endColor = Color.White;

        [DataMember]
        public int MaxParticles { get; set; }

        [DataMember]
        public Color EndColor
        {
            get => _endColor;
            set
            {
                if (_endColor != value)
                {
                    _endColor = value;
                    RefreshFadeColor();
                }
            }
        }

        [DataMember]
        public Color StartColor
        {
            get => _startColor;
            set
            {
                if (value != _startColor)
                {
                    _startColor = value;
                    RefreshFadeColor();
                }
            }
        }

        [DataMember]
        public bool FadeColor => _fadeColor;

        [DataMember]
        public bool IsEmpty => ParticleIds.Count == 0;

        private void RefreshFadeColor()
        {
            _fadeColor = StartColor != EndColor;
        }

        public ParticleData CreateParticleData()
        {
            Sprite particle = new(ParticleIds.RandomItem());
            ParticleData particleData = new(particle, this);
            Initialize(particleData);
            return particleData;
        }

        public void Initialize(ParticleData data)
        {
            Vector2 vector = (!RadialPosition) ? new Vector2(Maths.Random(-1f, 1f), Maths.Random(-1f, 1f)) : VectorUtil.ToVector(Maths.Random(1f), Maths.Random((float)Math.PI * 2f));
            data.Particle.Position = (vector * PositionRange) + ParticlesPosition;
            data.LifeTime = LifeTime.GetValueInRange();
            data.FadeInTime = FadeInTime.GetValueInRange();
            data.FadeOutTime = FadeOutTime.GetValueInRange();
            data.Speed = VectorUtil.ToVector(StartSpeed.GetValueInRange(), StartSpeedAngle.GetValueInRange());
            Vector2 vector2 = VectorUtil.ToVector(EndSpeed.GetValueInRange(), EndSpeedAngle.GetValueInRange());
            data.Acceleration = (vector2 - data.Speed) / (data.LifeTime + data.FadeInTime + data.FadeOutTime);
            data.LifeDistance = GetValueInRange(LifeDistance);
            Vector2 vector3 = new(1f);
            if (CanFlipX && Maths.Random(2) == 1)
            {
                vector3.X = -1f;
            }
            if (CanFlipY && Maths.Random(2) == 1)
            {
                vector3.Y = -1f;
            }
            data.StartScale = StartScale.GetValueInRange() * vector3;
            data.EndScale = EndScale.GetValueInRange() * vector3;
            data.Particle.ScaleVec = data.StartScale * vector3;
            data.Particle.OpacityFloat = 0f;
            data.StartOpacity = StartOpacity.GetValueInRange();
            data.EndOpacity = EndOpacity.GetValueInRange();
            data.Particle.Color = StartColor;
            data.Particle.RotationRadians = StartRotation.GetValueInRange();
            data.RotationSpeed = RotationSpeed.GetValueInRange();
            if (data.Particle is Sprite sprite)
            {
                sprite.Blend = BlendAdditive ? BlendMode.Additive : Mokus2DGame.Config.DefaultSpriteBatchProperties.Blend;
            }
            data.Initialize();
        }

        private static float? GetValueInRange(RandomRange? range)
        {
            return range.HasValue ? range.Value.GetValueInRange() : null;
        }

        public ParticleSystemConfig Clone()
        {
            ParticleSystemConfig particleSystemConfig = this.DeepClone();
            particleSystemConfig.ParticlesPosition = ParticlesPosition;
            particleSystemConfig.Gravity = Gravity;
            particleSystemConfig.PositionRange = PositionRange;
            particleSystemConfig._startColor = _startColor;
            particleSystemConfig._fadeColor = _fadeColor;
            return particleSystemConfig;
        }
    }
}
