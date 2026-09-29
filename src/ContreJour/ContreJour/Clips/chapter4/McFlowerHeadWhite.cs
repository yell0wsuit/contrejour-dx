using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFlowerHeadWhite : Sprite, IFreeable, IId
{
    public const string ID = "chapter4/McFlowerHeadWhite";

    public string Id => "chapter4/McFlowerHeadWhite";

    public static McFlowerHeadWhite New()
    {
        McFlowerHeadWhite mcFlowerHeadWhite = StaticPool.New<McFlowerHeadWhite>();
        mcFlowerHeadWhite.RefreshProperties();
        return mcFlowerHeadWhite;
    }

    public McFlowerHeadWhite()
        : base("chapter4/McFlowerHeadWhite")
    {
    }

    public void Free()
    {
        StaticPool.Free<McFlowerHeadWhite>(this);
    }
}
