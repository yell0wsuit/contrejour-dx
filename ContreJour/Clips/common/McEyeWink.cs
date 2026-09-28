using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeWink : MovieClip, IFreeable, IId
{
    public const string ID = "common/McEyeWink";

    public string Id => "common/McEyeWink";

    public static McEyeWink New()
    {
        McEyeWink mcEyeWink = StaticPool<McEyeWink>.New();
        mcEyeWink.RefreshProperties();
        return mcEyeWink;
    }

    public McEyeWink()
        : base("common/McEyeWink")
    {
    }

    public void Free()
    {
        StaticPool<McEyeWink>.Free(this);
    }
}
