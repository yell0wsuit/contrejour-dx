using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBlackSkyBackground : Sprite, IFreeable, IId
{
    public const string ID = "level1/McBlackSkyBackground";

    public string Id => "level1/McBlackSkyBackground";

    public static McBlackSkyBackground New()
    {
        McBlackSkyBackground mcBlackSkyBackground = StaticPool<McBlackSkyBackground>.New();
        mcBlackSkyBackground.RefreshProperties();
        return mcBlackSkyBackground;
    }

    public McBlackSkyBackground()
        : base("level1/McBlackSkyBackground")
    {
    }

    public void Free()
    {
        StaticPool<McBlackSkyBackground>.Free(this);
    }
}
