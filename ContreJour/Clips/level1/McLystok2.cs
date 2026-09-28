using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLystok2 : AnimationNode, IFreeable, IId
{
    public const string ID = "level1/McLystok2";

    public McLystok2Content instance5232095 { get; protected set; }

    public string Id => "level1/McLystok2";

    public static McLystok2 New()
    {
        McLystok2 mcLystok = StaticPool<McLystok2>.New();
        mcLystok.RefreshProperties();
        return mcLystok;
    }

    public McLystok2()
        : base("level1/McLystok2")
    {
        instance5232095 = new McLystok2Content();
        AddChild("instance5232095", instance5232095);
        Initialize();
    }

    public void Free()
    {
        StaticPool<McLystok2>.Free(this);
    }
}
