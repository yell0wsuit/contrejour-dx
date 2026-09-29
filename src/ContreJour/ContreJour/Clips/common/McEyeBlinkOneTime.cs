using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McEyeBlinkOneTime : MovieClip, IFreeable, IId
    {
        public const string ID = "common/McEyeBlinkOneTime";

        public string Id => "common/McEyeBlinkOneTime";

        public static McEyeBlinkOneTime New()
        {
            McEyeBlinkOneTime mcEyeBlinkOneTime = StaticPool.New<McEyeBlinkOneTime>();
            mcEyeBlinkOneTime.RefreshProperties();
            return mcEyeBlinkOneTime;
        }

        public McEyeBlinkOneTime()
            : base("common/McEyeBlinkOneTime")
        {
        }

        public void Free()
        {
            StaticPool.Free<McEyeBlinkOneTime>(this);
        }
    }
}
