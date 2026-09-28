using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McImprovedResult : Sprite, IFreeable, IId
{
    public const string ID = "menu/McImprovedResult";

    public string Id => "menu/McImprovedResult";

    public static McImprovedResult New()
    {
        McImprovedResult mcImprovedResult = StaticPool.New<McImprovedResult>();
        mcImprovedResult.RefreshProperties();
        return mcImprovedResult;
    }

    public McImprovedResult()
        : base("menu/McImprovedResult")
    {
    }

    public void Free()
    {
        StaticPool.Free<McImprovedResult>(this);
    }
}
