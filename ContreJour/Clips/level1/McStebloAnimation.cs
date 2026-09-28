using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McStebloAnimation : MovieClip, IFreeable, IId
{
    public const string ID = "level1/McStebloAnimation";

    public string Id => "level1/McStebloAnimation";

    public static McStebloAnimation New()
    {
        McStebloAnimation mcStebloAnimation = StaticPool<McStebloAnimation>.New();
        mcStebloAnimation.RefreshProperties();
        return mcStebloAnimation;
    }

    public McStebloAnimation()
        : base("level1/McStebloAnimation")
    {
    }

    public void Free()
    {
        StaticPool<McStebloAnimation>.Free(this);
    }
}
