using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter3More
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McShesterna2Background : Sprite, IFreeable, IId
    {
        public const string ID = "chapter3More/McShesterna2Background";

        public string Id => "chapter3More/McShesterna2Background";

        public static McShesterna2Background New()
        {
            McShesterna2Background mcShesterna2Background = StaticPool.New<McShesterna2Background>();
            mcShesterna2Background.RefreshProperties();
            return mcShesterna2Background;
        }

        public McShesterna2Background()
            : base("chapter3More/McShesterna2Background")
        {
        }

        public void Free()
        {
            StaticPool.Free<McShesterna2Background>(this);
        }
    }
}
