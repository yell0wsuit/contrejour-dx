using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLevels2 : Sprite, IFreeable, IId
{
    public const string ID = "menu/McLevels2";

    public string Id => "menu/McLevels2";

    public static McLevels2 New()
    {
        McLevels2 mcLevels = StaticPool<McLevels2>.New();
        mcLevels.RefreshProperties();
        return mcLevels;
    }

    public McLevels2()
        : base("menu/McLevels2")
    {
    }

    public void Free()
    {
        StaticPool<McLevels2>.Free(this);
    }
}
