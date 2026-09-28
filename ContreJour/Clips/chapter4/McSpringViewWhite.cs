using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSpringViewWhite : MovieClip, IFreeable, IId
{
    public const string ID = "chapter4/McSpringViewWhite";

    public string Id => "chapter4/McSpringViewWhite";

    public static McSpringViewWhite New()
    {
        McSpringViewWhite mcSpringViewWhite = StaticPool.New<McSpringViewWhite>();
        mcSpringViewWhite.RefreshProperties();
        return mcSpringViewWhite;
    }

    public McSpringViewWhite()
        : base("chapter4/McSpringViewWhite")
    {
    }

    public void Free()
    {
        StaticPool.Free<McSpringViewWhite>(this);
    }
}
