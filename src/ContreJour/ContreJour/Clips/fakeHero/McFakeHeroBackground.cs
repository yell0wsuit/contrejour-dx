using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McFakeHeroBackground : Sprite, IFreeable, IId
    {
        public const string ID = "fakeHero/McFakeHeroBackground";

        public string Id => "fakeHero/McFakeHeroBackground";

        public static McFakeHeroBackground New()
        {
            McFakeHeroBackground mcFakeHeroBackground = StaticPool.New<McFakeHeroBackground>();
            mcFakeHeroBackground.RefreshProperties();
            return mcFakeHeroBackground;
        }

        public McFakeHeroBackground()
            : base("fakeHero/McFakeHeroBackground")
        {
        }

        public void Free()
        {
            StaticPool.Free<McFakeHeroBackground>(this);
        }
    }
}
