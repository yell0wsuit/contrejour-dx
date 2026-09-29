using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeSpotlight : Sprite, IFreeable, IId
{
    public const string ID = "fakeHero/McEyeSpotlight";

    public string Id => "fakeHero/McEyeSpotlight";

    public static McEyeSpotlight New()
    {
        McEyeSpotlight mcEyeSpotlight = StaticPool.New<McEyeSpotlight>();
        mcEyeSpotlight.RefreshProperties();
        return mcEyeSpotlight;
    }

    public McEyeSpotlight()
        : base("fakeHero/McEyeSpotlight")
    {
    }

    public void Free()
    {
        StaticPool.Free<McEyeSpotlight>(this);
    }
}
