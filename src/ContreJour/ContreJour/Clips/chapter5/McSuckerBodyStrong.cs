using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McSuckerBodyStrong : Sprite, IFreeable, IId
    {
        public const string ID = "chapter5/McSuckerBodyStrong";

        public string Id => "chapter5/McSuckerBodyStrong";

        public static McSuckerBodyStrong New()
        {
            McSuckerBodyStrong mcSuckerBodyStrong = StaticPool.New<McSuckerBodyStrong>();
            mcSuckerBodyStrong.RefreshProperties();
            return mcSuckerBodyStrong;
        }

        public McSuckerBodyStrong()
            : base("chapter5/McSuckerBodyStrong")
        {
        }

        public void Free()
        {
            StaticPool.Free<McSuckerBodyStrong>(this);
        }
    }
}
