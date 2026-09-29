using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McCircleSpikesView : MovieClip, IFreeable, IId
    {
        public const string ID = "common/McCircleSpikesView";

        public string Id => "common/McCircleSpikesView";

        public static McCircleSpikesView New()
        {
            McCircleSpikesView mcCircleSpikesView = StaticPool.New<McCircleSpikesView>();
            mcCircleSpikesView.RefreshProperties();
            return mcCircleSpikesView;
        }

        public McCircleSpikesView()
            : base("common/McCircleSpikesView")
        {
        }

        public void Free()
        {
            StaticPool.Free<McCircleSpikesView>(this);
        }
    }
}
