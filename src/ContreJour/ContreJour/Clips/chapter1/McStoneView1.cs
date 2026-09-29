using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McStoneView1 : Sprite, IFreeable, IId
    {
        public const string ID = "chapter1/McStoneView1";

        public string Id => "chapter1/McStoneView1";

        public static McStoneView1 New()
        {
            McStoneView1 mcStoneView = StaticPool.New<McStoneView1>();
            mcStoneView.RefreshProperties();
            return mcStoneView;
        }

        public McStoneView1()
            : base("chapter1/McStoneView1")
        {
        }

        public void Free()
        {
            StaticPool.Free<McStoneView1>(this);
        }
    }
}
