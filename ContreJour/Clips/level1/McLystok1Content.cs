using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLystok1Content : Sprite, IFreeable, IId
{
    public const string ID = "level1/McLystok1Content";

    public string Id => "level1/McLystok1Content";

    public static McLystok1Content New()
    {
        McLystok1Content mcLystok1Content = StaticPool.New<McLystok1Content>();
        mcLystok1Content.RefreshProperties();
        return mcLystok1Content;
    }

    public McLystok1Content()
        : base("level1/McLystok1Content")
    {
    }

    public void Free()
    {
        StaticPool.Free<McLystok1Content>(this);
    }
}
