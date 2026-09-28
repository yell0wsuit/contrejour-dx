using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McHeroShadow : Sprite, IFreeable, IId
{
    public const string ID = "common/McHeroShadow";

    public string Id => "common/McHeroShadow";

    public static McHeroShadow New()
    {
        McHeroShadow mcHeroShadow = StaticPool<McHeroShadow>.New();
        mcHeroShadow.RefreshProperties();
        return mcHeroShadow;
    }

    public McHeroShadow()
        : base("common/McHeroShadow")
    {
    }

    public void Free()
    {
        StaticPool<McHeroShadow>.Free(this);
    }
}
