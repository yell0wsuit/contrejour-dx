using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.liveTile
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class liveTileGraphics_358x173 : Sprite, IFreeable, IId
    {
        public const string ID = "liveTile/liveTileGraphics_358x173";

        public string Id => "liveTile/liveTileGraphics_358x173";

        public static liveTileGraphics_358x173 New()
        {
            liveTileGraphics_358x173 liveTileGraphics_358x359 = StaticPool.New<liveTileGraphics_358x173>();
            liveTileGraphics_358x359.RefreshProperties();
            return liveTileGraphics_358x359;
        }

        public liveTileGraphics_358x173()
            : base("liveTile/liveTileGraphics_358x173")
        {
        }

        public void Free()
        {
            StaticPool.Free<liveTileGraphics_358x173>(this);
        }
    }
}
