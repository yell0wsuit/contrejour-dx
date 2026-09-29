using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common2
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McEyeSleepBlack : MovieClip, IFreeable, IId
    {
        public const string ID = "common2/McEyeSleepBlack";

        public string Id => "common2/McEyeSleepBlack";

        public static McEyeSleepBlack New()
        {
            McEyeSleepBlack mcEyeSleepBlack = StaticPool.New<McEyeSleepBlack>();
            mcEyeSleepBlack.RefreshProperties();
            return mcEyeSleepBlack;
        }

        public McEyeSleepBlack()
            : base("common2/McEyeSleepBlack")
        {
        }

        public void Free()
        {
            StaticPool.Free<McEyeSleepBlack>(this);
        }
    }
}
