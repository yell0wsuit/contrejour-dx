using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeClose : MovieClip, IFreeable, IId
{
    public const string ID = "common/McEyeClose";

    public string Id => "common/McEyeClose";

    public static McEyeClose New()
    {
        McEyeClose mcEyeClose = StaticPool<McEyeClose>.New();
        mcEyeClose.RefreshProperties();
        return mcEyeClose;
    }

    public McEyeClose()
        : base("common/McEyeClose")
    {
    }

    public void Free()
    {
        StaticPool<McEyeClose>.Free(this);
    }
}
