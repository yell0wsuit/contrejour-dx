using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.lights;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLightView11 : AnimationNode, IFreeable, IId
{
    public const string ID = "lights/McLightView11";

    public McLightView11ContentExport instance5230326 { get; protected set; }

    public string Id => "lights/McLightView11";

    public static McLightView11 New()
    {
        McLightView11 mcLightView = StaticPool<McLightView11>.New();
        mcLightView.RefreshProperties();
        return mcLightView;
    }

    public McLightView11()
        : base("lights/McLightView11")
    {
        instance5230326 = new McLightView11ContentExport();
        AddChild("instance5230326", instance5230326);
        Initialize();
    }

    public void Free()
    {
        StaticPool<McLightView11>.Free(this);
    }
}
