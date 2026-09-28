using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McMusicPauseIcon : Sprite, IFreeable, IId
{
    public const string ID = "menu/McMusicPauseIcon";

    public string Id => "menu/McMusicPauseIcon";

    public static McMusicPauseIcon New()
    {
        McMusicPauseIcon mcMusicPauseIcon = StaticPool<McMusicPauseIcon>.New();
        mcMusicPauseIcon.RefreshProperties();
        return mcMusicPauseIcon;
    }

    public McMusicPauseIcon()
        : base("menu/McMusicPauseIcon")
    {
    }

    public void Free()
    {
        StaticPool<McMusicPauseIcon>.Free(this);
    }
}
