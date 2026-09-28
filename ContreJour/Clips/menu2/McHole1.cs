using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McHole1 : Sprite, IFreeable, IId
{
    public const string ID = "menu2/McHole1";

    public string Id => "menu2/McHole1";

    public static McHole1 New()
    {
        McHole1 mcHole = StaticPool<McHole1>.New();
        mcHole.RefreshProperties();
        return mcHole;
    }

    public McHole1()
        : base("menu2/McHole1")
    {
    }

    public void Free()
    {
        StaticPool<McHole1>.Free(this);
    }
}
