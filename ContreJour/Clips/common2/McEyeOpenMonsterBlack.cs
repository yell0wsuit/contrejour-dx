using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeOpenMonsterBlack : MovieClip, IFreeable, IId
{
    public const string ID = "common2/McEyeOpenMonsterBlack";

    public string Id => "common2/McEyeOpenMonsterBlack";

    public static McEyeOpenMonsterBlack New()
    {
        McEyeOpenMonsterBlack mcEyeOpenMonsterBlack = StaticPool.New<McEyeOpenMonsterBlack>();
        mcEyeOpenMonsterBlack.RefreshProperties();
        return mcEyeOpenMonsterBlack;
    }

    public McEyeOpenMonsterBlack()
        : base("common2/McEyeOpenMonsterBlack")
    {
    }

    public void Free()
    {
        StaticPool.Free<McEyeOpenMonsterBlack>(this);
    }
}
