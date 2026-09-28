using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McMusicLoadingIcon : Sprite, IFreeable, IId
{
    public const string ID = "menu/McMusicLoadingIcon";

    public string Id => "menu/McMusicLoadingIcon";

    public static McMusicLoadingIcon New()
    {
        McMusicLoadingIcon mcMusicLoadingIcon = StaticPool<McMusicLoadingIcon>.New();
        mcMusicLoadingIcon.RefreshProperties();
        return mcMusicLoadingIcon;
    }

    public McMusicLoadingIcon()
        : base("menu/McMusicLoadingIcon")
    {
    }

    public void Free()
    {
        StaticPool<McMusicLoadingIcon>.Free(this);
    }
}
