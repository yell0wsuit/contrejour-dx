using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McEyeBallHit : MovieClip, IFreeable, IId
    {
        public const string ID = "common/McEyeBallHit";

        public string Id => "common/McEyeBallHit";

        public static McEyeBallHit New()
        {
            McEyeBallHit mcEyeBallHit = StaticPool.New<McEyeBallHit>();
            mcEyeBallHit.RefreshProperties();
            return mcEyeBallHit;
        }

        public McEyeBallHit()
            : base("common/McEyeBallHit")
        {
        }

        public void Free()
        {
            StaticPool.Free<McEyeBallHit>(this);
        }
    }
}
