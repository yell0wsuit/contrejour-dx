using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRotatorGrass0 : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McRotatorGrass0";

    public string Id => "chapter5/McRotatorGrass0";

    public static McRotatorGrass0 New()
    {
        McRotatorGrass0 mcRotatorGrass = StaticPool.New<McRotatorGrass0>();
        mcRotatorGrass.RefreshProperties();
        return mcRotatorGrass;
    }

    public McRotatorGrass0()
        : base("chapter5/McRotatorGrass0")
    {
    }

    public void Free()
    {
        StaticPool.Free<McRotatorGrass0>(this);
    }
}
