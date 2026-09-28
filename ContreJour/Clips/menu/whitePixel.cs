using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class whitePixel : Sprite, IFreeable, IId
{
    public const string ID = "menu/whitePixel";

    public string Id => "menu/whitePixel";

    public static whitePixel New()
    {
        whitePixel whitePixel2 = StaticPool<whitePixel>.New();
        whitePixel2.RefreshProperties();
        return whitePixel2;
    }

    public whitePixel()
        : base("menu/whitePixel")
    {
    }

    public void Free()
    {
        StaticPool<whitePixel>.Free(this);
    }
}
