using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McTailTextureWhite : Sprite, IFreeable, IId
    {
        public const string ID = "chapter4/McTailTextureWhite";

        public string Id => "chapter4/McTailTextureWhite";

        public static McTailTextureWhite New()
        {
            McTailTextureWhite mcTailTextureWhite = StaticPool.New<McTailTextureWhite>();
            mcTailTextureWhite.RefreshProperties();
            return mcTailTextureWhite;
        }

        public McTailTextureWhite()
            : base("chapter4/McTailTextureWhite")
        {
        }

        public void Free()
        {
            StaticPool.Free<McTailTextureWhite>(this);
        }
    }
}
