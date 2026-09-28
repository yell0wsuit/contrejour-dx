using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeDead : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McEyeDead";

    public string Id => "chapter5/McEyeDead";

    public static McEyeDead New()
    {
        McEyeDead mcEyeDead = StaticPool<McEyeDead>.New();
        mcEyeDead.RefreshProperties();
        return mcEyeDead;
    }

    public McEyeDead()
        : base("chapter5/McEyeDead")
    {
    }

    public void Free()
    {
        StaticPool<McEyeDead>.Free(this);
    }
}
