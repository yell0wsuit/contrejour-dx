using System.Collections.Generic;

using Mokus2D.Data;

namespace Mokus2D.Visual.Text.LabelData;

public class LabelLine : ICleanable
{
    private static readonly Pool<LabelLine> Pool = new Pool<LabelLine>(() => new LabelLine());

    public readonly List<Glyph> Glyphs = new List<Glyph>();

    public static int ObjectsInPool => Pool.ObjectsInPool;

    public float Width { get; internal set; }

    public static LabelLine New()
    {
        return Pool.New();
    }

    public static void Free(LabelLine line)
    {
        Pool.Free(line);
    }

    private LabelLine()
    {
    }

    public void Clean()
    {
        Glyphs.Clear();
    }
}
