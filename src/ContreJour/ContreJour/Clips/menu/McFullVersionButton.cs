using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McFullVersionButton : Sprite, IFreeable, IId
    {
        public const string ID = "menu/McFullVersionButton";

        public string Id => "menu/McFullVersionButton";

        public static McFullVersionButton New()
        {
            McFullVersionButton mcFullVersionButton = StaticPool.New<McFullVersionButton>();
            mcFullVersionButton.RefreshProperties();
            return mcFullVersionButton;
        }

        public McFullVersionButton()
            : base("menu/McFullVersionButton")
        {
        }

        public void Free()
        {
            StaticPool.Free<McFullVersionButton>(this);
        }
    }
}
