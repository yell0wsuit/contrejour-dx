using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McHole2 : Sprite, IFreeable, IId
{
    public const string ID = "menu2/McHole2";

    public string Id => "menu2/McHole2";

    public static McHole2 New()
    {
        McHole2 mcHole = StaticPool<McHole2>.New();
        mcHole.RefreshProperties();
        return mcHole;
    }

    public McHole2()
        : base("menu2/McHole2")
    {
    }

    public void Free()
    {
        StaticPool<McHole2>.Free(this);
    }
}
