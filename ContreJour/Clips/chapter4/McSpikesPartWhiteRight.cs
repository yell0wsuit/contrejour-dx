using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSpikesPartWhiteRight : MovieClip, IFreeable, IId
{
    public const string ID = "chapter4/McSpikesPartWhiteRight";

    public string Id => "chapter4/McSpikesPartWhiteRight";

    public static McSpikesPartWhiteRight New()
    {
        McSpikesPartWhiteRight mcSpikesPartWhiteRight = StaticPool<McSpikesPartWhiteRight>.New();
        mcSpikesPartWhiteRight.RefreshProperties();
        return mcSpikesPartWhiteRight;
    }

    public McSpikesPartWhiteRight()
        : base("chapter4/McSpikesPartWhiteRight")
    {
    }

    public void Free()
    {
        StaticPool<McSpikesPartWhiteRight>.Free(this);
    }
}
