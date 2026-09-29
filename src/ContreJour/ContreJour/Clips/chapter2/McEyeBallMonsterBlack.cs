using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McEyeBallMonsterBlack : Sprite, IFreeable, IId
    {
        public const string ID = "chapter2/McEyeBallMonsterBlack";

        public string Id => "chapter2/McEyeBallMonsterBlack";

        public static McEyeBallMonsterBlack New()
        {
            McEyeBallMonsterBlack mcEyeBallMonsterBlack = StaticPool.New<McEyeBallMonsterBlack>();
            mcEyeBallMonsterBlack.RefreshProperties();
            return mcEyeBallMonsterBlack;
        }

        public McEyeBallMonsterBlack()
            : base("chapter2/McEyeBallMonsterBlack")
        {
        }

        public void Free()
        {
            StaticPool.Free<McEyeBallMonsterBlack>(this);
        }
    }
}
