using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McRotatorGrass1 : Sprite, IFreeable, IId
    {
        public const string ID = "chapter5/McRotatorGrass1";

        public string Id => "chapter5/McRotatorGrass1";

        public static McRotatorGrass1 New()
        {
            McRotatorGrass1 mcRotatorGrass = StaticPool.New<McRotatorGrass1>();
            mcRotatorGrass.RefreshProperties();
            return mcRotatorGrass;
        }

        public McRotatorGrass1()
            : base("chapter5/McRotatorGrass1")
        {
        }

        public void Free()
        {
            StaticPool.Free<McRotatorGrass1>(this);
        }
    }
}
