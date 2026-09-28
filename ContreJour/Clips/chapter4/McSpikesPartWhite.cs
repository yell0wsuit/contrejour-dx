using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSpikesPartWhite : MovieClip, IFreeable, IId
{
    public const string ID = "chapter4/McSpikesPartWhite";

    public string Id => "chapter4/McSpikesPartWhite";

    public static McSpikesPartWhite New()
    {
        McSpikesPartWhite mcSpikesPartWhite = StaticPool.New<McSpikesPartWhite>();
        mcSpikesPartWhite.RefreshProperties();
        return mcSpikesPartWhite;
    }

    public McSpikesPartWhite()
        : base("chapter4/McSpikesPartWhite")
    {
    }

    public void Free()
    {
        StaticPool.Free<McSpikesPartWhite>(this);
    }
}
