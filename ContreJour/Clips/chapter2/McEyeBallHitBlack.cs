using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeBallHitBlack : AnimationNode, IFreeable, IId
{
    public const string ID = "chapter2/McEyeBallHitBlack";

    public McEyeBallBlack instance24962 { get; protected set; }

    public McEyeBallBlack instance24968 { get; protected set; }

    public string Id => "chapter2/McEyeBallHitBlack";

    public static McEyeBallHitBlack New()
    {
        McEyeBallHitBlack mcEyeBallHitBlack = StaticPool<McEyeBallHitBlack>.New();
        mcEyeBallHitBlack.RefreshProperties();
        return mcEyeBallHitBlack;
    }

    public McEyeBallHitBlack()
        : base("chapter2/McEyeBallHitBlack")
    {
        instance24962 = new McEyeBallBlack();
        AddChild("instance24962", instance24962);
        instance24968 = new McEyeBallBlack();
        AddChild("instance24968", instance24968);
        Initialize();
    }

    public void Free()
    {
        StaticPool<McEyeBallHitBlack>.Free(this);
    }
}
