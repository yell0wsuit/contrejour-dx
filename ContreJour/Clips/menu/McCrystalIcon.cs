using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McCrystalIcon : Sprite, IFreeable, IId
{
    public const string ID = "menu/McCrystalIcon";

    public string Id => "menu/McCrystalIcon";

    public static McCrystalIcon New()
    {
        McCrystalIcon mcCrystalIcon = StaticPool<McCrystalIcon>.New();
        mcCrystalIcon.RefreshProperties();
        return mcCrystalIcon;
    }

    public McCrystalIcon()
        : base("menu/McCrystalIcon")
    {
    }

    public void Free()
    {
        StaticPool<McCrystalIcon>.Free(this);
    }
}
