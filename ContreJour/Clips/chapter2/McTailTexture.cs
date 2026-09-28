using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McTailTexture : Sprite, IFreeable, IId
{
    public const string ID = "chapter2/McTailTexture";

    public string Id => "chapter2/McTailTexture";

    public static McTailTexture New()
    {
        McTailTexture mcTailTexture = StaticPool<McTailTexture>.New();
        mcTailTexture.RefreshProperties();
        return mcTailTexture;
    }

    public McTailTexture()
        : base("chapter2/McTailTexture")
    {
    }

    public void Free()
    {
        StaticPool<McTailTexture>.Free(this);
    }
}
