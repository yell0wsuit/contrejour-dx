using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFlyBody : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McFlyBody";

    public string Id => "chapter5/McFlyBody";

    public static McFlyBody New()
    {
        McFlyBody mcFlyBody = StaticPool<McFlyBody>.New();
        mcFlyBody.RefreshProperties();
        return mcFlyBody;
    }

    public McFlyBody()
        : base("chapter5/McFlyBody")
    {
    }

    public void Free()
    {
        StaticPool<McFlyBody>.Free(this);
    }
}
