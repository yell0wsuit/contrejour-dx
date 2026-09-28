using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5Backgrounds;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackgroundContent5_4 : AnimationNode, IFreeable, IId
{
    public const string ID = "chapter5Backgrounds/McBackgroundContent5_4";

    public McBackgroundContent5_6 instance5242444 { get; protected set; }

    public string Id => "chapter5Backgrounds/McBackgroundContent5_4";

    public static McBackgroundContent5_4 New()
    {
        McBackgroundContent5_4 mcBackgroundContent5_ = StaticPool<McBackgroundContent5_4>.New();
        mcBackgroundContent5_.RefreshProperties();
        return mcBackgroundContent5_;
    }

    public McBackgroundContent5_4()
        : base("chapter5Backgrounds/McBackgroundContent5_4")
    {
        instance5242444 = new McBackgroundContent5_6();
        AddChild("instance5242444", instance5242444);
        Initialize();
    }

    public void Free()
    {
        StaticPool<McBackgroundContent5_4>.Free(this);
    }
}
