using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common2
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McEyeAngryBlack : MovieClip, IFreeable, IId
    {
        public const string ID = "common2/McEyeAngryBlack";

        public string Id => "common2/McEyeAngryBlack";

        public static McEyeAngryBlack New()
        {
            McEyeAngryBlack mcEyeAngryBlack = StaticPool.New<McEyeAngryBlack>();
            mcEyeAngryBlack.RefreshProperties();
            return mcEyeAngryBlack;
        }

        public McEyeAngryBlack()
            : base("common2/McEyeAngryBlack")
        {
        }

        public void Free()
        {
            StaticPool.Free<McEyeAngryBlack>(this);
        }
    }
}
