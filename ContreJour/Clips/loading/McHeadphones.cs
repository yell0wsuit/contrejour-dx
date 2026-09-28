using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.loading;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McHeadphones : Sprite, IFreeable, IId
{
    public const string ID = "loading/McHeadphones";

    public string Id => "loading/McHeadphones";

    public static McHeadphones New()
    {
        McHeadphones mcHeadphones = StaticPool<McHeadphones>.New();
        mcHeadphones.RefreshProperties();
        return mcHeadphones;
    }

    public McHeadphones()
        : base("loading/McHeadphones")
    {
    }

    public void Free()
    {
        StaticPool<McHeadphones>.Free(this);
    }
}
