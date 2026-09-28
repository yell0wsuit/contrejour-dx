using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McMusicPlayIcon : Sprite, IFreeable, IId
{
    public const string ID = "menu/McMusicPlayIcon";

    public string Id => "menu/McMusicPlayIcon";

    public static McMusicPlayIcon New()
    {
        McMusicPlayIcon mcMusicPlayIcon = StaticPool<McMusicPlayIcon>.New();
        mcMusicPlayIcon.RefreshProperties();
        return mcMusicPlayIcon;
    }

    public McMusicPlayIcon()
        : base("menu/McMusicPlayIcon")
    {
    }

    public void Free()
    {
        StaticPool<McMusicPlayIcon>.Free(this);
    }
}
