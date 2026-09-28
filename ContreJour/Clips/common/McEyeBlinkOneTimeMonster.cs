using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeBlinkOneTimeMonster : MovieClip, IFreeable, IId
{
    public const string ID = "common/McEyeBlinkOneTimeMonster";

    public string Id => "common/McEyeBlinkOneTimeMonster";

    public static McEyeBlinkOneTimeMonster New()
    {
        McEyeBlinkOneTimeMonster mcEyeBlinkOneTimeMonster = StaticPool.New<McEyeBlinkOneTimeMonster>();
        mcEyeBlinkOneTimeMonster.RefreshProperties();
        return mcEyeBlinkOneTimeMonster;
    }

    public McEyeBlinkOneTimeMonster()
        : base("common/McEyeBlinkOneTimeMonster")
    {
    }

    public void Free()
    {
        StaticPool.Free<McEyeBlinkOneTimeMonster>(this);
    }
}
