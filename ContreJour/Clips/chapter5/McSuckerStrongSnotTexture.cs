using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSuckerStrongSnotTexture : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McSuckerStrongSnotTexture";

    public string Id => "chapter5/McSuckerStrongSnotTexture";

    public static McSuckerStrongSnotTexture New()
    {
        McSuckerStrongSnotTexture mcSuckerStrongSnotTexture = StaticPool<McSuckerStrongSnotTexture>.New();
        mcSuckerStrongSnotTexture.RefreshProperties();
        return mcSuckerStrongSnotTexture;
    }

    public McSuckerStrongSnotTexture()
        : base("chapter5/McSuckerStrongSnotTexture")
    {
    }

    public void Free()
    {
        StaticPool<McSuckerStrongSnotTexture>.Free(this);
    }
}
