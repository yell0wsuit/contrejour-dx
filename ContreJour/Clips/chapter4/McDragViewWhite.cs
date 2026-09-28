using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McDragViewWhite : Sprite, IFreeable, IId
{
    public const string ID = "chapter4/McDragViewWhite";

    public string Id => "chapter4/McDragViewWhite";

    public static McDragViewWhite New()
    {
        McDragViewWhite mcDragViewWhite = StaticPool<McDragViewWhite>.New();
        mcDragViewWhite.RefreshProperties();
        return mcDragViewWhite;
    }

    public McDragViewWhite()
        : base("chapter4/McDragViewWhite")
    {
    }

    public void Free()
    {
        StaticPool<McDragViewWhite>.Free(this);
    }
}
