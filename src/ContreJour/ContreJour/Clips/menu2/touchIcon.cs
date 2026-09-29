using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu2
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class touchIcon : Sprite, IFreeable, IId
    {
        public const string ID = "menu2/touchIcon";

        public string Id => "menu2/touchIcon";

        public static touchIcon New()
        {
            touchIcon touchIcon2 = StaticPool.New<touchIcon>();
            touchIcon2.RefreshProperties();
            return touchIcon2;
        }

        public touchIcon()
            : base("menu2/touchIcon")
        {
        }

        public void Free()
        {
            StaticPool.Free<touchIcon>(this);
        }
    }
}
