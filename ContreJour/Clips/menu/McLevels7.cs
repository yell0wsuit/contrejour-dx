using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLevels7 : Sprite, IFreeable, IId
{
    public const string ID = "menu/McLevels7";

    public string Id => "menu/McLevels7";

    public static McLevels7 New()
    {
        McLevels7 mcLevels = StaticPool<McLevels7>.New();
        mcLevels.RefreshProperties();
        return mcLevels;
    }

    public McLevels7()
        : base("menu/McLevels7")
    {
    }

    public void Free()
    {
        StaticPool<McLevels7>.Free(this);
    }
}
