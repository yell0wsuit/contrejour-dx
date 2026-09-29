using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common2
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McEyeBlinkMonsterBlack : MovieClip, IFreeable, IId
    {
        public const string ID = "common2/McEyeBlinkMonsterBlack";

        public string Id => "common2/McEyeBlinkMonsterBlack";

        public static McEyeBlinkMonsterBlack New()
        {
            McEyeBlinkMonsterBlack mcEyeBlinkMonsterBlack = StaticPool.New<McEyeBlinkMonsterBlack>();
            mcEyeBlinkMonsterBlack.RefreshProperties();
            return mcEyeBlinkMonsterBlack;
        }

        public McEyeBlinkMonsterBlack()
            : base("common2/McEyeBlinkMonsterBlack")
        {
        }

        public void Free()
        {
            StaticPool.Free<McEyeBlinkMonsterBlack>(this);
        }
    }
}
