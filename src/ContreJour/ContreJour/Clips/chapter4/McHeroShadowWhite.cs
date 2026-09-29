using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McHeroShadowWhite : Sprite, IFreeable, IId
    {
        public const string ID = "chapter4/McHeroShadowWhite";

        public string Id => "chapter4/McHeroShadowWhite";

        public static McHeroShadowWhite New()
        {
            McHeroShadowWhite mcHeroShadowWhite = StaticPool.New<McHeroShadowWhite>();
            mcHeroShadowWhite.RefreshProperties();
            return mcHeroShadowWhite;
        }

        public McHeroShadowWhite()
            : base("chapter4/McHeroShadowWhite")
        {
        }

        public void Free()
        {
            StaticPool.Free<McHeroShadowWhite>(this);
        }
    }
}
