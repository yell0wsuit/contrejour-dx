using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFly : Sprite, IFreeable, IId
{
    public const string ID = "common/McFly";

    public string Id => "common/McFly";

    public static McFly New()
    {
        McFly mcFly = StaticPool<McFly>.New();
        mcFly.RefreshProperties();
        return mcFly;
    }

    public McFly()
        : base("common/McFly")
    {
    }

    public void Free()
    {
        StaticPool<McFly>.Free(this);
    }
}
