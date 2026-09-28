using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLevels6 : Sprite, IFreeable, IId
{
    public const string ID = "menu/McLevels6";

    public string Id => "menu/McLevels6";

    public static McLevels6 New()
    {
        McLevels6 mcLevels = StaticPool<McLevels6>.New();
        mcLevels.RefreshProperties();
        return mcLevels;
    }

    public McLevels6()
        : base("menu/McLevels6")
    {
    }

    public void Free()
    {
        StaticPool<McLevels6>.Free(this);
    }
}
