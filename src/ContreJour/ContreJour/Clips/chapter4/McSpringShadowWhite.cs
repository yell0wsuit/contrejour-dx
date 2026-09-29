using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McSpringShadowWhite : Sprite, IFreeable, IId
    {
        public const string ID = "chapter4/McSpringShadowWhite";

        public string Id => "chapter4/McSpringShadowWhite";

        public static McSpringShadowWhite New()
        {
            McSpringShadowWhite mcSpringShadowWhite = StaticPool.New<McSpringShadowWhite>();
            mcSpringShadowWhite.RefreshProperties();
            return mcSpringShadowWhite;
        }

        public McSpringShadowWhite()
            : base("chapter4/McSpringShadowWhite")
        {
        }

        public void Free()
        {
            StaticPool.Free<McSpringShadowWhite>(this);
        }
    }
}
