using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBaloonGrassAll : AnimationNode, IFreeable, IId
{
    public const string ID = "chapter5/McBaloonGrassAll";

    public McRotatorGrass0 instance5244430 { get; protected set; }

    public string Id => "chapter5/McBaloonGrassAll";

    public static McBaloonGrassAll New()
    {
        McBaloonGrassAll mcBaloonGrassAll = StaticPool.New<McBaloonGrassAll>();
        mcBaloonGrassAll.RefreshProperties();
        return mcBaloonGrassAll;
    }

    public McBaloonGrassAll()
        : base("chapter5/McBaloonGrassAll")
    {
        instance5244430 = new McRotatorGrass0();
        AddChild("instance5244430", instance5244430);
        Initialize();
    }

    public void Free()
    {
        StaticPool.Free<McBaloonGrassAll>(this);
    }
}
