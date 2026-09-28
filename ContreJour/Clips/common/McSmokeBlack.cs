using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSmokeBlack : Sprite, IFreeable, IId
{
    public const string ID = "common/McSmokeBlack";

    public string Id => "common/McSmokeBlack";

    public static McSmokeBlack New()
    {
        McSmokeBlack mcSmokeBlack = StaticPool<McSmokeBlack>.New();
        mcSmokeBlack.RefreshProperties();
        return mcSmokeBlack;
    }

    public McSmokeBlack()
        : base("common/McSmokeBlack")
    {
    }

    public void Free()
    {
        StaticPool<McSmokeBlack>.Free(this);
    }
}
