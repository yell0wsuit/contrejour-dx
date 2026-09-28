using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackground2Back : Sprite, IFreeable, IId
{
    public const string ID = "level1/McBackground2Back";

    public string Id => "level1/McBackground2Back";

    public static McBackground2Back New()
    {
        McBackground2Back mcBackground2Back = StaticPool.New<McBackground2Back>();
        mcBackground2Back.RefreshProperties();
        return mcBackground2Back;
    }

    public McBackground2Back()
        : base("level1/McBackground2Back")
    {
    }

    public void Free()
    {
        StaticPool.Free<McBackground2Back>(this);
    }
}
