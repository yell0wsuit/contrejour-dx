using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeBlink : MovieClip, IFreeable, IId
{
    public const string ID = "common/McEyeBlink";

    public string Id => "common/McEyeBlink";

    public static McEyeBlink New()
    {
        McEyeBlink mcEyeBlink = StaticPool<McEyeBlink>.New();
        mcEyeBlink.RefreshProperties();
        return mcEyeBlink;
    }

    public McEyeBlink()
        : base("common/McEyeBlink")
    {
    }

    public void Free()
    {
        StaticPool<McEyeBlink>.Free(this);
    }
}
