using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRoseLight : Sprite, IFreeable, IId
{
    public const string ID = "menu2/McRoseLight";

    public string Id => "menu2/McRoseLight";

    public static McRoseLight New()
    {
        McRoseLight mcRoseLight = StaticPool<McRoseLight>.New();
        mcRoseLight.RefreshProperties();
        return mcRoseLight;
    }

    public McRoseLight()
        : base("menu2/McRoseLight")
    {
    }

    public void Free()
    {
        StaticPool<McRoseLight>.Free(this);
    }
}
