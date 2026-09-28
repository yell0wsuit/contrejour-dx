using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.liveTile;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class liveTileHighLight : Sprite, IFreeable, IId
{
    public const string ID = "liveTile/liveTileHighLight";

    public string Id => "liveTile/liveTileHighLight";

    public static liveTileHighLight New()
    {
        liveTileHighLight liveTileHighLight2 = StaticPool<liveTileHighLight>.New();
        liveTileHighLight2.RefreshProperties();
        return liveTileHighLight2;
    }

    public liveTileHighLight()
        : base("liveTile/liveTileHighLight")
    {
    }

    public void Free()
    {
        StaticPool<liveTileHighLight>.Free(this);
    }
}
