using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeCloseMonsterBlack : MovieClip, IFreeable, IId
{
    public const string ID = "common2/McEyeCloseMonsterBlack";

    public string Id => "common2/McEyeCloseMonsterBlack";

    public static McEyeCloseMonsterBlack New()
    {
        McEyeCloseMonsterBlack mcEyeCloseMonsterBlack = StaticPool.New<McEyeCloseMonsterBlack>();
        mcEyeCloseMonsterBlack.RefreshProperties();
        return mcEyeCloseMonsterBlack;
    }

    public McEyeCloseMonsterBlack()
        : base("common2/McEyeCloseMonsterBlack")
    {
    }

    public void Free()
    {
        StaticPool.Free<McEyeCloseMonsterBlack>(this);
    }
}
