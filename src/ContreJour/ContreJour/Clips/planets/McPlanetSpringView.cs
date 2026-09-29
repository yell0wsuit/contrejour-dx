using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McPlanetSpringView : Sprite, IFreeable, IId
    {
        public const string ID = "planets/McPlanetSpringView";

        public string Id => "planets/McPlanetSpringView";

        public static McPlanetSpringView New()
        {
            McPlanetSpringView mcPlanetSpringView = StaticPool.New<McPlanetSpringView>();
            mcPlanetSpringView.RefreshProperties();
            return mcPlanetSpringView;
        }

        public McPlanetSpringView()
            : base("planets/McPlanetSpringView")
        {
        }

        public void Free()
        {
            StaticPool.Free<McPlanetSpringView>(this);
        }
    }
}
