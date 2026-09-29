using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McWhiteGrass : MovieClip, IFreeable, IId
{
    public const string ID = "chapter4/McWhiteGrass";

    public string Id => "chapter4/McWhiteGrass";

    public static McWhiteGrass New()
    {
        McWhiteGrass mcWhiteGrass = StaticPool.New<McWhiteGrass>();
        mcWhiteGrass.RefreshProperties();
        return mcWhiteGrass;
    }

    public McWhiteGrass()
        : base("chapter4/McWhiteGrass")
    {
    }

    public void Free()
    {
        StaticPool.Free<McWhiteGrass>(this);
    }
}
