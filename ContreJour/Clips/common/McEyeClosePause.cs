using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeClosePause : MovieClip, IFreeable, IId
{
    public const string ID = "common/McEyeClosePause";

    public string Id => "common/McEyeClosePause";

    public static McEyeClosePause New()
    {
        McEyeClosePause mcEyeClosePause = StaticPool<McEyeClosePause>.New();
        mcEyeClosePause.RefreshProperties();
        return mcEyeClosePause;
    }

    public McEyeClosePause()
        : base("common/McEyeClosePause")
    {
    }

    public void Free()
    {
        StaticPool<McEyeClosePause>.Free(this);
    }
}
