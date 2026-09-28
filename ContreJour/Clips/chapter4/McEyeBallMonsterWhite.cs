using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeBallMonsterWhite : Sprite, IFreeable, IId
{
    public const string ID = "chapter4/McEyeBallMonsterWhite";

    public string Id => "chapter4/McEyeBallMonsterWhite";

    public static McEyeBallMonsterWhite New()
    {
        McEyeBallMonsterWhite mcEyeBallMonsterWhite = StaticPool.New<McEyeBallMonsterWhite>();
        mcEyeBallMonsterWhite.RefreshProperties();
        return mcEyeBallMonsterWhite;
    }

    public McEyeBallMonsterWhite()
        : base("chapter4/McEyeBallMonsterWhite")
    {
    }

    public void Free()
    {
        StaticPool.Free<McEyeBallMonsterWhite>(this);
    }
}
