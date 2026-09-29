using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McRoseForeground : Sprite, IFreeable, IId
    {
        public const string ID = "menu/McRoseForeground";

        public string Id => "menu/McRoseForeground";

        public static McRoseForeground New()
        {
            McRoseForeground mcRoseForeground = StaticPool.New<McRoseForeground>();
            mcRoseForeground.RefreshProperties();
            return mcRoseForeground;
        }

        public McRoseForeground()
            : base("menu/McRoseForeground")
        {
        }

        public void Free()
        {
            StaticPool.Free<McRoseForeground>(this);
        }
    }
}
