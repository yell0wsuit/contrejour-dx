using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.Drawing.Effects;

public static class EffectUtil
{
	private const string Extension = "dx.mgfxo";

	public static Effect LoadEffect(Type type, string path)
	{
		path = path.Replace(".", "/");
		path = Path.ChangeExtension(path, "dx.mgfxo");
		Stream stream = Mokus2DGame.FileLoader.OpenFile(path);
		byte[] array = new byte[stream.Length];
		stream.Read(array, 0, array.Length);
		return new Effect(Mokus2DGame.Device, array);
	}
}
