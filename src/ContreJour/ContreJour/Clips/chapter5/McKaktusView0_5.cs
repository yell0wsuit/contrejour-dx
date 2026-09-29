using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McKaktusView0_5 : Sprite, IFreeable, IId
    {
        public const string ID = "chapter5/McKaktusView0_5";

        public string Id => "chapter5/McKaktusView0_5";

        public static McKaktusView0_5 New()
        {
            McKaktusView0_5 mcKaktusView0_ = StaticPool.New<McKaktusView0_5>();
            mcKaktusView0_.RefreshProperties();
            return mcKaktusView0_;
        }

        public McKaktusView0_5()
            : base("chapter5/McKaktusView0_5")
        {
        }

        public void Free()
        {
            StaticPool.Free<McKaktusView0_5>(this);
        }
    }
}
