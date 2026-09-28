using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRotatorGrass2 : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McRotatorGrass2";

    public string Id => "chapter5/McRotatorGrass2";

    public static McRotatorGrass2 New()
    {
        McRotatorGrass2 mcRotatorGrass = StaticPool<McRotatorGrass2>.New();
        mcRotatorGrass.RefreshProperties();
        return mcRotatorGrass;
    }

    public McRotatorGrass2()
        : base("chapter5/McRotatorGrass2")
    {
    }

    public void Free()
    {
        StaticPool<McRotatorGrass2>.Free(this);
    }
}
