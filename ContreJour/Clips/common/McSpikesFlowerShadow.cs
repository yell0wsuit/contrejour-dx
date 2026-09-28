using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSpikesFlowerShadow : Sprite, IFreeable, IId
{
    public const string ID = "common/McSpikesFlowerShadow";

    public string Id => "common/McSpikesFlowerShadow";

    public static McSpikesFlowerShadow New()
    {
        McSpikesFlowerShadow mcSpikesFlowerShadow = StaticPool<McSpikesFlowerShadow>.New();
        mcSpikesFlowerShadow.RefreshProperties();
        return mcSpikesFlowerShadow;
    }

    public McSpikesFlowerShadow()
        : base("common/McSpikesFlowerShadow")
    {
    }

    public void Free()
    {
        StaticPool<McSpikesFlowerShadow>.Free(this);
    }
}
