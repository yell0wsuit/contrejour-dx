using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter3
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McBackground3_0Content : Sprite, IFreeable, IId
    {
        public const string ID = "chapter3/McBackground3_0Content";

        public string Id => "chapter3/McBackground3_0Content";

        public static McBackground3_0Content New()
        {
            McBackground3_0Content mcBackground3_0Content = StaticPool.New<McBackground3_0Content>();
            mcBackground3_0Content.RefreshProperties();
            return mcBackground3_0Content;
        }

        public McBackground3_0Content()
            : base("chapter3/McBackground3_0Content")
        {
        }

        public void Free()
        {
            StaticPool.Free<McBackground3_0Content>(this);
        }
    }
}
