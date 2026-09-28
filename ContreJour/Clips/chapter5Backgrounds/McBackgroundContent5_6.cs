using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5Backgrounds;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackgroundContent5_6 : Sprite, IFreeable, IId
{
    public const string ID = "chapter5Backgrounds/McBackgroundContent5_6";

    public string Id => "chapter5Backgrounds/McBackgroundContent5_6";

    public static McBackgroundContent5_6 New()
    {
        McBackgroundContent5_6 mcBackgroundContent5_ = StaticPool<McBackgroundContent5_6>.New();
        mcBackgroundContent5_.RefreshProperties();
        return mcBackgroundContent5_;
    }

    public McBackgroundContent5_6()
        : base("chapter5Backgrounds/McBackgroundContent5_6")
    {
    }

    public void Free()
    {
        StaticPool<McBackgroundContent5_6>.Free(this);
    }
}
