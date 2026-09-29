using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McFakeHeroHotspot_6 : Sprite, IFreeable, IId
    {
        public const string ID = "fakeHero/McFakeHeroHotspot_6";

        public string Id => "fakeHero/McFakeHeroHotspot_6";

        public static McFakeHeroHotspot_6 New()
        {
            McFakeHeroHotspot_6 mcFakeHeroHotspot_ = StaticPool.New<McFakeHeroHotspot_6>();
            mcFakeHeroHotspot_.RefreshProperties();
            return mcFakeHeroHotspot_;
        }

        public McFakeHeroHotspot_6()
            : base("fakeHero/McFakeHeroHotspot_6")
        {
        }

        public void Free()
        {
            StaticPool.Free<McFakeHeroHotspot_6>(this);
        }
    }
}
