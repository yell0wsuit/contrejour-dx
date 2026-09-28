using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRoseHeadLightDown : AnimationNode, IFreeable, IId
{
    public const string ID = "level1/McRoseHeadLightDown";

    public PelustokNoLight instance5232203 { get; protected set; }

    public PelustokNoLight instance5232206 { get; protected set; }

    public PelustokNoLight instance5232209 { get; protected set; }

    public McRoseHeadBase instance5232212 { get; protected set; }

    public McRoseLightBlue light { get; protected set; }

    public PelustokNoLight instance5232218 { get; protected set; }

    public PelustokNoLight instance5232221 { get; protected set; }

    public string Id => "level1/McRoseHeadLightDown";

    public static McRoseHeadLightDown New()
    {
        McRoseHeadLightDown mcRoseHeadLightDown = StaticPool<McRoseHeadLightDown>.New();
        mcRoseHeadLightDown.RefreshProperties();
        return mcRoseHeadLightDown;
    }

    public McRoseHeadLightDown()
        : base("level1/McRoseHeadLightDown")
    {
        instance5232203 = new PelustokNoLight();
        AddChild("instance5232203", instance5232203);
        instance5232206 = new PelustokNoLight();
        AddChild("instance5232206", instance5232206);
        instance5232209 = new PelustokNoLight();
        AddChild("instance5232209", instance5232209);
        instance5232212 = new McRoseHeadBase();
        AddChild("instance5232212", instance5232212);
        light = new McRoseLightBlue();
        AddChild("light", light);
        instance5232218 = new PelustokNoLight();
        AddChild("instance5232218", instance5232218);
        instance5232221 = new PelustokNoLight();
        AddChild("instance5232221", instance5232221);
        Initialize();
    }

    public void Free()
    {
        StaticPool<McRoseHeadLightDown>.Free(this);
    }
}
