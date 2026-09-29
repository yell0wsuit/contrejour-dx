using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McMultitouchHintView : Sprite, IFreeable, IId
    {
        public const string ID = "chapter1/McMultitouchHintView";

        public string Id => "chapter1/McMultitouchHintView";

        public static McMultitouchHintView New()
        {
            McMultitouchHintView mcMultitouchHintView = StaticPool.New<McMultitouchHintView>();
            mcMultitouchHintView.RefreshProperties();
            return mcMultitouchHintView;
        }

        public McMultitouchHintView()
            : base("chapter1/McMultitouchHintView")
        {
        }

        public void Free()
        {
            StaticPool.Free<McMultitouchHintView>(this);
        }
    }
}
