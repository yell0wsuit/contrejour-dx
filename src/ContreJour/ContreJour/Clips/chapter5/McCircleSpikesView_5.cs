using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McCircleSpikesView_5 : MovieClip, IFreeable, IId
    {
        public const string ID = "chapter5/McCircleSpikesView_5";

        public string Id => "chapter5/McCircleSpikesView_5";

        public static McCircleSpikesView_5 New()
        {
            McCircleSpikesView_5 mcCircleSpikesView_ = StaticPool.New<McCircleSpikesView_5>();
            mcCircleSpikesView_.RefreshProperties();
            return mcCircleSpikesView_;
        }

        public McCircleSpikesView_5()
            : base("chapter5/McCircleSpikesView_5")
        {
        }

        public void Free()
        {
            StaticPool.Free<McCircleSpikesView_5>(this);
        }
    }
}
