using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackgroundContent2_4 : Sprite, IFreeable, IId
{
    public const string ID = "chapter2/McBackgroundContent2_4";

    public string Id => "chapter2/McBackgroundContent2_4";

    public static McBackgroundContent2_4 New()
    {
        McBackgroundContent2_4 mcBackgroundContent2_ = StaticPool<McBackgroundContent2_4>.New();
        mcBackgroundContent2_.RefreshProperties();
        return mcBackgroundContent2_;
    }

    public McBackgroundContent2_4()
        : base("chapter2/McBackgroundContent2_4")
    {
    }

    public void Free()
    {
        StaticPool<McBackgroundContent2_4>.Free(this);
    }
}
