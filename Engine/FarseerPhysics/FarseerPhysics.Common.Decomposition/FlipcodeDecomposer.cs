using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common.Decomposition;

internal static class FlipcodeDecomposer
{
	private static Vector2 _tmpA;

	private static Vector2 _tmpB;

	private static Vector2 _tmpC;

	public static List<Vertices> ConvexPartition(Vertices vertices)
	{
		int[] array = new int[vertices.Count];
		for (int i = 0; i < vertices.Count; i++)
		{
			array[i] = i;
		}
		int num = vertices.Count;
		int num2 = 2 * num;
		List<Vertices> list = new List<Vertices>();
		int num3 = num - 1;
		while (num > 2)
		{
			if (0 >= num2--)
			{
				return new List<Vertices>();
			}
			int num4 = num3;
			if (num <= num4)
			{
				num4 = 0;
			}
			num3 = num4 + 1;
			if (num <= num3)
			{
				num3 = 0;
			}
			int num5 = num3 + 1;
			if (num <= num5)
			{
				num5 = 0;
			}
			_tmpA = vertices[array[num4]];
			_tmpB = vertices[array[num3]];
			_tmpC = vertices[array[num5]];
			if (Snip(vertices, num4, num3, num5, num, array))
			{
				Vertices vertices2 = new Vertices(3);
				vertices2.Add(_tmpA);
				vertices2.Add(_tmpB);
				vertices2.Add(_tmpC);
				list.Add(vertices2);
				int num6 = num3;
				for (int j = num3 + 1; j < num; j++)
				{
					array[num6] = array[j];
					num6++;
				}
				num--;
				num2 = 2 * num;
			}
		}
		return list;
	}

	private static bool InsideTriangle(ref Vector2 a, ref Vector2 b, ref Vector2 c, ref Vector2 p)
	{
		float num = (c.X - b.X) * (p.Y - b.Y) - (c.Y - b.Y) * (p.X - b.X);
		float num2 = (b.X - a.X) * (p.Y - a.Y) - (b.Y - a.Y) * (p.X - a.X);
		float num3 = (a.X - c.X) * (p.Y - c.Y) - (a.Y - c.Y) * (p.X - c.X);
		if (num >= 0f && num3 >= 0f)
		{
			return num2 >= 0f;
		}
		return false;
	}

	private static bool Snip(Vertices contour, int u, int v, int w, int n, int[] V)
	{
		if (1.1920929E-07f > MathUtils.Area(ref _tmpA, ref _tmpB, ref _tmpC))
		{
			return false;
		}
		for (int i = 0; i < n; i++)
		{
			if (i != u && i != v && i != w)
			{
				Vector2 p = contour[V[i]];
				if (InsideTriangle(ref _tmpA, ref _tmpB, ref _tmpC, ref p))
				{
					return false;
				}
			}
		}
		return true;
	}
}
