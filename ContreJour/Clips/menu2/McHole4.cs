using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McHole4 : Sprite, IFreeable, IId
{
    public const string ID = "menu2/McHole4";

    public string Id => "menu2/McHole4";

    public static McHole4 New()
    {
        McHole4 mcHole = StaticPool.New<McHole4>();
        mcHole.RefreshProperties();
        return mcHole;
    }

    public McHole4()
        : base("menu2/McHole4")
    {
    }

    public void Free()
    {
        StaticPool.Free<McHole4>(this);
    }
}
