using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeBallWhite : Sprite, IFreeable, IId
{
    public const string ID = "chapter4/McEyeBallWhite";

    public string Id => "chapter4/McEyeBallWhite";

    public static McEyeBallWhite New()
    {
        McEyeBallWhite mcEyeBallWhite = StaticPool.New<McEyeBallWhite>();
        mcEyeBallWhite.RefreshProperties();
        return mcEyeBallWhite;
    }

    public McEyeBallWhite()
        : base("chapter4/McEyeBallWhite")
    {
    }

    public void Free()
    {
        StaticPool.Free<McEyeBallWhite>(this);
    }
}
