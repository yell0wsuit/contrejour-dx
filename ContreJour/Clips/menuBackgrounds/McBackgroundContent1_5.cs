using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menuBackgrounds;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackgroundContent1_5 : Sprite, IFreeable, IId
{
    public const string ID = "menuBackgrounds/McBackgroundContent1_5";

    public string Id => "menuBackgrounds/McBackgroundContent1_5";

    public static McBackgroundContent1_5 New()
    {
        McBackgroundContent1_5 mcBackgroundContent1_ = StaticPool.New<McBackgroundContent1_5>();
        mcBackgroundContent1_.RefreshProperties();
        return mcBackgroundContent1_;
    }

    public McBackgroundContent1_5()
        : base("menuBackgrounds/McBackgroundContent1_5")
    {
    }

    public void Free()
    {
        StaticPool.Free<McBackgroundContent1_5>(this);
    }
}
