using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McRoundDragView_5 : Sprite, IFreeable, IId
    {
        public const string ID = "chapter5/McRoundDragView_5";

        public string Id => "chapter5/McRoundDragView_5";

        public static McRoundDragView_5 New()
        {
            McRoundDragView_5 mcRoundDragView_ = StaticPool.New<McRoundDragView_5>();
            mcRoundDragView_.RefreshProperties();
            return mcRoundDragView_;
        }

        public McRoundDragView_5()
            : base("chapter5/McRoundDragView_5")
        {
        }

        public void Free()
        {
            StaticPool.Free<McRoundDragView_5>(this);
        }
    }
}
