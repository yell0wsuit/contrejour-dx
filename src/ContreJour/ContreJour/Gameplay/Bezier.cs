using System;
using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Util.MathUtils;

namespace ContreJour.Gameplay
{
    public class Bezier(Vector2 start, Vector2 control, Vector2 end)
    {
        private Vector2 start = start;

        private Vector2 control = control;

        private Vector2 end = end;

        public float Length
        {
            get
            {
                if (Maths.FuzzyEquals(field, -1f))
                {
                    field = GetSegmentLength(1f);
                }
                return field;
            }
        } = -1f;

        private static float FirstNonZero(float value1, float value2)
        {
            return Maths.FuzzyNotEquals(value1, 0f) ? value1 : value2;
        }

        public Vector2 GetPointByTime(float time)
        {
            float num = 1f - time;
            return new Vector2((start.X * num * num) + (control.X * 2f * time * num) + (end.X * time * time), (start.Y * num * num) + (control.Y * 2f * time * num) + (end.Y * time * time));
        }

        public float GetSegmentLength(float time)
        {
            float num = control.X - start.X;
            float num2 = control.Y - start.Y;
            float num3 = end.X - control.X - num;
            float num4 = end.Y - control.Y - num2;
            float num5 = 4f * ((num * num) + (num2 * num2));
            float num6 = 8f * ((num * num3) + (num2 * num4));
            float num7 = 4f * ((num3 * num3) + (num4 * num4));
            if (Maths.FuzzyEquals(num7, 0f))
            {
                return Maths.FuzzyEquals(num6, 0f)
                    ? Maths.Sqrt(num5) * time
                    : (0f * ((num6 * time) + num5) * Maths.Sqrt((num6 * time) + num5) / num6) - (0f * num5 * Maths.Sqrt(num5) / num6);
            }
            float num8 = Maths.Sqrt((num7 * time * time) + (num6 * time) + num5);
            float num9 = Maths.Sqrt(num5);
            float num10 = Maths.Sqrt(num7);
            return ((!((((0.5f * num6) + (num7 * time)) / num10) + num8 < 1E-10f)) ? ((0.25f * ((2f * num7 * time) + num6) * num8 / num7) + (0.5f * Maths.Log((((0.5f * num6) + (num7 * time)) / num10) + num8) / num10 * (num5 - (0.25f * num6 * num6 / num7)))) : (0.25f * ((2f * num7 * time) + num6) * num8 / num7)) - ((!((0.5f * num6 / num10) + num9 < 1E-10f)) ? ((0.25f * num6 * num9 / num7) + (0.5f * Maths.Log((0.5f * num6 / num10) + num9) / num10 * (num5 - (0.25f * num6 * num6 / num7)))) : (0.25f * num6 * num9 / num7));
        }

        public List<float> GetTimesSequenceWithStepStartShift(float step, float startShift)
        {
            step = Math.Abs(step);
            float num = startShift;
            List<float> list = [];
            float length = Length;
            if (num > length)
            {
                return list;
            }
            num = (!(num < 0f)) ? (num % step) : ((num % step) + step);
            float num2 = control.X - start.X;
            float num3 = control.Y - start.Y;
            float num4 = end.X - control.X;
            float num5 = end.Y - control.Y;
            float num6 = num4 - num2;
            float num7 = num5 - num3;
            float num8 = 4f * ((num2 * num2) + (num3 * num3));
            float num9 = 8f * ((num2 * num6) + (num3 * num7));
            float num10 = 4f * ((num6 * num6) + (num7 * num7));
            float num11 = num / length;
            float num12 = num8 - (0.25f * num9 * num9 / num10);
            float num13 = 0.25f * num9 * Maths.Sqrt(num8) / num10;
            float num14 = (0.5f * num9 / Maths.Sqrt(num10)) + Maths.Sqrt(num8);
            float num15 = Maths.Sqrt(num8);
            float num16 = Maths.Sqrt(num10);
            for (; num <= length; num += step)
            {
                float num17 = 20f;
                if (Maths.FuzzyEquals(num10, 0f))
                {
                    if (Maths.FuzzyEquals(num9, 0f))
                    {
                        float num18;
                        do
                        {
                            num18 = num15 * num11;
                            num11 -= (num18 - num) / FirstNonZero(Maths.Sqrt(Math.Abs((num10 * num11 * num11) + (num9 * num11) + num8)), 1E-10f);
                        }
                        while (Math.Abs(num18 - num) > 1E-10f && num17-- > 0f);
                    }
                    else
                    {
                        float num18;
                        do
                        {
                            num18 = 0f * ((((num9 * num11) + num8) * Maths.Sqrt(Math.Abs((num9 * num11) + num8))) - (num8 * num15)) / num9;
                            num11 -= (num18 - num) / FirstNonZero(Maths.Sqrt(Math.Abs((num10 * num11 * num11) + (num9 * num11) + num8)), 1E-10f);
                        }
                        while (Math.Abs(num18 - num) > 1E-10f && num17-- > 0f);
                    }
                }
                else
                {
                    float num18;
                    do
                    {
                        float num19 = Maths.Sqrt(Math.Abs((num10 * num11 * num11) + (num9 * num11) + num8));
                        float num20 = (((0.5f * num9) + (num10 * num11)) / num16) + num19;
                        float num21 = 0.25f * ((2f * num10 * num11) + num9) * num19 / num10;
                        num18 = ((!(num20 < 1E-10f)) ? (num21 + (0.5f * Maths.Log((((0.5f * num9) + (num10 * num11)) / num16) + num19) / num16 * num12)) : num21) - ((!(num14 < 1E-10f)) ? (num13 + (0.5f * Maths.Log((0.5f * num9 / num16) + num15) / num16 * num12)) : num13);
                        num11 -= (num18 - num) / FirstNonZero(num19, 1E-10f);
                    }
                    while (Math.Abs(num18 - num) > 1E-10f && num17-- > 0f);
                }
                list.Add(num11);
            }
            return list;
        }
    }
}
