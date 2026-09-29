using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McStoneView8 : Sprite, IFreeable, IId
    {
        public const string ID = "chapter2/McStoneView8";

        public string Id => "chapter2/McStoneView8";

        public static McStoneView8 New()
        {
            McStoneView8 mcStoneView = StaticPool.New<McStoneView8>();
            mcStoneView.RefreshProperties();
            return mcStoneView;
        }

        public McStoneView8()
            : base("chapter2/McStoneView8")
        {
        }

        public void Free()
        {
            StaticPool.Free<McStoneView8>(this);
        }
    }
}
