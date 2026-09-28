using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Drawing.Vertex;

namespace Mokus2D.Visual.Shaders.Parallax;

public struct ParallaxVertex : ITintVertex, IVertex, IVertexType
{
	public Vector3 Position;

	public Color Color;

	public Vector2 TextureCoordinate;

	public float ColorRatio;

	public float Parallax;

	public static readonly VertexDeclaration VertexDeclaration;

	VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

	Vector3 IVertex.Position
	{
		get
		{
			return Position;
		}
		set
		{
			Position = value;
		}
	}

	Color IVertex.Color
	{
		get
		{
			return Color;
		}
		set
		{
			Color = value;
		}
	}

	Vector2 IVertex.TextureCoordinate
	{
		get
		{
			return TextureCoordinate;
		}
		set
		{
			TextureCoordinate = value;
		}
	}

	float ITintVertex.ColorRatio
	{
		get
		{
			return ColorRatio;
		}
		set
		{
			ColorRatio = value;
		}
	}

	static ParallaxVertex()
	{
		VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0), new VertexElement(16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0), new VertexElement(24, VertexElementFormat.Single, VertexElementUsage.BlendWeight, 0), new VertexElement(28, VertexElementFormat.Single, VertexElementUsage.BlendWeight, 1))
		{
			Name = "ParallaxVertex.VertexDeclaration"
		};
	}

	public ParallaxVertex(Vector3 position, Color color, Vector2 textureCoordinate, float colorRatio)
		: this(position, color, textureCoordinate, colorRatio, 1f)
	{
	}

	public ParallaxVertex(Vector3 position, Color color, Vector2 textureCoordinate, float colorRatio, float parallax)
	{
		this = default(ParallaxVertex);
		Position = position;
		Color = color;
		TextureCoordinate = textureCoordinate;
		ColorRatio = colorRatio;
		Parallax = parallax;
	}

	public static bool operator ==(ParallaxVertex left, ParallaxVertex right)
	{
		if (left.Position == right.Position && left.Color == right.Color && left.TextureCoordinate == right.TextureCoordinate && left.ColorRatio == right.ColorRatio)
		{
			return left.Parallax == right.Parallax;
		}
		return false;
	}

	public static bool operator !=(ParallaxVertex left, ParallaxVertex right)
	{
		return !(left == right);
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.CurrentCulture, "{{Position:{0} Color:{1} TextureCoordinate:{2}}}", new object[3] { Position, Color, TextureCoordinate });
	}

	public override bool Equals(object obj)
	{
		if (obj == null || (object)obj.GetType() != GetType())
		{
			return false;
		}
		return this == (ParallaxVertex)obj;
	}
}
