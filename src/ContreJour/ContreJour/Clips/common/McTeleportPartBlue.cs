using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McTeleportPartBlue : Sprite, IFreeable, IId
{
    public const string ID = "common/McTeleportPartBlue";

    public string Id => "common/McTeleportPartBlue";

    public static McTeleportPartBlue New()
    {
        McTeleportPartBlue mcTeleportPartBlue = StaticPool.New<McTeleportPartBlue>();
        mcTeleportPartBlue.RefreshProperties();
        return mcTeleportPartBlue;
    }

    public McTeleportPartBlue()
        : base("common/McTeleportPartBlue")
    {
    }

    public void Free()
    {
        StaticPool.Free<McTeleportPartBlue>(this);
    }
}
