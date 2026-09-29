using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McIntroLogo : Sprite, IFreeable, IId
    {
        public const string ID = "level1/McIntroLogo";

        public string Id => "level1/McIntroLogo";

        public static McIntroLogo New()
        {
            McIntroLogo mcIntroLogo = StaticPool.New<McIntroLogo>();
            mcIntroLogo.RefreshProperties();
            return mcIntroLogo;
        }

        public McIntroLogo()
            : base("level1/McIntroLogo")
        {
        }

        public void Free()
        {
            StaticPool.Free<McIntroLogo>(this);
        }
    }
}
