using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McWhiteSmoke : Sprite, IFreeable, IId
{
    public const string ID = "common/McWhiteSmoke";

    public string Id => "common/McWhiteSmoke";

    public static McWhiteSmoke New()
    {
        McWhiteSmoke mcWhiteSmoke = StaticPool<McWhiteSmoke>.New();
        mcWhiteSmoke.RefreshProperties();
        return mcWhiteSmoke;
    }

    public McWhiteSmoke()
        : base("common/McWhiteSmoke")
    {
    }

    public void Free()
    {
        StaticPool<McWhiteSmoke>.Free(this);
    }
}
