using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McGreySmoke : Sprite, IFreeable, IId
{
    public const string ID = "common/McGreySmoke";

    public string Id => "common/McGreySmoke";

    public static McGreySmoke New()
    {
        McGreySmoke mcGreySmoke = StaticPool<McGreySmoke>.New();
        mcGreySmoke.RefreshProperties();
        return mcGreySmoke;
    }

    public McGreySmoke()
        : base("common/McGreySmoke")
    {
    }

    public void Free()
    {
        StaticPool<McGreySmoke>.Free(this);
    }
}
