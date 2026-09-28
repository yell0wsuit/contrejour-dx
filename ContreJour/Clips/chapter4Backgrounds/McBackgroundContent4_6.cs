using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4Backgrounds;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackgroundContent4_6 : Sprite, IFreeable, IId
{
    public const string ID = "chapter4Backgrounds/McBackgroundContent4_6";

    public string Id => "chapter4Backgrounds/McBackgroundContent4_6";

    public static McBackgroundContent4_6 New()
    {
        McBackgroundContent4_6 mcBackgroundContent4_ = StaticPool<McBackgroundContent4_6>.New();
        mcBackgroundContent4_.RefreshProperties();
        return mcBackgroundContent4_;
    }

    public McBackgroundContent4_6()
        : base("chapter4Backgrounds/McBackgroundContent4_6")
    {
    }

    public void Free()
    {
        StaticPool<McBackgroundContent4_6>.Free(this);
    }
}
