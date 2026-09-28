using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.finalLevel;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEndRose : MovieClip, IFreeable, IId
{
    public const string ID = "finalLevel/McEndRose";

    public string Id => "finalLevel/McEndRose";

    public static McEndRose New()
    {
        McEndRose mcEndRose = StaticPool<McEndRose>.New();
        mcEndRose.RefreshProperties();
        return mcEndRose;
    }

    public McEndRose()
        : base("finalLevel/McEndRose")
    {
    }

    public void Free()
    {
        StaticPool<McEndRose>.Free(this);
    }
}
