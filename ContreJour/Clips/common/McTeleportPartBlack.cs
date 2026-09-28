using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McTeleportPartBlack : Sprite, IFreeable, IId
{
    public const string ID = "common/McTeleportPartBlack";

    public string Id => "common/McTeleportPartBlack";

    public static McTeleportPartBlack New()
    {
        McTeleportPartBlack mcTeleportPartBlack = StaticPool<McTeleportPartBlack>.New();
        mcTeleportPartBlack.RefreshProperties();
        return mcTeleportPartBlack;
    }

    public McTeleportPartBlack()
        : base("common/McTeleportPartBlack")
    {
    }

    public void Free()
    {
        StaticPool<McTeleportPartBlack>.Free(this);
    }
}
