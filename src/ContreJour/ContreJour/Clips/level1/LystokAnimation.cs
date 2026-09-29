using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class LystokAnimation : AnimationNode, IFreeable, IId
{
    public const string ID = "level1/LystokAnimation";

    public McLystok1Content instance5231921 { get; protected set; }

    public string Id => "level1/LystokAnimation";

    public static LystokAnimation New()
    {
        LystokAnimation lystokAnimation = StaticPool.New<LystokAnimation>();
        lystokAnimation.RefreshProperties();
        return lystokAnimation;
    }

    public LystokAnimation()
        : base("level1/LystokAnimation")
    {
        instance5231921 = new McLystok1Content();
        AddChild("instance5231921", instance5231921);
        Initialize();
    }

    public void Free()
    {
        StaticPool.Free<LystokAnimation>(this);
    }
}
