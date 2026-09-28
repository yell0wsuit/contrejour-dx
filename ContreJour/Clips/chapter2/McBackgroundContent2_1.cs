using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackgroundContent2_1 : Sprite, IFreeable, IId
{
    public const string ID = "chapter2/McBackgroundContent2_1";

    public string Id => "chapter2/McBackgroundContent2_1";

    public static McBackgroundContent2_1 New()
    {
        McBackgroundContent2_1 mcBackgroundContent2_ = StaticPool.New<McBackgroundContent2_1>();
        mcBackgroundContent2_.RefreshProperties();
        return mcBackgroundContent2_;
    }

    public McBackgroundContent2_1()
        : base("chapter2/McBackgroundContent2_1")
    {
    }

    public void Free()
    {
        StaticPool.Free<McBackgroundContent2_1>(this);
    }
}
