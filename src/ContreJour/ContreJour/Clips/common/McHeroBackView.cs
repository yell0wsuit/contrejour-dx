using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McHeroBackView : Sprite, IFreeable, IId
    {
        public const string ID = "common/McHeroBackView";

        public string Id => "common/McHeroBackView";

        public static McHeroBackView New()
        {
            McHeroBackView mcHeroBackView = StaticPool.New<McHeroBackView>();
            mcHeroBackView.RefreshProperties();
            return mcHeroBackView;
        }

        public McHeroBackView()
            : base("common/McHeroBackView")
        {
        }

        public void Free()
        {
            StaticPool.Free<McHeroBackView>(this);
        }
    }
}
