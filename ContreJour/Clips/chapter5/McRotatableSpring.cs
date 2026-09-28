using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRotatableSpring : MovieClip, IFreeable, IId
{
    public const string ID = "chapter5/McRotatableSpring";

    public string Id => "chapter5/McRotatableSpring";

    public static McRotatableSpring New()
    {
        McRotatableSpring mcRotatableSpring = StaticPool<McRotatableSpring>.New();
        mcRotatableSpring.RefreshProperties();
        return mcRotatableSpring;
    }

    public McRotatableSpring()
        : base("chapter5/McRotatableSpring")
    {
    }

    public void Free()
    {
        StaticPool<McRotatableSpring>.Free(this);
    }
}
