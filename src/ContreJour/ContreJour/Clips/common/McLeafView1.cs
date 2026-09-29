using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McLeafView1 : Sprite, IFreeable, IId
    {
        public const string ID = "common/McLeafView1";

        public string Id => "common/McLeafView1";

        public static McLeafView1 New()
        {
            McLeafView1 mcLeafView = StaticPool.New<McLeafView1>();
            mcLeafView.RefreshProperties();
            return mcLeafView;
        }

        public McLeafView1()
            : base("common/McLeafView1")
        {
        }

        public void Free()
        {
            StaticPool.Free<McLeafView1>(this);
        }
    }
}
