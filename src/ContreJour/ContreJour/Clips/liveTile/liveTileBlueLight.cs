using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.liveTile
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class liveTileBlueLight : Sprite, IFreeable, IId
    {
        public const string ID = "liveTile/liveTileBlueLight";

        public string Id => "liveTile/liveTileBlueLight";

        public static liveTileBlueLight New()
        {
            liveTileBlueLight liveTileBlueLight2 = StaticPool.New<liveTileBlueLight>();
            liveTileBlueLight2.RefreshProperties();
            return liveTileBlueLight2;
        }

        public liveTileBlueLight()
            : base("liveTile/liveTileBlueLight")
        {
        }

        public void Free()
        {
            StaticPool.Free<liveTileBlueLight>(this);
        }
    }
}
