using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBlackSquare : Sprite, IFreeable, IId
{
    public const string ID = "common/McBlackSquare";

    public string Id => "common/McBlackSquare";

    public static McBlackSquare New()
    {
        McBlackSquare mcBlackSquare = StaticPool.New<McBlackSquare>();
        mcBlackSquare.RefreshProperties();
        return mcBlackSquare;
    }

    public McBlackSquare()
        : base("common/McBlackSquare")
    {
    }

    public void Free()
    {
        StaticPool.Free<McBlackSquare>(this);
    }
}
