using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McBackgroundContent2_3 : Sprite, IFreeable, IId
    {
        public const string ID = "chapter2/McBackgroundContent2_3";

        public string Id => "chapter2/McBackgroundContent2_3";

        public static McBackgroundContent2_3 New()
        {
            McBackgroundContent2_3 mcBackgroundContent2_ = StaticPool.New<McBackgroundContent2_3>();
            mcBackgroundContent2_.RefreshProperties();
            return mcBackgroundContent2_;
        }

        public McBackgroundContent2_3()
            : base("chapter2/McBackgroundContent2_3")
        {
        }

        public void Free()
        {
            StaticPool.Free<McBackgroundContent2_3>(this);
        }
    }
}
