using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class resizeIcon : Sprite, IFreeable, IId
{
    public const string ID = "menu2/resizeIcon";

    public string Id => "menu2/resizeIcon";

    public static resizeIcon New()
    {
        resizeIcon resizeIcon2 = StaticPool<resizeIcon>.New();
        resizeIcon2.RefreshProperties();
        return resizeIcon2;
    }

    public resizeIcon()
        : base("menu2/resizeIcon")
    {
    }

    public void Free()
    {
        StaticPool<resizeIcon>.Free(this);
    }
}
