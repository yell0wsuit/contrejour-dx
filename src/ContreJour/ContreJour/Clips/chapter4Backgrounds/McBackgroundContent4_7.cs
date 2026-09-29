using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4Backgrounds
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McBackgroundContent4_7 : Sprite, IFreeable, IId
    {
        public const string ID = "chapter4Backgrounds/McBackgroundContent4_7";

        public string Id => "chapter4Backgrounds/McBackgroundContent4_7";

        public static McBackgroundContent4_7 New()
        {
            McBackgroundContent4_7 mcBackgroundContent4_ = StaticPool.New<McBackgroundContent4_7>();
            mcBackgroundContent4_.RefreshProperties();
            return mcBackgroundContent4_;
        }

        public McBackgroundContent4_7()
            : base("chapter4Backgrounds/McBackgroundContent4_7")
        {
        }

        public void Free()
        {
            StaticPool.Free<McBackgroundContent4_7>(this);
        }
    }
}
