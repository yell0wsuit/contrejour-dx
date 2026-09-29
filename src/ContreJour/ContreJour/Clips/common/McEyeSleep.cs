using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McEyeSleep : MovieClip, IFreeable, IId
    {
        public const string ID = "common/McEyeSleep";

        public string Id => "common/McEyeSleep";

        public static McEyeSleep New()
        {
            McEyeSleep mcEyeSleep = StaticPool.New<McEyeSleep>();
            mcEyeSleep.RefreshProperties();
            return mcEyeSleep;
        }

        public McEyeSleep()
            : base("common/McEyeSleep")
        {
        }

        public void Free()
        {
            StaticPool.Free<McEyeSleep>(this);
        }
    }
}
