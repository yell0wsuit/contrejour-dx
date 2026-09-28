using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSunBackground : Sprite, IFreeable, IId
{
    public const string ID = "level1/McSunBackground";

    public string Id => "level1/McSunBackground";

    public static McSunBackground New()
    {
        McSunBackground mcSunBackground = StaticPool<McSunBackground>.New();
        mcSunBackground.RefreshProperties();
        return mcSunBackground;
    }

    public McSunBackground()
        : base("level1/McSunBackground")
    {
    }

    public void Free()
    {
        StaticPool<McSunBackground>.Free(this);
    }
}
