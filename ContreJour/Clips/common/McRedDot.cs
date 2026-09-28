using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRedDot : Sprite, IFreeable, IId
{
    public const string ID = "common/McRedDot";

    public string Id => "common/McRedDot";

    public static McRedDot New()
    {
        McRedDot mcRedDot = StaticPool<McRedDot>.New();
        mcRedDot.RefreshProperties();
        return mcRedDot;
    }

    public McRedDot()
        : base("common/McRedDot")
    {
    }

    public void Free()
    {
        StaticPool<McRedDot>.Free(this);
    }
}
