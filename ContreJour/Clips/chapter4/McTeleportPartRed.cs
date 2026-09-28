using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McTeleportPartRed : Sprite, IFreeable, IId
{
    public const string ID = "chapter4/McTeleportPartRed";

    public string Id => "chapter4/McTeleportPartRed";

    public static McTeleportPartRed New()
    {
        McTeleportPartRed mcTeleportPartRed = StaticPool<McTeleportPartRed>.New();
        mcTeleportPartRed.RefreshProperties();
        return mcTeleportPartRed;
    }

    public McTeleportPartRed()
        : base("chapter4/McTeleportPartRed")
    {
    }

    public void Free()
    {
        StaticPool<McTeleportPartRed>.Free(this);
    }
}
