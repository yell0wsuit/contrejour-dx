using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McArrowView : Sprite, IFreeable, IId
    {
        public const string ID = "common/McArrowView";

        public string Id => "common/McArrowView";

        public static McArrowView New()
        {
            McArrowView mcArrowView = StaticPool.New<McArrowView>();
            mcArrowView.RefreshProperties();
            return mcArrowView;
        }

        public McArrowView()
            : base("common/McArrowView")
        {
        }

        public void Free()
        {
            StaticPool.Free<McArrowView>(this);
        }
    }
}
