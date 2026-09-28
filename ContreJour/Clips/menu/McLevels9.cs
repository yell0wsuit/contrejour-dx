using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLevels9 : Sprite, IFreeable, IId
{
    public const string ID = "menu/McLevels9";

    public string Id => "menu/McLevels9";

    public static McLevels9 New()
    {
        McLevels9 mcLevels = StaticPool.New<McLevels9>();
        mcLevels.RefreshProperties();
        return mcLevels;
    }

    public McLevels9()
        : base("menu/McLevels9")
    {
    }

    public void Free()
    {
        StaticPool.Free<McLevels9>(this);
    }
}
