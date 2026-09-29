using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McFakeHeroEyeOpen : MovieClip, IFreeable, IId
    {
        public const string ID = "fakeHero/McFakeHeroEyeOpen";

        public string Id => "fakeHero/McFakeHeroEyeOpen";

        public static McFakeHeroEyeOpen New()
        {
            McFakeHeroEyeOpen mcFakeHeroEyeOpen = StaticPool.New<McFakeHeroEyeOpen>();
            mcFakeHeroEyeOpen.RefreshProperties();
            return mcFakeHeroEyeOpen;
        }

        public McFakeHeroEyeOpen()
            : base("fakeHero/McFakeHeroEyeOpen")
        {
        }

        public void Free()
        {
            StaticPool.Free<McFakeHeroEyeOpen>(this);
        }
    }
}
