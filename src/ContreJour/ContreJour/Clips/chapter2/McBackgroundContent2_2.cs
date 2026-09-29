using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McBackgroundContent2_2 : Sprite, IFreeable, IId
    {
        public const string ID = "chapter2/McBackgroundContent2_2";

        public string Id => "chapter2/McBackgroundContent2_2";

        public static McBackgroundContent2_2 New()
        {
            McBackgroundContent2_2 mcBackgroundContent2_ = StaticPool.New<McBackgroundContent2_2>();
            mcBackgroundContent2_.RefreshProperties();
            return mcBackgroundContent2_;
        }

        public McBackgroundContent2_2()
            : base("chapter2/McBackgroundContent2_2")
        {
        }

        public void Free()
        {
            StaticPool.Free<McBackgroundContent2_2>(this);
        }
    }
}
