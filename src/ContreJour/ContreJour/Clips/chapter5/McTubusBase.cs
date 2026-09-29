using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McTubusBase : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McTubusBase";

    public string Id => "chapter5/McTubusBase";

    public static McTubusBase New()
    {
        McTubusBase mcTubusBase = StaticPool.New<McTubusBase>();
        mcTubusBase.RefreshProperties();
        return mcTubusBase;
    }

    public McTubusBase()
        : base("chapter5/McTubusBase")
    {
    }

    public void Free()
    {
        StaticPool.Free<McTubusBase>(this);
    }
}
