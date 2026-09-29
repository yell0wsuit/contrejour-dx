using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McHeroBlackView : Sprite, IFreeable, IId
    {
        public const string ID = "chapter2/McHeroBlackView";

        public string Id => "chapter2/McHeroBlackView";

        public static McHeroBlackView New()
        {
            McHeroBlackView mcHeroBlackView = StaticPool.New<McHeroBlackView>();
            mcHeroBlackView.RefreshProperties();
            return mcHeroBlackView;
        }

        public McHeroBlackView()
            : base("chapter2/McHeroBlackView")
        {
        }

        public void Free()
        {
            StaticPool.Free<McHeroBlackView>(this);
        }
    }
}
