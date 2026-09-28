using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlayIcon : Sprite, IFreeable, IId
{
    public const string ID = "menu/McPlayIcon";

    public string Id => "menu/McPlayIcon";

    public static McPlayIcon New()
    {
        McPlayIcon mcPlayIcon = StaticPool<McPlayIcon>.New();
        mcPlayIcon.RefreshProperties();
        return mcPlayIcon;
    }

    public McPlayIcon()
        : base("menu/McPlayIcon")
    {
    }

    public void Free()
    {
        StaticPool<McPlayIcon>.Free(this);
    }
}
