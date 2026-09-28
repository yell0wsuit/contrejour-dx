using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBlackGround0 : Sprite, IFreeable, IId
{
    public const string ID = "chapter2/McBlackGround0";

    public string Id => "chapter2/McBlackGround0";

    public static McBlackGround0 New()
    {
        McBlackGround0 mcBlackGround = StaticPool<McBlackGround0>.New();
        mcBlackGround.RefreshProperties();
        return mcBlackGround;
    }

    public McBlackGround0()
        : base("chapter2/McBlackGround0")
    {
    }

    public void Free()
    {
        StaticPool<McBlackGround0>.Free(this);
    }
}
