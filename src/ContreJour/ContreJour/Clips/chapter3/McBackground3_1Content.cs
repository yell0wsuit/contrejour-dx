using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter3
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McBackground3_1Content : Sprite, IFreeable, IId
    {
        public const string ID = "chapter3/McBackground3_1Content";

        public string Id => "chapter3/McBackground3_1Content";

        public static McBackground3_1Content New()
        {
            McBackground3_1Content mcBackground3_1Content = StaticPool.New<McBackground3_1Content>();
            mcBackground3_1Content.RefreshProperties();
            return mcBackground3_1Content;
        }

        public McBackground3_1Content()
            : base("chapter3/McBackground3_1Content")
        {
        }

        public void Free()
        {
            StaticPool.Free<McBackground3_1Content>(this);
        }
    }
}
