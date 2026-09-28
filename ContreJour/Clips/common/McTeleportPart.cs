using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McTeleportPart : Sprite, IFreeable, IId
{
    public const string ID = "common/McTeleportPart";

    public string Id => "common/McTeleportPart";

    public static McTeleportPart New()
    {
        McTeleportPart mcTeleportPart = StaticPool<McTeleportPart>.New();
        mcTeleportPart.RefreshProperties();
        return mcTeleportPart;
    }

    public McTeleportPart()
        : base("common/McTeleportPart")
    {
    }

    public void Free()
    {
        StaticPool<McTeleportPart>.Free(this);
    }
}
