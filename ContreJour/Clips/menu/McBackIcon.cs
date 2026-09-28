using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackIcon : Sprite, IFreeable, IId
{
    public const string ID = "menu/McBackIcon";

    public string Id => "menu/McBackIcon";

    public static McBackIcon New()
    {
        McBackIcon mcBackIcon = StaticPool<McBackIcon>.New();
        mcBackIcon.RefreshProperties();
        return mcBackIcon;
    }

    public McBackIcon()
        : base("menu/McBackIcon")
    {
    }

    public void Free()
    {
        StaticPool<McBackIcon>.Free(this);
    }
}
