using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.lights;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLightView7 : AnimationNode, IFreeable, IId
{
    public const string ID = "lights/McLightView7";

    public McLightView7Content instance5230392 { get; protected set; }

    public string Id => "lights/McLightView7";

    public static McLightView7 New()
    {
        McLightView7 mcLightView = StaticPool<McLightView7>.New();
        mcLightView.RefreshProperties();
        return mcLightView;
    }

    public McLightView7()
        : base("lights/McLightView7")
    {
        instance5230392 = new McLightView7Content();
        AddChild("instance5230392", instance5230392);
        Initialize();
    }

    public void Free()
    {
        StaticPool<McLightView7>.Free(this);
    }
}
