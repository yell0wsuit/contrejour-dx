using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeOpenMonster : MovieClip, IFreeable, IId
{
    public const string ID = "common/McEyeOpenMonster";

    public string Id => "common/McEyeOpenMonster";

    public static McEyeOpenMonster New()
    {
        McEyeOpenMonster mcEyeOpenMonster = StaticPool<McEyeOpenMonster>.New();
        mcEyeOpenMonster.RefreshProperties();
        return mcEyeOpenMonster;
    }

    public McEyeOpenMonster()
        : base("common/McEyeOpenMonster")
    {
    }

    public void Free()
    {
        StaticPool<McEyeOpenMonster>.Free(this);
    }
}
