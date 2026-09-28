using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeCloseMonster : MovieClip, IFreeable, IId
{
    public const string ID = "common/McEyeCloseMonster";

    public string Id => "common/McEyeCloseMonster";

    public static McEyeCloseMonster New()
    {
        McEyeCloseMonster mcEyeCloseMonster = StaticPool<McEyeCloseMonster>.New();
        mcEyeCloseMonster.RefreshProperties();
        return mcEyeCloseMonster;
    }

    public McEyeCloseMonster()
        : base("common/McEyeCloseMonster")
    {
    }

    public void Free()
    {
        StaticPool<McEyeCloseMonster>.Free(this);
    }
}
