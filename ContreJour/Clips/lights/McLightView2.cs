using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.lights;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLightView2 : AnimationNode, IFreeable, IId
{
    public const string ID = "lights/McLightView2";

    public McLightView7Content instance5230372 { get; protected set; }

    public string Id => "lights/McLightView2";

    public static McLightView2 New()
    {
        McLightView2 mcLightView = StaticPool<McLightView2>.New();
        mcLightView.RefreshProperties();
        return mcLightView;
    }

    public McLightView2()
        : base("lights/McLightView2")
    {
        instance5230372 = new McLightView7Content();
        AddChild("instance5230372", instance5230372);
        Initialize();
    }

    public void Free()
    {
        StaticPool<McLightView2>.Free(this);
    }
}
