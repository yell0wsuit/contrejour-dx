using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFlowerHead : Sprite, IFreeable, IId
{
    public const string ID = "common/McFlowerHead";

    public string Id => "common/McFlowerHead";

    public static McFlowerHead New()
    {
        McFlowerHead mcFlowerHead = StaticPool.New<McFlowerHead>();
        mcFlowerHead.RefreshProperties();
        return mcFlowerHead;
    }

    public McFlowerHead()
        : base("common/McFlowerHead")
    {
    }

    public void Free()
    {
        StaticPool.Free<McFlowerHead>(this);
    }
}
