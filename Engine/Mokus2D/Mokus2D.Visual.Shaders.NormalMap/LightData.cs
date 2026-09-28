using Microsoft.Xna.Framework;

namespace Mokus2D.Visual.Shaders.NormalMap;

public class LightData
{
    private Vector3 _position;

    private float _power;

    private Color _color;

    private float _distanceRate;

    internal int Index;

    private readonly NormalMapEffectBase _effect;

    public Vector3 Position
    {
        get => _position;
        set
        {
            if (_position != value)
            {
                _position = value;
                _effect.SetLightPosition(Index, value);
            }
        }
    }

    public float Power
    {
        get => _power;
        set
        {
            if (_power != value)
            {
                _power = value;
                _effect.SetLightPower(Index, value);
            }
        }
    }

    public Color Color
    {
        get => _color;
        set
        {
            if (_color != value)
            {
                _color = value;
                _effect.SetLightColor(Index, value);
            }
        }
    }

    public float DistanceRate
    {
        get => _distanceRate;
        set
        {
            if (_distanceRate != value)
            {
                _distanceRate = value;
                _effect.SetLightDistanceRate(Index, value);
            }
        }
    }

    internal LightData(NormalMapEffectBase effect, int index, Vector3 position, float power, Color color, float distanceRate)
    {
        _effect = effect;
        Index = index;
        Position = position;
        Power = power;
        Color = color;
        DistanceRate = distanceRate;
    }

    internal LightData(NormalMapEffectBase effect, int index, Vector3 position, float power, Color color)
        : this(effect, index, position, power, color, 3f)
    {
    }

    internal LightData(NormalMapEffectBase effect, int index, Vector3 position, float power)
        : this(effect, index, position, power, Color.White)
    {
    }
}
