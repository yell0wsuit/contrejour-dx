using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRoundDragSquareView : Sprite, IFreeable, IId
{
    public const string ID = "chapter4/McRoundDragSquareView";

    public string Id => "chapter4/McRoundDragSquareView";

    public static McRoundDragSquareView New()
    {
        McRoundDragSquareView mcRoundDragSquareView = StaticPool.New<McRoundDragSquareView>();
        mcRoundDragSquareView.RefreshProperties();
        return mcRoundDragSquareView;
    }

    public McRoundDragSquareView()
        : base("chapter4/McRoundDragSquareView")
    {
    }

    public void Free()
    {
        StaticPool.Free<McRoundDragSquareView>(this);
    }
}
