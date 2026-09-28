using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.lights;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLightView1 : AnimationNode, IFreeable, IId
{
    public const string ID = "lights/McLightView1";

    public McLightView1Content instance5230306 { get; protected set; }

    public string Id => "lights/McLightView1";

    public static McLightView1 New()
    {
        McLightView1 mcLightView = StaticPool.New<McLightView1>();
        mcLightView.RefreshProperties();
        return mcLightView;
    }

    public McLightView1()
        : base("lights/McLightView1")
    {
        instance5230306 = new McLightView1Content();
        AddChild("instance5230306", instance5230306);
        Initialize();
    }

    public void Free()
    {
        StaticPool.Free<McLightView1>(this);
    }
}
