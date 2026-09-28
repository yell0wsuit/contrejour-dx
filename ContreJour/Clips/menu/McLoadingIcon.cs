using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLoadingIcon : Sprite, IFreeable, IId
{
    public const string ID = "menu/McLoadingIcon";

    public string Id => "menu/McLoadingIcon";

    public static McLoadingIcon New()
    {
        McLoadingIcon mcLoadingIcon = StaticPool<McLoadingIcon>.New();
        mcLoadingIcon.RefreshProperties();
        return mcLoadingIcon;
    }

    public McLoadingIcon()
        : base("menu/McLoadingIcon")
    {
    }

    public void Free()
    {
        StaticPool<McLoadingIcon>.Free(this);
    }
}
