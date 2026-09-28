using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Default.Namespace;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Particles.Util;

namespace Mokus2D.Visual.Particles.Data;

[DataContract]
public class ParticleSystemConfig : ICloneable<ParticleSystemConfig>
{
    [DataMember]
    public bool Test;

    [DataMember]
    public bool BlendAdditive;

    [DataMember]
    public readonly List<string> ParticleIds = [];

    [DataMember]
    public float UpdateSpeed = 1f;

    [DataMember]
    public bool ReuseParticles = true;

    [NonSerialized]
    [DataMember]
    public Vector2 ParticlesPosition;

    [NonSerialized]
    [DataMember]
    public Vector2 Gravity = Vector2.Zero;

    [NonSerialized]
    [DataMember]
    public Vector2 PositionRange = Vector2.Zero;

    [DataMember]
    public bool RadialPosition = true;

    [DataMember]
    public RandomRange StartSpeed = new(100f, 0f);

    [DataMember]
    public RandomRange StartSpeedAngle = new(0f, 0f);

    [DataMember]
    public bool EndSpeedEnabled = true;

    [DataMember]
    public RandomRange EndSpeed = new(0f, 0f);

    [DataMember]
    public RandomRange EndSpeedAngle = new(0f, 0f);

    [DataMember]
    public bool LockRotationToSpeed;

    [DataMember]
    public RandomRange FadeInTime = new(0.1f, 0f);

    [DataMember]
    public RandomRange FadeOutTime = new(0.1f, 0f);

    [DataMember]
    public RandomRange LifeTime;

    [DataMember]
    public RandomRange? LifeDistance;

    [DataMember]
    public RandomRange StartScale = new(1f, 0f);

    [DataMember]
    public RandomRange EndScale = new(0.5f, 0f);

    [DataMember]
    public RandomRange StartOpacity = new(1f, 0f);

    [DataMember]
    public RandomRange EndOpacity = new(0.5f, 0f);

    [DataMember]
    public RandomRange CreateDelay = new(1f, 0f);

    [DataMember]
    public RandomRange StartRotation = new(0f, 0f);

    [DataMember]
    public RandomRange RotationSpeed = new(0f, 0f);

    [DataMember]
    public bool CanFlipX;

    [DataMember]
    public bool CanFlipY;

    private bool _fadeColor;

    [NonSerialized]
    private Color _startColor = Color.White;

    [NonSerialized]
    private Color _endColor = Color.White;

    [DataMember]
    public int MaxParticles;

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
    public bool IsEmpty => ParticleIds.Empty();

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
            sprite.Blend = BlendAdditive ? BlendState.Additive : Mokus2DGame.Config.DefaultSpriteBatchProperties.Blend;
        }
        data.Initialize();
    }

    private float? GetValueInRange(RandomRange? range)
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
