using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menuBackgrounds;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackground4Content : Sprite, IFreeable, IId
{
    public const string ID = "menuBackgrounds/McBackground4Content";

    public string Id => "menuBackgrounds/McBackground4Content";

    public static McBackground4Content New()
    {
        McBackground4Content mcBackground4Content = StaticPool<McBackground4Content>.New();
        mcBackground4Content.RefreshProperties();
        return mcBackground4Content;
    }

    public McBackground4Content()
        : base("menuBackgrounds/McBackground4Content")
    {
    }

    public void Free()
    {
        StaticPool<McBackground4Content>.Free(this);
    }
}
