using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McSnotEndWhite : Sprite, IFreeable, IId
    {
        public const string ID = "chapter4/McSnotEndWhite";

        public string Id => "chapter4/McSnotEndWhite";

        public static McSnotEndWhite New()
        {
            McSnotEndWhite mcSnotEndWhite = StaticPool.New<McSnotEndWhite>();
            mcSnotEndWhite.RefreshProperties();
            return mcSnotEndWhite;
        }

        public McSnotEndWhite()
            : base("chapter4/McSnotEndWhite")
        {
        }

        public void Free()
        {
            StaticPool.Free<McSnotEndWhite>(this);
        }
    }
}
