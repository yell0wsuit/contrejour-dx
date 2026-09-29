using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McSpringViewBlack : MovieClip, IFreeable, IId
    {
        public const string ID = "chapter2/McSpringViewBlack";

        public string Id => "chapter2/McSpringViewBlack";

        public static McSpringViewBlack New()
        {
            McSpringViewBlack mcSpringViewBlack = StaticPool.New<McSpringViewBlack>();
            mcSpringViewBlack.RefreshProperties();
            return mcSpringViewBlack;
        }

        public McSpringViewBlack()
            : base("chapter2/McSpringViewBlack")
        {
        }

        public void Free()
        {
            StaticPool.Free<McSpringViewBlack>(this);
        }
    }
}
