using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRoundDragFrameView : Sprite, IFreeable, IId
{
    public const string ID = "common/McRoundDragFrameView";

    public string Id => "common/McRoundDragFrameView";

    public static McRoundDragFrameView New()
    {
        McRoundDragFrameView mcRoundDragFrameView = StaticPool.New<McRoundDragFrameView>();
        mcRoundDragFrameView.RefreshProperties();
        return mcRoundDragFrameView;
    }

    public McRoundDragFrameView()
        : base("common/McRoundDragFrameView")
    {
    }

    public void Free()
    {
        StaticPool.Free<McRoundDragFrameView>(this);
    }
}
