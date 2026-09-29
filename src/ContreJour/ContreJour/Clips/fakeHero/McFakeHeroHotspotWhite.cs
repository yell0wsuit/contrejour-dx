using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McFakeHeroHotspotWhite : Sprite, IFreeable, IId
    {
        public const string ID = "fakeHero/McFakeHeroHotspotWhite";

        public string Id => "fakeHero/McFakeHeroHotspotWhite";

        public static McFakeHeroHotspotWhite New()
        {
            McFakeHeroHotspotWhite mcFakeHeroHotspotWhite = StaticPool.New<McFakeHeroHotspotWhite>();
            mcFakeHeroHotspotWhite.RefreshProperties();
            return mcFakeHeroHotspotWhite;
        }

        public McFakeHeroHotspotWhite()
            : base("fakeHero/McFakeHeroHotspotWhite")
        {
        }

        public void Free()
        {
            StaticPool.Free<McFakeHeroHotspotWhite>(this);
        }
    }
}
