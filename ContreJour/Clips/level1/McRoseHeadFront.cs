using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRoseHeadFront : AnimationNode, IFreeable, IId
{
    public const string ID = "level1/McRoseHeadFront";

    public PelustokNoLight instance5232242 { get; protected set; }

    public PelustokNoLight instance5232245 { get; protected set; }

    public McRoseHeadBase2 instance5232248 { get; protected set; }

    public string Id => "level1/McRoseHeadFront";

    public static McRoseHeadFront New()
    {
        McRoseHeadFront mcRoseHeadFront = StaticPool<McRoseHeadFront>.New();
        mcRoseHeadFront.RefreshProperties();
        return mcRoseHeadFront;
    }

    public McRoseHeadFront()
        : base("level1/McRoseHeadFront")
    {
        instance5232242 = new PelustokNoLight();
        AddChild("instance5232242", instance5232242);
        instance5232245 = new PelustokNoLight();
        AddChild("instance5232245", instance5232245);
        instance5232248 = new McRoseHeadBase2();
        AddChild("instance5232248", instance5232248);
        Initialize();
    }

    public void Free()
    {
        StaticPool<McRoseHeadFront>.Free(this);
    }
}
