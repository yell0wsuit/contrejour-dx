using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter3More
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McStoneLongView : Sprite, IFreeable, IId
    {
        public const string ID = "chapter3More/McStoneLongView";

        public string Id => "chapter3More/McStoneLongView";

        public static McStoneLongView New()
        {
            McStoneLongView mcStoneLongView = StaticPool.New<McStoneLongView>();
            mcStoneLongView.RefreshProperties();
            return mcStoneLongView;
        }

        public McStoneLongView()
            : base("chapter3More/McStoneLongView")
        {
        }

        public void Free()
        {
            StaticPool.Free<McStoneLongView>(this);
        }
    }
}
