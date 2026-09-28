using System;
using System.Collections.Generic;

namespace Mokus2D.Util.MathUtils;

public class ReversableRandom(int seed) : Random(seed)
{
    private readonly List<double> _values = [];

    private int _step;

    public int Step => _step;

    public void ResetStep(int value)
    {
        _step = value;
    }

    public override double NextDouble()
    {
        return NextValue(base.NextDouble);
    }

    public override int Next(int maxValue)
    {
        return (int)(NextDouble() * maxValue);
    }

    public override int Next()
    {
        return (int)NextValue(NextIntValue);
    }

    private double NextIntValue()
    {
        return base.Next();
    }

    private double NextValue(Func<double> baseFunction)
    {
        double num;
        if (_step == _values.Count)
        {
            num = baseFunction();
            _values.Add(num);
        }
        else
        {
            num = _values[_step];
        }
        _step++;
        return num;
    }
}
