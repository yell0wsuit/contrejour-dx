using Microsoft.Xna.Framework;
using Mokus2D.Util.Extensions;

namespace Mokus2D.Visual.Util;

public static class ColorUtil
{
	public static byte ComponentToByte(float value)
	{
		return (byte)(value * 255f);
	}

	public static Color AddOpacity(Color color, float opacity, bool premultiplied)
	{
		if (premultiplied)
		{
			color *= opacity;
		}
		else
		{
			color.A = ComponentToByte(opacity);
		}
		return color;
	}

	public static Color CreateColorByte(float r, float g, float b, float a)
	{
		return CreateColor((int)r, (int)g, (int)b, (int)a);
	}

	public static Color Mult(Color color, float value)
	{
		return color.Mult(value);
	}

	public static Color CreateColor(int r, int g, int b, int a)
	{
		return new Color(r, g, b, a);
	}
}
