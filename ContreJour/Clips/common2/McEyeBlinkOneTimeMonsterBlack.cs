using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeBlinkOneTimeMonsterBlack : MovieClip, IFreeable, IId
{
    public const string ID = "common2/McEyeBlinkOneTimeMonsterBlack";

    public string Id => "common2/McEyeBlinkOneTimeMonsterBlack";

    public static McEyeBlinkOneTimeMonsterBlack New()
    {
        McEyeBlinkOneTimeMonsterBlack mcEyeBlinkOneTimeMonsterBlack = StaticPool<McEyeBlinkOneTimeMonsterBlack>.New();
        mcEyeBlinkOneTimeMonsterBlack.RefreshProperties();
        return mcEyeBlinkOneTimeMonsterBlack;
    }

    public McEyeBlinkOneTimeMonsterBlack()
        : base("common2/McEyeBlinkOneTimeMonsterBlack")
    {
    }

    public void Free()
    {
        StaticPool<McEyeBlinkOneTimeMonsterBlack>.Free(this);
    }
}
