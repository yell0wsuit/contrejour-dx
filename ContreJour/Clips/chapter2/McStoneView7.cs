using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McStoneView7 : Sprite, IFreeable, IId
{
    public const string ID = "chapter2/McStoneView7";

    public string Id => "chapter2/McStoneView7";

    public static McStoneView7 New()
    {
        McStoneView7 mcStoneView = StaticPool<McStoneView7>.New();
        mcStoneView.RefreshProperties();
        return mcStoneView;
    }

    public McStoneView7()
        : base("chapter2/McStoneView7")
    {
    }

    public void Free()
    {
        StaticPool<McStoneView7>.Free(this);
    }
}
