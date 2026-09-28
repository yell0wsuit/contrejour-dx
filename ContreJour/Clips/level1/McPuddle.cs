using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPuddle : AnimationNode, IFreeable, IId
{
    public const string ID = "level1/McPuddle";

    public McPuddleContent instance5232114 { get; protected set; }

    public string Id => "level1/McPuddle";

    public static McPuddle New()
    {
        McPuddle mcPuddle = StaticPool.New<McPuddle>();
        mcPuddle.RefreshProperties();
        return mcPuddle;
    }

    public McPuddle()
        : base("level1/McPuddle")
    {
        instance5232114 = new McPuddleContent();
        AddChild("instance5232114", instance5232114);
        Initialize();
    }

    public void Free()
    {
        StaticPool.Free<McPuddle>(this);
    }
}
