using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McMusicIcon : Sprite, IFreeable, IId
{
    public const string ID = "menu/McMusicIcon";

    public string Id => "menu/McMusicIcon";

    public static McMusicIcon New()
    {
        McMusicIcon mcMusicIcon = StaticPool<McMusicIcon>.New();
        mcMusicIcon.RefreshProperties();
        return mcMusicIcon;
    }

    public McMusicIcon()
        : base("menu/McMusicIcon")
    {
    }

    public void Free()
    {
        StaticPool<McMusicIcon>.Free(this);
    }
}
