using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLevelItemSelected : Sprite, IFreeable, IId
{
    public const string ID = "menu/McLevelItemSelected";

    public string Id => "menu/McLevelItemSelected";

    public static McLevelItemSelected New()
    {
        McLevelItemSelected mcLevelItemSelected = StaticPool<McLevelItemSelected>.New();
        mcLevelItemSelected.RefreshProperties();
        return mcLevelItemSelected;
    }

    public McLevelItemSelected()
        : base("menu/McLevelItemSelected")
    {
    }

    public void Free()
    {
        StaticPool<McLevelItemSelected>.Free(this);
    }
}
