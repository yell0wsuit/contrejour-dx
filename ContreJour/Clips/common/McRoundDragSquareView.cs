using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRoundDragSquareView : Sprite, IFreeable, IId
{
    public const string ID = "common/McRoundDragSquareView";

    public string Id => "common/McRoundDragSquareView";

    public static McRoundDragSquareView New()
    {
        McRoundDragSquareView mcRoundDragSquareView = StaticPool<McRoundDragSquareView>.New();
        mcRoundDragSquareView.RefreshProperties();
        return mcRoundDragSquareView;
    }

    public McRoundDragSquareView()
        : base("common/McRoundDragSquareView")
    {
    }

    public void Free()
    {
        StaticPool<McRoundDragSquareView>.Free(this);
    }
}
