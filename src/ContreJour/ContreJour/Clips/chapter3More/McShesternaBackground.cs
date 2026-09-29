using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter3More
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McShesternaBackground : Sprite, IFreeable, IId
    {
        public const string ID = "chapter3More/McShesternaBackground";

        public string Id => "chapter3More/McShesternaBackground";

        public static McShesternaBackground New()
        {
            McShesternaBackground mcShesternaBackground = StaticPool.New<McShesternaBackground>();
            mcShesternaBackground.RefreshProperties();
            return mcShesternaBackground;
        }

        public McShesternaBackground()
            : base("chapter3More/McShesternaBackground")
        {
        }

        public void Free()
        {
            StaticPool.Free<McShesternaBackground>(this);
        }
    }
}
