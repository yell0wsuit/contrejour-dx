using Mokus2D.Data;
using Mokus2D.Fonts;

namespace Mokus2D.Visual.Text.LabelData;

public class Glyph : Sprite
{
	private static readonly Pool<Glyph> Pool = new Pool<Glyph>(() => new Glyph());

	private CharData _data;

	public static int ObjectsInPool => Pool.ObjectsInPool;

	public char Symbol { get; private set; }

	public float Width => Data.Width * ScaleFactor;

	public CharData Data
	{
		get
		{
			return _data;
		}
		set
		{
			_data = value;
			base.Texture = _data.Texture;
			ResetData(_data);
		}
	}

	public static Glyph New(CharData data, float scaleFactor, char symbol)
	{
		return Pool.New().Initialize(data, scaleFactor, symbol);
	}

	public static void Free(Glyph glyph)
	{
		Pool.Free(glyph);
	}

	private Glyph()
	{
	}

	public void ReloadData(FontData data)
	{
		Data = data[Symbol];
		base.Texture = data.Texture;
	}

	public override void ReloadData()
	{
	}

	public Glyph Initialize(CharData charData, float scaleFactor, char symbol)
	{
		Data = charData;
		ScaleFactor = scaleFactor;
		Symbol = symbol;
		return this;
	}
}
