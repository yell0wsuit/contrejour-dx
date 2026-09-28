using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRotatorGrassAll : AnimationNode, IFreeable, IId
{
    public const string ID = "chapter5/McRotatorGrassAll";

    public McRotatorGrass0 instance5244712 { get; protected set; }

    public string Id => "chapter5/McRotatorGrassAll";

    public static McRotatorGrassAll New()
    {
        McRotatorGrassAll mcRotatorGrassAll = StaticPool<McRotatorGrassAll>.New();
        mcRotatorGrassAll.RefreshProperties();
        return mcRotatorGrassAll;
    }

    public McRotatorGrassAll()
        : base("chapter5/McRotatorGrassAll")
    {
        instance5244712 = new McRotatorGrass0();
        AddChild("instance5244712", instance5244712);
        Initialize();
    }

    public void Free()
    {
        StaticPool<McRotatorGrassAll>.Free(this);
    }
}
