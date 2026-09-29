using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter3More
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McRelsHintView : Sprite, IFreeable, IId
    {
        public const string ID = "chapter3More/McRelsHintView";

        public string Id => "chapter3More/McRelsHintView";

        public static McRelsHintView New()
        {
            McRelsHintView mcRelsHintView = StaticPool.New<McRelsHintView>();
            mcRelsHintView.RefreshProperties();
            return mcRelsHintView;
        }

        public McRelsHintView()
            : base("chapter3More/McRelsHintView")
        {
        }

        public void Free()
        {
            StaticPool.Free<McRelsHintView>(this);
        }
    }
}
