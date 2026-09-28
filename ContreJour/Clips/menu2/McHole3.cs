using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McHole3 : Sprite, IFreeable, IId
{
    public const string ID = "menu2/McHole3";

    public string Id => "menu2/McHole3";

    public static McHole3 New()
    {
        McHole3 mcHole = StaticPool<McHole3>.New();
        mcHole.RefreshProperties();
        return mcHole;
    }

    public McHole3()
        : base("menu2/McHole3")
    {
    }

    public void Free()
    {
        StaticPool<McHole3>.Free(this);
    }
}
