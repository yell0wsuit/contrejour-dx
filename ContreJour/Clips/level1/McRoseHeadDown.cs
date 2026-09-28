using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRoseHeadDown : AnimationNode, IFreeable, IId
{
    public const string ID = "level1/McRoseHeadDown";

    public McRoseHeadLightDown content { get; protected set; }

    public string Id => "level1/McRoseHeadDown";

    public static McRoseHeadDown New()
    {
        McRoseHeadDown mcRoseHeadDown = StaticPool<McRoseHeadDown>.New();
        mcRoseHeadDown.RefreshProperties();
        return mcRoseHeadDown;
    }

    public McRoseHeadDown()
        : base("level1/McRoseHeadDown")
    {
        content = new McRoseHeadLightDown();
        AddChild("content", content);
        Initialize();
    }

    public void Free()
    {
        StaticPool<McRoseHeadDown>.Free(this);
    }
}
