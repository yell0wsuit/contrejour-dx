using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McTotalGrass : MovieClip, IFreeable, IId
{
    public const string ID = "common/McTotalGrass";

    public string Id => "common/McTotalGrass";

    public static McTotalGrass New()
    {
        McTotalGrass mcTotalGrass = StaticPool.New<McTotalGrass>();
        mcTotalGrass.RefreshProperties();
        return mcTotalGrass;
    }

    public McTotalGrass()
        : base("common/McTotalGrass")
    {
    }

    public void Free()
    {
        StaticPool.Free<McTotalGrass>(this);
    }
}
