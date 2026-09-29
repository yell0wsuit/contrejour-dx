using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McLevels8 : Sprite, IFreeable, IId
    {
        public const string ID = "menu/McLevels8";

        public string Id => "menu/McLevels8";

        public static McLevels8 New()
        {
            McLevels8 mcLevels = StaticPool.New<McLevels8>();
            mcLevels.RefreshProperties();
            return mcLevels;
        }

        public McLevels8()
            : base("menu/McLevels8")
        {
        }

        public void Free()
        {
            StaticPool.Free<McLevels8>(this);
        }
    }
}
