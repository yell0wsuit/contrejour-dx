using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRotatorTouch : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McRotatorTouch";

    public string Id => "chapter5/McRotatorTouch";

    public static McRotatorTouch New()
    {
        McRotatorTouch mcRotatorTouch = StaticPool<McRotatorTouch>.New();
        mcRotatorTouch.RefreshProperties();
        return mcRotatorTouch;
    }

    public McRotatorTouch()
        : base("chapter5/McRotatorTouch")
    {
    }

    public void Free()
    {
        StaticPool<McRotatorTouch>.Free(this);
    }
}
