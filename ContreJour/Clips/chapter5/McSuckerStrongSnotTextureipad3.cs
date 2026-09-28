using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSuckerStrongSnotTextureipad3 : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McSuckerStrongSnotTextureipad3";

    public string Id => "chapter5/McSuckerStrongSnotTextureipad3";

    public static McSuckerStrongSnotTextureipad3 New()
    {
        McSuckerStrongSnotTextureipad3 mcSuckerStrongSnotTextureipad = StaticPool<McSuckerStrongSnotTextureipad3>.New();
        mcSuckerStrongSnotTextureipad.RefreshProperties();
        return mcSuckerStrongSnotTextureipad;
    }

    public McSuckerStrongSnotTextureipad3()
        : base("chapter5/McSuckerStrongSnotTextureipad3")
    {
    }

    public void Free()
    {
        StaticPool<McSuckerStrongSnotTextureipad3>.Free(this);
    }
}
