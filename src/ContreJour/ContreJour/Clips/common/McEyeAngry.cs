using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeAngry : MovieClip, IFreeable, IId
{
    public const string ID = "common/McEyeAngry";

    public string Id => "common/McEyeAngry";

    public static McEyeAngry New()
    {
        McEyeAngry mcEyeAngry = StaticPool.New<McEyeAngry>();
        mcEyeAngry.RefreshProperties();
        return mcEyeAngry;
    }

    public McEyeAngry()
        : base("common/McEyeAngry")
    {
    }

    public void Free()
    {
        StaticPool.Free<McEyeAngry>(this);
    }
}
