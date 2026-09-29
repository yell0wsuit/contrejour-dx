using System.CodeDom.Compiler;

using ContreJour.WinRT;

using Microsoft.Xna.Framework;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual.Text;

namespace ContreJour.Clips.liveTile
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class liveTileWide : LiveTileAnimation, IFreeable, IId
    {
        public const string ID = "liveTile/liveTileWide";

        public blackSquare instance652 { get; protected set; }

        public liveTileGraphics_358x173 background { get; protected set; }

        public liveTileBlueLight blueLight { get; protected set; }

        public liveTileHighLight highLight { get; protected set; }

        public Label count { get; protected set; }

        public string Id => "liveTile/liveTileWide";

        public static liveTileWide New()
        {
            liveTileWide liveTileWide2 = StaticPool.New<liveTileWide>();
            liveTileWide2.RefreshProperties();
            return liveTileWide2;
        }

        public liveTileWide()
            : base("liveTile/liveTileWide")
        {
            instance652 = new blackSquare();
            AddChild("instance652", instance652);
            background = new liveTileGraphics_358x173();
            AddChild("background", background);
            blueLight = new liveTileBlueLight();
            AddChild("blueLight", blueLight);
            highLight = new liveTileHighLight();
            AddChild("highLight", highLight);
            count = new Label("Segoe Print", 24f, new Vector2(72f, 45.85f))
            {
                TextString = "5",
                LineSpacing = 2f,
                Align = TextAlign.Right
            };
            AddChild("count", count);
            Initialize();
        }

        public void Free()
        {
            StaticPool.Free<liveTileWide>(this);
        }
    }
}
