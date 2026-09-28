using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McTubusBody : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McTubusBody";

    public string Id => "chapter5/McTubusBody";

    public static McTubusBody New()
    {
        McTubusBody mcTubusBody = StaticPool<McTubusBody>.New();
        mcTubusBody.RefreshProperties();
        return mcTubusBody;
    }

    public McTubusBody()
        : base("chapter5/McTubusBody")
    {
    }

    public void Free()
    {
        StaticPool<McTubusBody>.Free(this);
    }
}
