using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLevels4 : Sprite, IFreeable, IId
{
    public const string ID = "menu/McLevels4";

    public string Id => "menu/McLevels4";

    public static McLevels4 New()
    {
        McLevels4 mcLevels = StaticPool.New<McLevels4>();
        mcLevels.RefreshProperties();
        return mcLevels;
    }

    public McLevels4()
        : base("menu/McLevels4")
    {
    }

    public void Free()
    {
        StaticPool.Free<McLevels4>(this);
    }
}
