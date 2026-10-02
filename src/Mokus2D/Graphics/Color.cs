using System;

namespace Mokus2D.Graphics
{
    // A packed RGBA color with XNA's layout (R in the lowest byte) and MonoGame 3.8.5.1's formulas
    // (MIT License, Copyright (C) The MonoGame Team, https://github.com/MonoGame/MonoGame), so vertex
    // data and blending stay byte-identical. Integer constructors saturate out-of-range channels;
    // float constructors truncate `x * 255` and then saturate.
    public struct Color : IEquatable<Color>
    {
        private uint _packedValue;

        public Color(byte r, byte g, byte b, byte alpha)
        {
            _packedValue = (uint)((alpha << 24) | (b << 16) | (g << 8) | r);
        }

        public Color(int r, int g, int b)
        {
            _packedValue = 0xFF000000;
            if (((r | g | b) & 0xFFFFFF00) != 0)
            {
                uint clampedR = (uint)Math.Clamp(r, 0, 255);
                uint clampedG = (uint)Math.Clamp(g, 0, 255);
                uint clampedB = (uint)Math.Clamp(b, 0, 255);
                _packedValue |= (clampedB << 16) | (clampedG << 8) | clampedR;
            }
            else
            {
                _packedValue |= (uint)((b << 16) | (g << 8) | r);
            }
        }

        public Color(int r, int g, int b, int alpha)
        {
            if (((r | g | b | alpha) & 0xFFFFFF00) != 0)
            {
                uint clampedR = (uint)Math.Clamp(r, 0, 255);
                uint clampedG = (uint)Math.Clamp(g, 0, 255);
                uint clampedB = (uint)Math.Clamp(b, 0, 255);
                uint clampedA = (uint)Math.Clamp(alpha, 0, 255);
                _packedValue = (clampedA << 24) | (clampedB << 16) | (clampedG << 8) | clampedR;
            }
            else
            {
                _packedValue = (uint)((alpha << 24) | (b << 16) | (g << 8) | r);
            }
        }

        public Color(float r, float g, float b)
            : this((int)(r * 255), (int)(g * 255), (int)(b * 255))
        {
        }

        public Color(float r, float g, float b, float alpha)
            : this((int)(r * 255), (int)(g * 255), (int)(b * 255), (int)(alpha * 255))
        {
        }

        private Color(uint packedValue)
        {
            _packedValue = packedValue;
        }

        public static Color White { get; } = new(0xFFFFFFFF);

        public static Color Black { get; } = new(0xFF000000);

        public static Color Red { get; } = new(0xFF0000FF);

        public static Color Green { get; } = new(0xFF008000);

        public byte R
        {
            readonly get => (byte)_packedValue;
            set => _packedValue = (_packedValue & 0xFFFFFF00) | value;
        }

        public byte G
        {
            readonly get => (byte)(_packedValue >> 8);
            set => _packedValue = (_packedValue & 0xFFFF00FF) | ((uint)value << 8);
        }

        public byte B
        {
            readonly get => (byte)(_packedValue >> 16);
            set => _packedValue = (_packedValue & 0xFF00FFFF) | ((uint)value << 16);
        }

        public byte A
        {
            readonly get => (byte)(_packedValue >> 24);
            set => _packedValue = (_packedValue & 0x00FFFFFF) | ((uint)value << 24);
        }

        public static Color Lerp(Color value1, Color value2, float amount)
        {
            amount = Math.Clamp(amount, 0f, 1f);
            return new Color((int)Lerp(value1.R, value2.R, amount), (int)Lerp(value1.G, value2.G, amount), (int)Lerp(value1.B, value2.B, amount), (int)Lerp(value1.A, value2.A, amount));
        }

        public static Color operator *(Color value, float scale)
        {
            return new Color((int)(value.R * scale), (int)(value.G * scale), (int)(value.B * scale), (int)(value.A * scale));
        }

        public static bool operator ==(Color a, Color b)
        {
            return a._packedValue == b._packedValue;
        }

        public static bool operator !=(Color a, Color b)
        {
            return a._packedValue != b._packedValue;
        }

        public readonly bool Equals(Color other)
        {
            return _packedValue == other._packedValue;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Color other && Equals(other);
        }

        public override readonly int GetHashCode()
        {
            return _packedValue.GetHashCode();
        }

        // MathHelper.Lerp, which Color.Lerp uses per channel.
        private static float Lerp(float value1, float value2, float amount)
        {
            return value1 + ((value2 - value1) * amount);
        }
    }
}
