using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackgroundContent2_6 : Sprite, IFreeable, IId
{
    public const string ID = "chapter2/McBackgroundContent2_6";

    public string Id => "chapter2/McBackgroundContent2_6";

    public static McBackgroundContent2_6 New()
    {
        McBackgroundContent2_6 mcBackgroundContent2_ = StaticPool<McBackgroundContent2_6>.New();
        mcBackgroundContent2_.RefreshProperties();
        return mcBackgroundContent2_;
    }

    public McBackgroundContent2_6()
        : base("chapter2/McBackgroundContent2_6")
    {
    }

    public void Free()
    {
        StaticPool<McBackgroundContent2_6>.Free(this);
    }
}
