using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McCircleSpikesView_1 : MovieClip, IFreeable, IId
    {
        public const string ID = "chapter2/McCircleSpikesView_1";

        public string Id => "chapter2/McCircleSpikesView_1";

        public static McCircleSpikesView_1 New()
        {
            McCircleSpikesView_1 mcCircleSpikesView_ = StaticPool.New<McCircleSpikesView_1>();
            mcCircleSpikesView_.RefreshProperties();
            return mcCircleSpikesView_;
        }

        public McCircleSpikesView_1()
            : base("chapter2/McCircleSpikesView_1")
        {
        }

        public void Free()
        {
            StaticPool.Free<McCircleSpikesView_1>(this);
        }
    }
}
