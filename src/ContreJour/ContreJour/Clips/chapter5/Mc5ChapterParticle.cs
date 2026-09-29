using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class Mc5ChapterParticle : Sprite, IFreeable, IId
    {
        public const string ID = "chapter5/Mc5ChapterParticle";

        public string Id => "chapter5/Mc5ChapterParticle";

        public static Mc5ChapterParticle New()
        {
            Mc5ChapterParticle mc5ChapterParticle = StaticPool.New<Mc5ChapterParticle>();
            mc5ChapterParticle.RefreshProperties();
            return mc5ChapterParticle;
        }

        public Mc5ChapterParticle()
            : base("chapter5/Mc5ChapterParticle")
        {
        }

        public void Free()
        {
            StaticPool.Free<Mc5ChapterParticle>(this);
        }
    }
}
