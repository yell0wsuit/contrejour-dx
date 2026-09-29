using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.lights
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McLightView12 : AnimationNode, IFreeable, IId
    {
        public const string ID = "lights/McLightView12";

        public McLightView11ContentExport instance5230352 { get; protected set; }

        public string Id => "lights/McLightView12";

        public static McLightView12 New()
        {
            McLightView12 mcLightView = StaticPool.New<McLightView12>();
            mcLightView.RefreshProperties();
            return mcLightView;
        }

        public McLightView12()
            : base("lights/McLightView12")
        {
            instance5230352 = new McLightView11ContentExport();
            AddChild("instance5230352", instance5230352);
            Initialize();
        }

        public void Free()
        {
            StaticPool.Free<McLightView12>(this);
        }
    }
}
