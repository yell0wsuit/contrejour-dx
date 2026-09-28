using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.lights;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLightView11ContentExport : Sprite, IFreeable, IId
{
    public const string ID = "lights/McLightView11ContentExport";

    public string Id => "lights/McLightView11ContentExport";

    public static McLightView11ContentExport New()
    {
        McLightView11ContentExport mcLightView11ContentExport = StaticPool.New<McLightView11ContentExport>();
        mcLightView11ContentExport.RefreshProperties();
        return mcLightView11ContentExport;
    }

    public McLightView11ContentExport()
        : base("lights/McLightView11ContentExport")
    {
    }

    public void Free()
    {
        StaticPool.Free<McLightView11ContentExport>(this);
    }
}
