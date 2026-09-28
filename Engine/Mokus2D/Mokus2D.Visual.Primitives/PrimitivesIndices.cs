using Mokus2D.Visual.Primitives.Collections;

namespace Mokus2D.Visual.Primitives;

public static class PrimitivesIndices
{
	private static readonly Pow2Array<short> LineIndices = new Pow2Array<short>();

	public static short[] GetLineIndices(int segmentsCount)
	{
		if (LineIndices.Length > segmentsCount * 6)
		{
			return LineIndices.Items;
		}
		LineIndices.EnsureCapacity(segmentsCount * 6);
		for (int i = LineIndices.Length / 6; i < segmentsCount; i++)
		{
			int num = i * 6;
			short num2 = (short)(i * 2);
			LineIndices.Items[num] = num2;
			LineIndices.Items[num + 1] = (short)(num2 + 1);
			LineIndices.Items[num + 2] = (short)(num2 + 3);
			LineIndices.Items[num + 3] = num2;
			LineIndices.Items[num + 4] = (short)(num2 + 3);
			LineIndices.Items[num + 5] = (short)(num2 + 2);
		}
		LineIndices.SetLength(segmentsCount * 6);
		return LineIndices.Items;
	}
}
