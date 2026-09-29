using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McTailTexture : Sprite, IFreeable, IId
    {
        public const string ID = "common/McTailTexture";

        public string Id => "common/McTailTexture";

        public static McTailTexture New()
        {
            McTailTexture mcTailTexture = StaticPool.New<McTailTexture>();
            mcTailTexture.RefreshProperties();
            return mcTailTexture;
        }

        public McTailTexture()
            : base("common/McTailTexture")
        {
        }

        public void Free()
        {
            StaticPool.Free<McTailTexture>(this);
        }
    }
}
