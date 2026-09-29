using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McPortalHintView : Sprite, IFreeable, IId
    {
        public const string ID = "chapter2/McPortalHintView";

        public string Id => "chapter2/McPortalHintView";

        public static McPortalHintView New()
        {
            McPortalHintView mcPortalHintView = StaticPool.New<McPortalHintView>();
            mcPortalHintView.RefreshProperties();
            return mcPortalHintView;
        }

        public McPortalHintView()
            : base("chapter2/McPortalHintView")
        {
        }

        public void Free()
        {
            StaticPool.Free<McPortalHintView>(this);
        }
    }
}
