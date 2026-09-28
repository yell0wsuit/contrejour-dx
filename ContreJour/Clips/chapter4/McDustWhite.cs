using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McDustWhite : Sprite, IFreeable, IId
{
    public const string ID = "chapter4/McDustWhite";

    public string Id => "chapter4/McDustWhite";

    public static McDustWhite New()
    {
        McDustWhite mcDustWhite = StaticPool.New<McDustWhite>();
        mcDustWhite.RefreshProperties();
        return mcDustWhite;
    }

    public McDustWhite()
        : base("chapter4/McDustWhite")
    {
    }

    public void Free()
    {
        StaticPool.Free<McDustWhite>(this);
    }
}
