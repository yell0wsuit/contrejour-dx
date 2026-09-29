using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu2
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class petitInformationBackground : Sprite, IFreeable, IId
    {
        public const string ID = "menu2/petitInformationBackground";

        public string Id => "menu2/petitInformationBackground";

        public static petitInformationBackground New()
        {
            petitInformationBackground petitInformationBackground2 = StaticPool.New<petitInformationBackground>();
            petitInformationBackground2.RefreshProperties();
            return petitInformationBackground2;
        }

        public petitInformationBackground()
            : base("menu2/petitInformationBackground")
        {
        }

        public void Free()
        {
            StaticPool.Free<petitInformationBackground>(this);
        }
    }
}
