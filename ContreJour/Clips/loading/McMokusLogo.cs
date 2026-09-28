using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.loading;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McMokusLogo : Sprite, IFreeable, IId
{
    public const string ID = "loading/McMokusLogo";

    public string Id => "loading/McMokusLogo";

    public static McMokusLogo New()
    {
        McMokusLogo mcMokusLogo = StaticPool.New<McMokusLogo>();
        mcMokusLogo.RefreshProperties();
        return mcMokusLogo;
    }

    public McMokusLogo()
        : base("loading/McMokusLogo")
    {
    }

    public void Free()
    {
        StaticPool.Free<McMokusLogo>(this);
    }
}
