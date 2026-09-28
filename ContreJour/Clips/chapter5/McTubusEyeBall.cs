using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McTubusEyeBall : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McTubusEyeBall";

    public string Id => "chapter5/McTubusEyeBall";

    public static McTubusEyeBall New()
    {
        McTubusEyeBall mcTubusEyeBall = StaticPool<McTubusEyeBall>.New();
        mcTubusEyeBall.RefreshProperties();
        return mcTubusEyeBall;
    }

    public McTubusEyeBall()
        : base("chapter5/McTubusEyeBall")
    {
    }

    public void Free()
    {
        StaticPool<McTubusEyeBall>.Free(this);
    }
}
