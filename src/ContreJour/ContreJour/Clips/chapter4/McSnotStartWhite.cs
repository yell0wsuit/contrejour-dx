using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McSnotStartWhite : Sprite, IFreeable, IId
    {
        public const string ID = "chapter4/McSnotStartWhite";

        public string Id => "chapter4/McSnotStartWhite";

        public static McSnotStartWhite New()
        {
            McSnotStartWhite mcSnotStartWhite = StaticPool.New<McSnotStartWhite>();
            mcSnotStartWhite.RefreshProperties();
            return mcSnotStartWhite;
        }

        public McSnotStartWhite()
            : base("chapter4/McSnotStartWhite")
        {
        }

        public void Free()
        {
            StaticPool.Free<McSnotStartWhite>(this);
        }
    }
}
