using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McSnowParticle : Sprite, IFreeable, IId
    {
        public const string ID = "common/McSnowParticle";

        public string Id => "common/McSnowParticle";

        public static McSnowParticle New()
        {
            McSnowParticle mcSnowParticle = StaticPool.New<McSnowParticle>();
            mcSnowParticle.RefreshProperties();
            return mcSnowParticle;
        }

        public McSnowParticle()
            : base("common/McSnowParticle")
        {
        }

        public void Free()
        {
            StaticPool.Free<McSnowParticle>(this);
        }
    }
}
