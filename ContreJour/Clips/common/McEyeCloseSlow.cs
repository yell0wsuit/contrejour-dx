using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeCloseSlow : MovieClip, IFreeable, IId
{
    public const string ID = "common/McEyeCloseSlow";

    public string Id => "common/McEyeCloseSlow";

    public static McEyeCloseSlow New()
    {
        McEyeCloseSlow mcEyeCloseSlow = StaticPool.New<McEyeCloseSlow>();
        mcEyeCloseSlow.RefreshProperties();
        return mcEyeCloseSlow;
    }

    public McEyeCloseSlow()
        : base("common/McEyeCloseSlow")
    {
    }

    public void Free()
    {
        StaticPool.Free<McEyeCloseSlow>(this);
    }
}
