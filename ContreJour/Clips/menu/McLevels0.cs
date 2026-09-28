using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLevels0 : Sprite, IFreeable, IId
{
    public const string ID = "menu/McLevels0";

    public string Id => "menu/McLevels0";

    public static McLevels0 New()
    {
        McLevels0 mcLevels = StaticPool.New<McLevels0>();
        mcLevels.RefreshProperties();
        return mcLevels;
    }

    public McLevels0()
        : base("menu/McLevels0")
    {
    }

    public void Free()
    {
        StaticPool.Free<McLevels0>(this);
    }
}
