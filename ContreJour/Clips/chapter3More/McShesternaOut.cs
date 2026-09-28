using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter3More;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McShesternaOut : Sprite, IFreeable, IId
{
    public const string ID = "chapter3More/McShesternaOut";

    public string Id => "chapter3More/McShesternaOut";

    public static McShesternaOut New()
    {
        McShesternaOut mcShesternaOut = StaticPool<McShesternaOut>.New();
        mcShesternaOut.RefreshProperties();
        return mcShesternaOut;
    }

    public McShesternaOut()
        : base("chapter3More/McShesternaOut")
    {
    }

    public void Free()
    {
        StaticPool<McShesternaOut>.Free(this);
    }
}
