using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLystok2Content : Sprite, IFreeable, IId
{
    public const string ID = "level1/McLystok2Content";

    public string Id => "level1/McLystok2Content";

    public static McLystok2Content New()
    {
        McLystok2Content mcLystok2Content = StaticPool<McLystok2Content>.New();
        mcLystok2Content.RefreshProperties();
        return mcLystok2Content;
    }

    public McLystok2Content()
        : base("level1/McLystok2Content")
    {
    }

    public void Free()
    {
        StaticPool<McLystok2Content>.Free(this);
    }
}
