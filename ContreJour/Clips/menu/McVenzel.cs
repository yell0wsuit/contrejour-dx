using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McVenzel : Sprite, IFreeable, IId
{
    public const string ID = "menu/McVenzel";

    public string Id => "menu/McVenzel";

    public static McVenzel New()
    {
        McVenzel mcVenzel = StaticPool<McVenzel>.New();
        mcVenzel.RefreshProperties();
        return mcVenzel;
    }

    public McVenzel()
        : base("menu/McVenzel")
    {
    }

    public void Free()
    {
        StaticPool<McVenzel>.Free(this);
    }
}
