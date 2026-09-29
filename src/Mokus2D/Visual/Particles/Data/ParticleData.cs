using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace Mokus2D.Visual.Particles.Data
{
    public class ParticleData(Node particle, ParticleSystemConfig systemConfig) : IUpdatable
    {
        public Vector2 Speed { get; set; }

        public Vector2 Acceleration { get; set; }

        public float LifeTime { get; set; }

        public float FadeInTime { get; set; }

        public float FadeOutTime { get; set; }

        public float? LifeDistance { get; set; }

        public Vector2 StartScale { get; set; }

        public Vector2 EndScale { get; set; }

        public float StartOpacity { get; set; }

        public float EndOpacity { get; set; }

        public float RotationSpeed { get; set; }

        public Node Particle { get; } = particle;

        private readonly ParticleSystemConfig SystemConfig = systemConfig;

        private float _deathTime;

        private float _totalTime;

        public float Time { get; private set; }

        public bool HasRemove => CheckValue(LifeDistance) || Time >= _totalTime;

        public void Initialize()
        {
            Time = 0f;
            _deathTime = LifeTime + FadeInTime;
            _totalTime = _deathTime + FadeOutTime;
        }

        public void Update(float time)
        {
            Vector2 vector = Speed * time;
            Speed += SystemConfig.Gravity * time;
            if (SystemConfig.EndSpeedEnabled)
            {
                Speed += Acceleration * time;
            }
            Particle.Position += vector;
            if (LifeDistance.HasValue)
            {
                LifeDistance -= vector.Length();
            }
            Time += time;
            if (Time < FadeInTime)
            {
                Particle.OpacityFloat = (Time / FadeInTime).Lerp(0f, StartOpacity);
            }
            else if (Time < _deathTime)
            {
                float amount = (Time - FadeInTime) / LifeTime;
                Particle.OpacityFloat = amount.Lerp(StartOpacity, EndOpacity);
            }
            else
            {
                Particle.OpacityFloat = ((Time - _deathTime) / FadeOutTime).Lerp(EndOpacity, 0f);
            }
            float amount2 = Time / _totalTime;
            if (SystemConfig.FadeColor)
            {
                Particle.Color = Color.Lerp(SystemConfig.StartColor, SystemConfig.EndColor, amount2);
            }
            Particle.ScaleVec = XnaMath.Lerp(StartScale, EndScale, amount2);
            if (SystemConfig.LockRotationToSpeed)
            {
                Particle.RotationRadians = Speed.Atan2();
            }
            else
            {
                Particle.RotationRadians += RotationSpeed * time;
            }
        }

        private static bool CheckValue(float? value)
        {
            return value.HasValue && value <= 0f;
        }
    }
}
