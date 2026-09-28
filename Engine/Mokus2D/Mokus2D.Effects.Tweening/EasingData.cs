using System;

using Mokus2D.Data;

namespace Mokus2D.Effects.Tweening;

public class EasingData : IEasingData, ICleanable
{
    private static readonly Pool<EasingData> Pool = new(() => new EasingData());

    private Func<float, float, float, float> _functionC;

    private Func<float, float, float> _functionB;

    private Func<float, float> _functionA;

    private float _dataA;

    private float _dataB;

    public static EasingData NewOrNull(Func<float, float, float, float> function, float dataA, float dataB)
    {
        return function != null ? Pool.New().Initialize(function, dataA, dataB) : null;
    }

    public static EasingData NewOrNull(Func<float, float, float> function, float dataA)
    {
        return function != null ? Pool.New().Initialize(function, dataA) : null;
    }

    public static EasingData NewOrNull(Func<float, float> function)
    {
        return function != null ? Pool.New().Initialize(function) : null;
    }

    public static void Free(EasingData data)
    {
        Pool.Free(data);
    }

    private EasingData()
    {
    }

    public EasingData Initialize(Func<float, float, float, float> function, float dataA, float dataB)
    {
        _functionC = function ?? throw new NullReferenceException("function can not be null");
        _dataA = dataA;
        _dataB = dataB;
        return this;
    }

    public EasingData Initialize(Func<float, float, float> function, float dataA)
    {
        _functionB = function ?? throw new NullReferenceException("function can not be null");
        _dataA = dataA;
        return this;
    }

    public EasingData Initialize(Func<float, float> function)
    {
        _functionA = function ?? throw new NullReferenceException("function can not be null");
        return this;
    }

    public float Ease(float ratio)
    {
        if (_functionA != null)
        {
            return _functionA(ratio);
        }
        if (_functionB != null)
        {
            return _functionB(ratio, _dataA);
        }
        return _functionC != null ? _functionC(ratio, _dataA, _dataB) : throw new Exception("No ease function selected");
    }

    public void Clean()
    {
        _functionA = null;
        _functionB = null;
        _functionC = null;
    }
}
