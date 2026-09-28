using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLockIcon : Sprite, IFreeable, IId
{
    public const string ID = "menu/McLockIcon";

    public string Id => "menu/McLockIcon";

    public static McLockIcon New()
    {
        McLockIcon mcLockIcon = StaticPool<McLockIcon>.New();
        mcLockIcon.RefreshProperties();
        return mcLockIcon;
    }

    public McLockIcon()
        : base("menu/McLockIcon")
    {
    }

    public void Free()
    {
        StaticPool<McLockIcon>.Free(this);
    }
}
