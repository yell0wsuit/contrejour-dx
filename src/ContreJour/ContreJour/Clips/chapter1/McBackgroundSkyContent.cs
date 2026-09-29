using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McBackgroundSkyContent : Sprite, IFreeable, IId
    {
        public const string ID = "chapter1/McBackgroundSkyContent";

        public string Id => "chapter1/McBackgroundSkyContent";

        public static McBackgroundSkyContent New()
        {
            McBackgroundSkyContent mcBackgroundSkyContent = StaticPool.New<McBackgroundSkyContent>();
            mcBackgroundSkyContent.RefreshProperties();
            return mcBackgroundSkyContent;
        }

        public McBackgroundSkyContent()
            : base("chapter1/McBackgroundSkyContent")
        {
        }

        public void Free()
        {
            StaticPool.Free<McBackgroundSkyContent>(this);
        }
    }
}
