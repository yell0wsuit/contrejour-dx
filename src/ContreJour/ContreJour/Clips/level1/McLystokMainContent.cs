using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLystokMainContent : Sprite, IFreeable, IId
{
    public const string ID = "level1/McLystokMainContent";

    public string Id => "level1/McLystokMainContent";

    public static McLystokMainContent New()
    {
        McLystokMainContent mcLystokMainContent = StaticPool.New<McLystokMainContent>();
        mcLystokMainContent.RefreshProperties();
        return mcLystokMainContent;
    }

    public McLystokMainContent()
        : base("level1/McLystokMainContent")
    {
    }

    public void Free()
    {
        StaticPool.Free<McLystokMainContent>(this);
    }
}
