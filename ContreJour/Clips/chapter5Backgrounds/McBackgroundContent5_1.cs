using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5Backgrounds;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackgroundContent5_1 : AnimationNode, IFreeable, IId
{
    public const string ID = "chapter5Backgrounds/McBackgroundContent5_1";

    public McBackgroundContent5_3 instance5242426 { get; protected set; }

    public string Id => "chapter5Backgrounds/McBackgroundContent5_1";

    public static McBackgroundContent5_1 New()
    {
        McBackgroundContent5_1 mcBackgroundContent5_ = StaticPool<McBackgroundContent5_1>.New();
        mcBackgroundContent5_.RefreshProperties();
        return mcBackgroundContent5_;
    }

    public McBackgroundContent5_1()
        : base("chapter5Backgrounds/McBackgroundContent5_1")
    {
        instance5242426 = new McBackgroundContent5_3();
        AddChild("instance5242426", instance5242426);
        Initialize();
    }

    public void Free()
    {
        StaticPool<McBackgroundContent5_1>.Free(this);
    }
}
