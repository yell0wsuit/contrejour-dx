using System;
using System.Numerics;

namespace Mokus2D.Graphics
{
    // The one vertex format the renderer draws. The layout (Vector3, Color, Vector2: 24 bytes) matches
    // MonoGame's VertexPositionColorTexture. Position z is always 0.
    public struct Vertex(Vector3 position, Color color, Vector2 textureCoordinate) : IEquatable<Vertex>
    {
        public Vector3 Position = position;

        public Color Color = color;

        public Vector2 TextureCoordinate = textureCoordinate;

        public readonly bool Equals(Vertex other)
        {
            return Position == other.Position && Color == other.Color && TextureCoordinate == other.TextureCoordinate;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Vertex other && Equals(other);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Position, Color, TextureCoordinate);
        }

        public static bool operator ==(Vertex left, Vertex right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vertex left, Vertex right)
        {
            return !left.Equals(right);
        }
    }
}
