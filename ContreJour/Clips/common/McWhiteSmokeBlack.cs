using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McWhiteSmokeBlack : Sprite, IFreeable, IId
{
    public const string ID = "common/McWhiteSmokeBlack";

    public string Id => "common/McWhiteSmokeBlack";

    public static McWhiteSmokeBlack New()
    {
        McWhiteSmokeBlack mcWhiteSmokeBlack = StaticPool.New<McWhiteSmokeBlack>();
        mcWhiteSmokeBlack.RefreshProperties();
        return mcWhiteSmokeBlack;
    }

    public McWhiteSmokeBlack()
        : base("common/McWhiteSmokeBlack")
    {
    }

    public void Free()
    {
        StaticPool.Free<McWhiteSmokeBlack>(this);
    }
}
