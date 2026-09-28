using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeBall : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McEyeBall";

    public string Id => "chapter5/McEyeBall";

    public static McEyeBall New()
    {
        McEyeBall mcEyeBall = StaticPool<McEyeBall>.New();
        mcEyeBall.RefreshProperties();
        return mcEyeBall;
    }

    public McEyeBall()
        : base("chapter5/McEyeBall")
    {
    }

    public void Free()
    {
        StaticPool<McEyeBall>.Free(this);
    }
}
