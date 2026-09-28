using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeSpotlight : Sprite, IFreeable, IId
{
    public const string ID = "common/McEyeSpotlight";

    public string Id => "common/McEyeSpotlight";

    public static McEyeSpotlight New()
    {
        McEyeSpotlight mcEyeSpotlight = StaticPool<McEyeSpotlight>.New();
        mcEyeSpotlight.RefreshProperties();
        return mcEyeSpotlight;
    }

    public McEyeSpotlight()
        : base("common/McEyeSpotlight")
    {
    }

    public void Free()
    {
        StaticPool<McEyeSpotlight>.Free(this);
    }
}
