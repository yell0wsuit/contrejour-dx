using System.Collections.Generic;

namespace FarseerPhysics.Common.PhysicsLogic;

internal class RayDataComparer : IComparer<float>
{
	int IComparer<float>.Compare(float a, float b)
	{
		float num = a - b;
		if (num > 0f)
		{
			return 1;
		}
		if (num < 0f)
		{
			return -1;
		}
		return 0;
	}
}
