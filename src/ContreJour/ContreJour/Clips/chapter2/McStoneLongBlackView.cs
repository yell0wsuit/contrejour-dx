using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McStoneLongBlackView : Sprite, IFreeable, IId
    {
        public const string ID = "chapter2/McStoneLongBlackView";

        public string Id => "chapter2/McStoneLongBlackView";

        public static McStoneLongBlackView New()
        {
            McStoneLongBlackView mcStoneLongBlackView = StaticPool.New<McStoneLongBlackView>();
            mcStoneLongBlackView.RefreshProperties();
            return mcStoneLongBlackView;
        }

        public McStoneLongBlackView()
            : base("chapter2/McStoneLongBlackView")
        {
        }

        public void Free()
        {
            StaticPool.Free<McStoneLongBlackView>(this);
        }
    }
}
