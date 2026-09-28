using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLystok1 : AnimationNode, IFreeable, IId
{
    public const string ID = "level1/McLystok1";

    public McLystok1Content instance5232087 { get; protected set; }

    public string Id => "level1/McLystok1";

    public static McLystok1 New()
    {
        McLystok1 mcLystok = StaticPool<McLystok1>.New();
        mcLystok.RefreshProperties();
        return mcLystok;
    }

    public McLystok1()
        : base("level1/McLystok1")
    {
        instance5232087 = new McLystok1Content();
        AddChild("instance5232087", instance5232087);
        Initialize();
    }

    public void Free()
    {
        StaticPool<McLystok1>.Free(this);
    }
}
