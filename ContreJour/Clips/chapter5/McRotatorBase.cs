using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRotatorBase : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McRotatorBase";

    public string Id => "chapter5/McRotatorBase";

    public static McRotatorBase New()
    {
        McRotatorBase mcRotatorBase = StaticPool<McRotatorBase>.New();
        mcRotatorBase.RefreshProperties();
        return mcRotatorBase;
    }

    public McRotatorBase()
        : base("chapter5/McRotatorBase")
    {
    }

    public void Free()
    {
        StaticPool<McRotatorBase>.Free(this);
    }
}
