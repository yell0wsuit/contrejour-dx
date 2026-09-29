using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McFakeHeroEyeBlink : MovieClip, IFreeable, IId
    {
        public const string ID = "fakeHero/McFakeHeroEyeBlink";

        public string Id => "fakeHero/McFakeHeroEyeBlink";

        public static McFakeHeroEyeBlink New()
        {
            McFakeHeroEyeBlink mcFakeHeroEyeBlink = StaticPool.New<McFakeHeroEyeBlink>();
            mcFakeHeroEyeBlink.RefreshProperties();
            return mcFakeHeroEyeBlink;
        }

        public McFakeHeroEyeBlink()
            : base("fakeHero/McFakeHeroEyeBlink")
        {
        }

        public void Free()
        {
            StaticPool.Free<McFakeHeroEyeBlink>(this);
        }
    }
}
