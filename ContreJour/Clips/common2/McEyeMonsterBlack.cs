using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeMonsterBlack : Sprite, IFreeable, IId
{
    public const string ID = "common2/McEyeMonsterBlack";

    public string Id => "common2/McEyeMonsterBlack";

    public static McEyeMonsterBlack New()
    {
        McEyeMonsterBlack mcEyeMonsterBlack = StaticPool<McEyeMonsterBlack>.New();
        mcEyeMonsterBlack.RefreshProperties();
        return mcEyeMonsterBlack;
    }

    public McEyeMonsterBlack()
        : base("common2/McEyeMonsterBlack")
    {
    }

    public void Free()
    {
        StaticPool<McEyeMonsterBlack>.Free(this);
    }
}
