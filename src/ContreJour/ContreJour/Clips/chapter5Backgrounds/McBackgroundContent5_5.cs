using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5Backgrounds
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McBackgroundContent5_5 : Sprite, IFreeable, IId
    {
        public const string ID = "chapter5Backgrounds/McBackgroundContent5_5";

        public string Id => "chapter5Backgrounds/McBackgroundContent5_5";

        public static McBackgroundContent5_5 New()
        {
            McBackgroundContent5_5 mcBackgroundContent5_ = StaticPool.New<McBackgroundContent5_5>();
            mcBackgroundContent5_.RefreshProperties();
            return mcBackgroundContent5_;
        }

        public McBackgroundContent5_5()
            : base("chapter5Backgrounds/McBackgroundContent5_5")
        {
        }

        public void Free()
        {
            StaticPool.Free<McBackgroundContent5_5>(this);
        }
    }
}
