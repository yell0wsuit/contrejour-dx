using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McBaloonTailEnd : AnimationNode, IFreeable, IId
    {
        public const string ID = "chapter5/McBaloonTailEnd";

        public McRotatorGrass0 instance5244474 { get; protected set; }

        public string Id => "chapter5/McBaloonTailEnd";

        public static McBaloonTailEnd New()
        {
            McBaloonTailEnd mcBaloonTailEnd = StaticPool.New<McBaloonTailEnd>();
            mcBaloonTailEnd.RefreshProperties();
            return mcBaloonTailEnd;
        }

        public McBaloonTailEnd()
            : base("chapter5/McBaloonTailEnd")
        {
            instance5244474 = new McRotatorGrass0();
            AddChild("instance5244474", instance5244474);
            Initialize();
        }

        public void Free()
        {
            StaticPool.Free<McBaloonTailEnd>(this);
        }
    }
}
