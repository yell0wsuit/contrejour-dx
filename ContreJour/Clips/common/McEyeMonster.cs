using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeMonster : Sprite, IFreeable, IId
{
    public const string ID = "common/McEyeMonster";

    public string Id => "common/McEyeMonster";

    public static McEyeMonster New()
    {
        McEyeMonster mcEyeMonster = StaticPool<McEyeMonster>.New();
        mcEyeMonster.RefreshProperties();
        return mcEyeMonster;
    }

    public McEyeMonster()
        : base("common/McEyeMonster")
    {
    }

    public void Free()
    {
        StaticPool<McEyeMonster>.Free(this);
    }
}
