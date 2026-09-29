using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McSpikesPart : MovieClip, IFreeable, IId
    {
        public const string ID = "common/McSpikesPart";

        public string Id => "common/McSpikesPart";

        public static McSpikesPart New()
        {
            McSpikesPart mcSpikesPart = StaticPool.New<McSpikesPart>();
            mcSpikesPart.RefreshProperties();
            return mcSpikesPart;
        }

        public McSpikesPart()
            : base("common/McSpikesPart")
        {
        }

        public void Free()
        {
            StaticPool.Free<McSpikesPart>(this);
        }
    }
}
