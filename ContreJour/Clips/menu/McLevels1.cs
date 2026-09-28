using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLevels1 : Sprite, IFreeable, IId
{
    public const string ID = "menu/McLevels1";

    public string Id => "menu/McLevels1";

    public static McLevels1 New()
    {
        McLevels1 mcLevels = StaticPool<McLevels1>.New();
        mcLevels.RefreshProperties();
        return mcLevels;
    }

    public McLevels1()
        : base("menu/McLevels1")
    {
    }

    public void Free()
    {
        StaticPool<McLevels1>.Free(this);
    }
}
