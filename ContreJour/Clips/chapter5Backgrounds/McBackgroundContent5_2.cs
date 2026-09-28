using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5Backgrounds;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackgroundContent5_2 : AnimationNode, IFreeable, IId
{
    public const string ID = "chapter5Backgrounds/McBackgroundContent5_2";

    public McBackgroundContent5_7 instance5242434 { get; protected set; }

    public string Id => "chapter5Backgrounds/McBackgroundContent5_2";

    public static McBackgroundContent5_2 New()
    {
        McBackgroundContent5_2 mcBackgroundContent5_ = StaticPool.New<McBackgroundContent5_2>();
        mcBackgroundContent5_.RefreshProperties();
        return mcBackgroundContent5_;
    }

    public McBackgroundContent5_2()
        : base("chapter5Backgrounds/McBackgroundContent5_2")
    {
        instance5242434 = new McBackgroundContent5_7();
        AddChild("instance5242434", instance5242434);
        Initialize();
    }

    public void Free()
    {
        StaticPool.Free<McBackgroundContent5_2>(this);
    }
}
