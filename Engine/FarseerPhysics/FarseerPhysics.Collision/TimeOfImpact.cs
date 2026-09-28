using System;

using FarseerPhysics.Common;

namespace FarseerPhysics.Collision;

public static class TimeOfImpact
{
    [ThreadStatic]
    public static int TOICalls;

    [ThreadStatic]
    public static int TOIIters;

    [ThreadStatic]
    public static int TOIMaxIters;

    [ThreadStatic]
    public static int TOIRootIters;

    [ThreadStatic]
    public static int TOIMaxRootIters;

    [ThreadStatic]
    private static DistanceInput _distanceInput;

    public static void CalculateTimeOfImpact(out TOIOutput output, TOIInput input)
    {
        TOICalls++;
        output = default(TOIOutput);
        output.State = TOIOutputState.Unknown;
        output.T = input.TMax;
        Sweep sweepA = input.SweepA;
        Sweep sweepB = input.SweepB;
        sweepA.Normalize();
        sweepB.Normalize();
        float tMax = input.TMax;
        float num = input.ProxyA.Radius + input.ProxyB.Radius;
        float num2 = Math.Max(0.005f, num - 0.015f);
        float num3 = 0f;
        int num4 = 0;
        _distanceInput = _distanceInput ?? new DistanceInput();
        _distanceInput.ProxyA = input.ProxyA;
        _distanceInput.ProxyB = input.ProxyB;
        _distanceInput.UseRadii = false;
        while (true)
        {
            sweepA.GetTransform(out var xfb, num3);
            sweepB.GetTransform(out var xfb2, num3);
            _distanceInput.TransformA = xfb;
            _distanceInput.TransformB = xfb2;
            Distance.ComputeDistance(out var output2, out var cache, _distanceInput);
            if (output2.Distance <= 0f)
            {
                output.State = TOIOutputState.Overlapped;
                output.T = 0f;
                break;
            }
            if (output2.Distance < num2 + 0.00125f)
            {
                output.State = TOIOutputState.Touching;
                output.T = num3;
                break;
            }
            SeparationFunction.Set(ref cache, input.ProxyA, ref sweepA, input.ProxyB, ref sweepB, num3);
            bool flag = false;
            float num5 = tMax;
            int num6 = 0;
            do
            {
                int indexA;
                int indexB;
                float num7 = SeparationFunction.FindMinSeparation(out indexA, out indexB, num5);
                if (num7 > num2 + 0.00125f)
                {
                    output.State = TOIOutputState.Seperated;
                    output.T = tMax;
                    flag = true;
                    break;
                }
                if (num7 > num2 - 0.00125f)
                {
                    num3 = num5;
                    break;
                }
                float num8 = SeparationFunction.Evaluate(indexA, indexB, num3);
                if (num8 < num2 - 0.00125f)
                {
                    output.State = TOIOutputState.Failed;
                    output.T = num3;
                    flag = true;
                    break;
                }
                if (num8 <= num2 + 0.00125f)
                {
                    output.State = TOIOutputState.Touching;
                    output.T = num3;
                    flag = true;
                    break;
                }
                int num9 = 0;
                float num10 = num3;
                float num11 = num5;
                do
                {
                    float num12 = (((num9 & 1) == 0) ? (0.5f * (num10 + num11)) : (num10 + (num2 - num8) * (num11 - num10) / (num7 - num8)));
                    num9++;
                    TOIRootIters++;
                    float num13 = SeparationFunction.Evaluate(indexA, indexB, num12);
                    if (Math.Abs(num13 - num2) < 0.00125f)
                    {
                        num5 = num12;
                        break;
                    }
                    if (num13 > num2)
                    {
                        num10 = num12;
                        num8 = num13;
                    }
                    else
                    {
                        num11 = num12;
                        num7 = num13;
                    }
                }
                while (num9 != 50);
                TOIMaxRootIters = Math.Max(TOIMaxRootIters, num9);
                num6++;
            }
            while (num6 != Settings.MaxPolygonVertices);
            num4++;
            TOIIters++;
            if (flag)
            {
                break;
            }
            if (num4 == 20)
            {
                output.State = TOIOutputState.Failed;
                output.T = num3;
                break;
            }
        }
        TOIMaxIters = Math.Max(TOIMaxIters, num4);
    }
}
