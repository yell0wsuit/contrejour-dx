using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common2
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McEyeBlack : Sprite, IFreeable, IId
    {
        public const string ID = "common2/McEyeBlack";

        public string Id => "common2/McEyeBlack";

        public static McEyeBlack New()
        {
            McEyeBlack mcEyeBlack = StaticPool.New<McEyeBlack>();
            mcEyeBlack.RefreshProperties();
            return mcEyeBlack;
        }

        public McEyeBlack()
            : base("common2/McEyeBlack")
        {
        }

        public void Free()
        {
            StaticPool.Free<McEyeBlack>(this);
        }
    }
}
