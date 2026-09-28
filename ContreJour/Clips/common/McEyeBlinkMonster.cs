using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeBlinkMonster : MovieClip, IFreeable, IId
{
    public const string ID = "common/McEyeBlinkMonster";

    public string Id => "common/McEyeBlinkMonster";

    public static McEyeBlinkMonster New()
    {
        McEyeBlinkMonster mcEyeBlinkMonster = StaticPool<McEyeBlinkMonster>.New();
        mcEyeBlinkMonster.RefreshProperties();
        return mcEyeBlinkMonster;
    }

    public McEyeBlinkMonster()
        : base("common/McEyeBlinkMonster")
    {
    }

    public void Free()
    {
        StaticPool<McEyeBlinkMonster>.Free(this);
    }
}
