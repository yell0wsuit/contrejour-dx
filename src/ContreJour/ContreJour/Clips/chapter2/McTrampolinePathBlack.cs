using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McTrampolinePathBlack : Sprite, IFreeable, IId
    {
        public const string ID = "chapter2/McTrampolinePathBlack";

        public string Id => "chapter2/McTrampolinePathBlack";

        public static McTrampolinePathBlack New()
        {
            McTrampolinePathBlack mcTrampolinePathBlack = StaticPool.New<McTrampolinePathBlack>();
            mcTrampolinePathBlack.RefreshProperties();
            return mcTrampolinePathBlack;
        }

        public McTrampolinePathBlack()
            : base("chapter2/McTrampolinePathBlack")
        {
        }

        public void Free()
        {
            StaticPool.Free<McTrampolinePathBlack>(this);
        }
    }
}
