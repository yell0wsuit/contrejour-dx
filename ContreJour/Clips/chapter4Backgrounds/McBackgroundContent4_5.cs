using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4Backgrounds;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackgroundContent4_5 : AnimationNode, IFreeable, IId
{
    public const string ID = "chapter4Backgrounds/McBackgroundContent4_5";

    public McBackgroundContent4_7 instance5245629 { get; protected set; }

    public string Id => "chapter4Backgrounds/McBackgroundContent4_5";

    public static McBackgroundContent4_5 New()
    {
        McBackgroundContent4_5 mcBackgroundContent4_ = StaticPool<McBackgroundContent4_5>.New();
        mcBackgroundContent4_.RefreshProperties();
        return mcBackgroundContent4_;
    }

    public McBackgroundContent4_5()
        : base("chapter4Backgrounds/McBackgroundContent4_5")
    {
        instance5245629 = new McBackgroundContent4_7();
        AddChild("instance5245629", instance5245629);
        Initialize();
    }

    public void Free()
    {
        StaticPool<McBackgroundContent4_5>.Free(this);
    }
}
