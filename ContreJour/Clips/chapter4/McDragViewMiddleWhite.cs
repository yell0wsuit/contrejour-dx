using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McDragViewMiddleWhite : Sprite, IFreeable, IId
{
    public const string ID = "chapter4/McDragViewMiddleWhite";

    public string Id => "chapter4/McDragViewMiddleWhite";

    public static McDragViewMiddleWhite New()
    {
        McDragViewMiddleWhite mcDragViewMiddleWhite = StaticPool.New<McDragViewMiddleWhite>();
        mcDragViewMiddleWhite.RefreshProperties();
        return mcDragViewMiddleWhite;
    }

    public McDragViewMiddleWhite()
        : base("chapter4/McDragViewMiddleWhite")
    {
    }

    public void Free()
    {
        StaticPool.Free<McDragViewMiddleWhite>(this);
    }
}
