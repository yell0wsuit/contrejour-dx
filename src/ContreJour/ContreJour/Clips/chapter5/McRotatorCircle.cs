using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McRotatorCircle : Sprite, IFreeable, IId
    {
        public const string ID = "chapter5/McRotatorCircle";

        public string Id => "chapter5/McRotatorCircle";

        public static McRotatorCircle New()
        {
            McRotatorCircle mcRotatorCircle = StaticPool.New<McRotatorCircle>();
            mcRotatorCircle.RefreshProperties();
            return mcRotatorCircle;
        }

        public McRotatorCircle()
            : base("chapter5/McRotatorCircle")
        {
        }

        public void Free()
        {
            StaticPool.Free<McRotatorCircle>(this);
        }
    }
}
