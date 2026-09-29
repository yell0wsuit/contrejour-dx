using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFallParticle : MovieClip, IFreeable, IId
{
    public const string ID = "common/McFallParticle";

    public string Id => "common/McFallParticle";

    public static McFallParticle New()
    {
        McFallParticle mcFallParticle = StaticPool.New<McFallParticle>();
        mcFallParticle.RefreshProperties();
        return mcFallParticle;
    }

    public McFallParticle()
        : base("common/McFallParticle")
    {
    }

    public void Free()
    {
        StaticPool.Free<McFallParticle>(this);
    }
}
