using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter3More;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McShesternaBackgroundBlur : Sprite, IFreeable, IId
{
    public const string ID = "chapter3More/McShesternaBackgroundBlur";

    public string Id => "chapter3More/McShesternaBackgroundBlur";

    public static McShesternaBackgroundBlur New()
    {
        McShesternaBackgroundBlur mcShesternaBackgroundBlur = StaticPool<McShesternaBackgroundBlur>.New();
        mcShesternaBackgroundBlur.RefreshProperties();
        return mcShesternaBackgroundBlur;
    }

    public McShesternaBackgroundBlur()
        : base("chapter3More/McShesternaBackgroundBlur")
    {
    }

    public void Free()
    {
        StaticPool<McShesternaBackgroundBlur>.Free(this);
    }
}
