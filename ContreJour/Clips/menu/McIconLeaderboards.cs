using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McIconLeaderboards : Sprite, IFreeable, IId
{
    public const string ID = "menu/McIconLeaderboards";

    public string Id => "menu/McIconLeaderboards";

    public static McIconLeaderboards New()
    {
        McIconLeaderboards mcIconLeaderboards = StaticPool.New<McIconLeaderboards>();
        mcIconLeaderboards.RefreshProperties();
        return mcIconLeaderboards;
    }

    public McIconLeaderboards()
        : base("menu/McIconLeaderboards")
    {
    }

    public void Free()
    {
        StaticPool.Free<McIconLeaderboards>(this);
    }
}
