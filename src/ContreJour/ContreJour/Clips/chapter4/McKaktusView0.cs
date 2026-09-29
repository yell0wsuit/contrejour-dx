using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McKaktusView0 : Sprite, IFreeable, IId
    {
        public const string ID = "chapter4/McKaktusView0";

        public string Id => "chapter4/McKaktusView0";

        public static McKaktusView0 New()
        {
            McKaktusView0 mcKaktusView = StaticPool.New<McKaktusView0>();
            mcKaktusView.RefreshProperties();
            return mcKaktusView;
        }

        public McKaktusView0()
            : base("chapter4/McKaktusView0")
        {
        }

        public void Free()
        {
            StaticPool.Free<McKaktusView0>(this);
        }
    }
}
