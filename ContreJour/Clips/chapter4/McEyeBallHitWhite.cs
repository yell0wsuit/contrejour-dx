using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeBallHitWhite : AnimationNode, IFreeable, IId
{
    public const string ID = "chapter4/McEyeBallHitWhite";

    public McEyeBallWhite instance5248083 { get; protected set; }

    public McEyeBallWhite instance5248089 { get; protected set; }

    public string Id => "chapter4/McEyeBallHitWhite";

    public static McEyeBallHitWhite New()
    {
        McEyeBallHitWhite mcEyeBallHitWhite = StaticPool<McEyeBallHitWhite>.New();
        mcEyeBallHitWhite.RefreshProperties();
        return mcEyeBallHitWhite;
    }

    public McEyeBallHitWhite()
        : base("chapter4/McEyeBallHitWhite")
    {
        instance5248083 = new McEyeBallWhite();
        AddChild("instance5248083", instance5248083);
        instance5248089 = new McEyeBallWhite();
        AddChild("instance5248089", instance5248089);
        Initialize();
    }

    public void Free()
    {
        StaticPool<McEyeBallHitWhite>.Free(this);
    }
}
