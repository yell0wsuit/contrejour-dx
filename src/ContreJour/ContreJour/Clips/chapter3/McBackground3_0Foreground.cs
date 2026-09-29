using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter3
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McBackground3_0Foreground : Sprite, IFreeable, IId
    {
        public const string ID = "chapter3/McBackground3_0Foreground";

        public string Id => "chapter3/McBackground3_0Foreground";

        public static McBackground3_0Foreground New()
        {
            McBackground3_0Foreground mcBackground3_0Foreground = StaticPool.New<McBackground3_0Foreground>();
            mcBackground3_0Foreground.RefreshProperties();
            return mcBackground3_0Foreground;
        }

        public McBackground3_0Foreground()
            : base("chapter3/McBackground3_0Foreground")
        {
        }

        public void Free()
        {
            StaticPool.Free<McBackground3_0Foreground>(this);
        }
    }
}
