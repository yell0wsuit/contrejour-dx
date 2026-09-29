using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McLevels3 : Sprite, IFreeable, IId
    {
        public const string ID = "menu/McLevels3";

        public string Id => "menu/McLevels3";

        public static McLevels3 New()
        {
            McLevels3 mcLevels = StaticPool.New<McLevels3>();
            mcLevels.RefreshProperties();
            return mcLevels;
        }

        public McLevels3()
            : base("menu/McLevels3")
        {
        }

        public void Free()
        {
            StaticPool.Free<McLevels3>(this);
        }
    }
}
