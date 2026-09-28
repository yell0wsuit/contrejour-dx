using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRoseHeadLight : Sprite, IFreeable, IId
{
    public const string ID = "level1/McRoseHeadLight";

    public string Id => "level1/McRoseHeadLight";

    public static McRoseHeadLight New()
    {
        McRoseHeadLight mcRoseHeadLight = StaticPool.New<McRoseHeadLight>();
        mcRoseHeadLight.RefreshProperties();
        return mcRoseHeadLight;
    }

    public McRoseHeadLight()
        : base("level1/McRoseHeadLight")
    {
    }

    public void Free()
    {
        StaticPool.Free<McRoseHeadLight>(this);
    }
}
