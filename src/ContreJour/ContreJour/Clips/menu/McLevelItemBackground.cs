using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McLevelItemBackground : Sprite, IFreeable, IId
    {
        public const string ID = "menu/McLevelItemBackground";

        public string Id => "menu/McLevelItemBackground";

        public static McLevelItemBackground New()
        {
            McLevelItemBackground mcLevelItemBackground = StaticPool.New<McLevelItemBackground>();
            mcLevelItemBackground.RefreshProperties();
            return mcLevelItemBackground;
        }

        public McLevelItemBackground()
            : base("menu/McLevelItemBackground")
        {
        }

        public void Free()
        {
            StaticPool.Free<McLevelItemBackground>(this);
        }
    }
}
