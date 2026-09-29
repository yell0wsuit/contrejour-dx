using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McEyeBallBlack : Sprite, IFreeable, IId
    {
        public const string ID = "chapter2/McEyeBallBlack";

        public string Id => "chapter2/McEyeBallBlack";

        public static McEyeBallBlack New()
        {
            McEyeBallBlack mcEyeBallBlack = StaticPool.New<McEyeBallBlack>();
            mcEyeBallBlack.RefreshProperties();
            return mcEyeBallBlack;
        }

        public McEyeBallBlack()
            : base("chapter2/McEyeBallBlack")
        {
        }

        public void Free()
        {
            StaticPool.Free<McEyeBallBlack>(this);
        }
    }
}
