using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McFakeHeroEyeBlinkBlack : MovieClip, IFreeable, IId
    {
        public const string ID = "fakeHero/McFakeHeroEyeBlinkBlack";

        public string Id => "fakeHero/McFakeHeroEyeBlinkBlack";

        public static McFakeHeroEyeBlinkBlack New()
        {
            McFakeHeroEyeBlinkBlack mcFakeHeroEyeBlinkBlack = StaticPool.New<McFakeHeroEyeBlinkBlack>();
            mcFakeHeroEyeBlinkBlack.RefreshProperties();
            return mcFakeHeroEyeBlinkBlack;
        }

        public McFakeHeroEyeBlinkBlack()
            : base("fakeHero/McFakeHeroEyeBlinkBlack")
        {
        }

        public void Free()
        {
            StaticPool.Free<McFakeHeroEyeBlinkBlack>(this);
        }
    }
}
