using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFurCircle : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McFurCircle";

    public string Id => "chapter5/McFurCircle";

    public static McFurCircle New()
    {
        McFurCircle mcFurCircle = StaticPool<McFurCircle>.New();
        mcFurCircle.RefreshProperties();
        return mcFurCircle;
    }

    public McFurCircle()
        : base("chapter5/McFurCircle")
    {
    }

    public void Free()
    {
        StaticPool<McFurCircle>.Free(this);
    }
}
