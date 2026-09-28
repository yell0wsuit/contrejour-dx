using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSpikesCenter : Sprite, IFreeable, IId
{
    public const string ID = "common/McSpikesCenter";

    public string Id => "common/McSpikesCenter";

    public static McSpikesCenter New()
    {
        McSpikesCenter mcSpikesCenter = StaticPool<McSpikesCenter>.New();
        mcSpikesCenter.RefreshProperties();
        return mcSpikesCenter;
    }

    public McSpikesCenter()
        : base("common/McSpikesCenter")
    {
    }

    public void Free()
    {
        StaticPool<McSpikesCenter>.Free(this);
    }
}
