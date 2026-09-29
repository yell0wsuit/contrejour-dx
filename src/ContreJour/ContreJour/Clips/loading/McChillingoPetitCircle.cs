using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.loading
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McChillingoPetitCircle : Sprite, IFreeable, IId
    {
        public const string ID = "loading/McChillingoPetitCircle";

        public string Id => "loading/McChillingoPetitCircle";

        public static McChillingoPetitCircle New()
        {
            McChillingoPetitCircle mcChillingoPetitCircle = StaticPool.New<McChillingoPetitCircle>();
            mcChillingoPetitCircle.RefreshProperties();
            return mcChillingoPetitCircle;
        }

        public McChillingoPetitCircle()
            : base("loading/McChillingoPetitCircle")
        {
        }

        public void Free()
        {
            StaticPool.Free<McChillingoPetitCircle>(this);
        }
    }
}
