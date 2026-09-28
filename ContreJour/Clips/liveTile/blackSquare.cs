using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.liveTile;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class blackSquare : Sprite, IFreeable, IId
{
    public const string ID = "liveTile/blackSquare";

    public string Id => "liveTile/blackSquare";

    public static blackSquare New()
    {
        blackSquare blackSquare2 = StaticPool<blackSquare>.New();
        blackSquare2.RefreshProperties();
        return blackSquare2;
    }

    public blackSquare()
        : base("liveTile/blackSquare")
    {
    }

    public void Free()
    {
        StaticPool<blackSquare>.Free(this);
    }
}
