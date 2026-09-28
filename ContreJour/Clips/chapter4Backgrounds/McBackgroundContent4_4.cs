using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4Backgrounds;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackgroundContent4_4 : Sprite, IFreeable, IId
{
    public const string ID = "chapter4Backgrounds/McBackgroundContent4_4";

    public string Id => "chapter4Backgrounds/McBackgroundContent4_4";

    public static McBackgroundContent4_4 New()
    {
        McBackgroundContent4_4 mcBackgroundContent4_ = StaticPool<McBackgroundContent4_4>.New();
        mcBackgroundContent4_.RefreshProperties();
        return mcBackgroundContent4_;
    }

    public McBackgroundContent4_4()
        : base("chapter4Backgrounds/McBackgroundContent4_4")
    {
    }

    public void Free()
    {
        StaticPool<McBackgroundContent4_4>.Free(this);
    }
}
