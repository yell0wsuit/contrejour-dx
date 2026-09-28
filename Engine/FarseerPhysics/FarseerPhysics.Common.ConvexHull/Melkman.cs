using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common.ConvexHull;

public static class Melkman
{
	public static Vertices GetConvexHull(Vertices vertices)
	{
		if (vertices.Count <= 3)
		{
			return vertices;
		}
		Vector2[] array = new Vector2[vertices.Count + 1];
		int num = 3;
		int num2 = 0;
		int i = 3;
		float num3 = MathUtils.Area(vertices[0], vertices[1], vertices[2]);
		if (num3 == 0f)
		{
			ref Vector2 reference = ref array[0];
			reference = vertices[0];
			ref Vector2 reference2 = ref array[1];
			reference2 = vertices[2];
			ref Vector2 reference3 = ref array[2];
			reference3 = vertices[0];
			num = 2;
			for (i = 3; i < vertices.Count; i++)
			{
				Vector2 c = vertices[i];
				if (MathUtils.Area(ref array[0], ref array[1], ref c) != 0f)
				{
					break;
				}
				ref Vector2 reference4 = ref array[1];
				reference4 = vertices[i];
			}
		}
		else
		{
			ref Vector2 reference5 = ref array[0];
			ref Vector2 reference6 = ref array[3];
			reference5 = (reference6 = vertices[2]);
			if (num3 > 0f)
			{
				ref Vector2 reference7 = ref array[1];
				reference7 = vertices[0];
				ref Vector2 reference8 = ref array[2];
				reference8 = vertices[1];
			}
			else
			{
				ref Vector2 reference9 = ref array[1];
				reference9 = vertices[1];
				ref Vector2 reference10 = ref array[2];
				reference10 = vertices[0];
			}
		}
		int num4 = ((num == 0) ? (array.Length - 1) : (num - 1));
		int num5 = ((num2 != array.Length - 1) ? (num2 + 1) : 0);
		for (int j = i; j < vertices.Count; j++)
		{
			Vector2 c2 = vertices[j];
			if (!(MathUtils.Area(ref array[num4], ref array[num], ref c2) > 0f) || !(MathUtils.Area(ref array[num2], ref array[num5], ref c2) > 0f))
			{
				while (!(MathUtils.Area(ref array[num4], ref array[num], ref c2) > 0f))
				{
					num = num4;
					num4 = ((num == 0) ? (array.Length - 1) : (num - 1));
				}
				num = ((num != array.Length - 1) ? (num + 1) : 0);
				num4 = ((num == 0) ? (array.Length - 1) : (num - 1));
				array[num] = c2;
				while (!(MathUtils.Area(ref array[num2], ref array[num5], ref c2) > 0f))
				{
					num2 = num5;
					num5 = ((num2 != array.Length - 1) ? (num2 + 1) : 0);
				}
				num2 = ((num2 == 0) ? (array.Length - 1) : (num2 - 1));
				num5 = ((num2 != array.Length - 1) ? (num2 + 1) : 0);
				array[num2] = c2;
			}
		}
		if (num2 < num)
		{
			Vertices vertices2 = new Vertices(num);
			for (int k = num2; k < num; k++)
			{
				vertices2.Add(array[k]);
			}
			return vertices2;
		}
		Vertices vertices3 = new Vertices(num + array.Length);
		for (int l = 0; l < num; l++)
		{
			vertices3.Add(array[l]);
		}
		for (int m = num2; m < array.Length; m++)
		{
			vertices3.Add(array[m]);
		}
		return vertices3;
	}
}
