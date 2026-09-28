using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBlackGround1 : Sprite, IFreeable, IId
{
    public const string ID = "chapter2/McBlackGround1";

    public string Id => "chapter2/McBlackGround1";

    public static McBlackGround1 New()
    {
        McBlackGround1 mcBlackGround = StaticPool.New<McBlackGround1>();
        mcBlackGround.RefreshProperties();
        return mcBlackGround;
    }

    public McBlackGround1()
        : base("chapter2/McBlackGround1")
    {
    }

    public void Free()
    {
        StaticPool.Free<McBlackGround1>(this);
    }
}
