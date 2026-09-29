using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McEggView2 : Sprite, IFreeable, IId
    {
        public const string ID = "chapter4/McEggView2";

        public string Id => "chapter4/McEggView2";

        public static McEggView2 New()
        {
            McEggView2 mcEggView = StaticPool.New<McEggView2>();
            mcEggView.RefreshProperties();
            return mcEggView;
        }

        public McEggView2()
            : base("chapter4/McEggView2")
        {
        }

        public void Free()
        {
            StaticPool.Free<McEggView2>(this);
        }
    }
}
