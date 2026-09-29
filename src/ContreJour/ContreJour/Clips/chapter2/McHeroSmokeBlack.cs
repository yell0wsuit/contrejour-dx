using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McHeroSmokeBlack : Sprite, IFreeable, IId
    {
        public const string ID = "chapter2/McHeroSmokeBlack";

        public string Id => "chapter2/McHeroSmokeBlack";

        public static McHeroSmokeBlack New()
        {
            McHeroSmokeBlack mcHeroSmokeBlack = StaticPool.New<McHeroSmokeBlack>();
            mcHeroSmokeBlack.RefreshProperties();
            return mcHeroSmokeBlack;
        }

        public McHeroSmokeBlack()
            : base("chapter2/McHeroSmokeBlack")
        {
        }

        public void Free()
        {
            StaticPool.Free<McHeroSmokeBlack>(this);
        }
    }
}
