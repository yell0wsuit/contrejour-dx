using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McTestAnimation2 : AnimationNode, IFreeable, IId
{
    public const string ID = "level1/McTestAnimation2";

    public McRoseLightBlue2 light { get; protected set; }

    public string Id => "level1/McTestAnimation2";

    public static McTestAnimation2 New()
    {
        McTestAnimation2 mcTestAnimation = StaticPool.New<McTestAnimation2>();
        mcTestAnimation.RefreshProperties();
        return mcTestAnimation;
    }

    public McTestAnimation2()
        : base("level1/McTestAnimation2")
    {
        light = new McRoseLightBlue2();
        AddChild("light", light);
        Initialize();
    }

    public void Free()
    {
        StaticPool.Free<McTestAnimation2>(this);
    }
}
