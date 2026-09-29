using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McShadowView0 : Sprite, IFreeable, IId
    {
        public const string ID = "common/McShadowView0";

        public string Id => "common/McShadowView0";

        public static McShadowView0 New()
        {
            McShadowView0 mcShadowView = StaticPool.New<McShadowView0>();
            mcShadowView.RefreshProperties();
            return mcShadowView;
        }

        public McShadowView0()
            : base("common/McShadowView0")
        {
        }

        public void Free()
        {
            StaticPool.Free<McShadowView0>(this);
        }
    }
}
