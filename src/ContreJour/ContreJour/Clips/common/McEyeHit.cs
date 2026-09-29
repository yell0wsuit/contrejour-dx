using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McEyeHit : MovieClip, IFreeable, IId
    {
        public const string ID = "common/McEyeHit";

        public string Id => "common/McEyeHit";

        public static McEyeHit New()
        {
            McEyeHit mcEyeHit = StaticPool.New<McEyeHit>();
            mcEyeHit.RefreshProperties();
            return mcEyeHit;
        }

        public McEyeHit()
            : base("common/McEyeHit")
        {
        }

        public void Free()
        {
            StaticPool.Free<McEyeHit>(this);
        }
    }
}
