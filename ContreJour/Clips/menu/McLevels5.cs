using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLevels5 : Sprite, IFreeable, IId
{
    public const string ID = "menu/McLevels5";

    public string Id => "menu/McLevels5";

    public static McLevels5 New()
    {
        McLevels5 mcLevels = StaticPool<McLevels5>.New();
        mcLevels.RefreshProperties();
        return mcLevels;
    }

    public McLevels5()
        : base("menu/McLevels5")
    {
    }

    public void Free()
    {
        StaticPool<McLevels5>.Free(this);
    }
}
