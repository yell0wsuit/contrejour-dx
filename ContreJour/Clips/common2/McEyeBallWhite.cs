using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeBallWhite : Sprite, IFreeable, IId
{
    public const string ID = "common2/McEyeBallWhite";

    public string Id => "common2/McEyeBallWhite";

    public static McEyeBallWhite New()
    {
        McEyeBallWhite mcEyeBallWhite = StaticPool<McEyeBallWhite>.New();
        mcEyeBallWhite.RefreshProperties();
        return mcEyeBallWhite;
    }

    public McEyeBallWhite()
        : base("common2/McEyeBallWhite")
    {
    }

    public void Free()
    {
        StaticPool<McEyeBallWhite>.Free(this);
    }
}
